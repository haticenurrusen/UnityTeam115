using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MapBehaviour : MonoBehaviour
{
    public Sprite[] levelSprites;
    public TextAsset levelData;
    GameObject player;
    Grid grid;
    Tilemap tileMap, objectMap;
    Vector2 mousePosition;
    bool inputLocked;
    Image levelImage;
    TMP_Text levelText;
    IDictionary<string, Sprite> spriteDict;
    GameObject enter;
    private List<Vector3Int> locks;

    bool lockHintShown;
    Vector3Int lockHintPosition;
    GameObject lockHint;

    [Serializable]
    public class Level
    {
        public Vector3Int position;
        public string name, spriteName;
    }

    [Serializable]
    public class Levels
    {
        public Level[] levels;
    }

    Levels map;

    void Awake()
    {
        player = GameObject.Find("Player");
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        InitializePlayerPosition();
        InitializeLocks();
        tileMap = GameObject.Find("TileMap").GetComponent<Tilemap>();
        objectMap = GameObject.Find("ObjectMap").GetComponent<Tilemap>();
        levelImage = GameObject.Find("Level").GetComponent<Image>();
        enter = GameObject.Find("Enter");
        levelText = GameObject.Find("LevelText").GetComponent<TMP_Text>();
    }

    void Start() {
        map = JsonUtility.FromJson<Levels>(levelData.text);
        spriteDict = new Dictionary<string, Sprite>();
        foreach(Sprite sprite in levelSprites) {
            spriteDict.Add(sprite.name, sprite);
        }
        enter.SetActive(false);
    }

    void InitializePlayerPosition() {
        player.transform.position = grid.CellToWorld(new Vector3Int(-9, -2, 0));
    }

    void InitializeLocks() {
        lockHint = GameObject.Find("LockHint");
        lockHint.SetActive(false);
        lockHintPosition = new Vector3Int(-1, -2, 0);
        locks = new List<Vector3Int>() {new Vector3Int(0, -2, 0),
                                        new Vector3Int(3, 0, 0),
                                        new Vector3Int(-2, 1, 0)};
        lockHintShown = false;
    }

    void OnAim() {
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    void OnClick() {
        Debug.Log(mousePosition + " <-> " + grid.WorldToCell(mousePosition));
    }

    void OnSelect() {
        if (enter.activeSelf) {
            SceneManager.LoadScene(levelText.text);
        }
    }

    void OnMoveRight() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x+1, newPosition.y, newPosition.z);
        if (tileMap.GetTile(newPosition) != null && !locks.Contains(newPosition) && !inputLocked) {
            inputLocked = true;
            StartCoroutine(MoveToPosition(grid.CellToWorld(newPosition)));
            CheckNewPosition(newPosition);
        }
    }

    void OnMoveLeft() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x-1, newPosition.y, newPosition.z);
        if (tileMap.GetTile(newPosition) != null && !locks.Contains(newPosition) && !inputLocked) {
            inputLocked = true;
            StartCoroutine(MoveToPosition(grid.CellToWorld(newPosition)));
            CheckNewPosition(newPosition);
        }
    }

    void OnMoveUp() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x, newPosition.y+1, newPosition.z);
        if (tileMap.GetTile(newPosition) != null && !locks.Contains(newPosition) && !inputLocked) {
            inputLocked = true;
            StartCoroutine(MoveToPosition(grid.CellToWorld(newPosition)));
            CheckNewPosition(newPosition);
        }
    }

    void OnMoveDown() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x, newPosition.y-1, newPosition.z);
        if (tileMap.GetTile(newPosition) != null&& !locks.Contains(newPosition) && !inputLocked) {
            inputLocked = true;
            StartCoroutine(MoveToPosition(grid.CellToWorld(newPosition)));
            CheckNewPosition(newPosition);
        }
    }

    IEnumerator MoveToPosition(Vector3 target) {
        while(player.transform.position != target) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, Time.deltaTime*10);
            yield return StartCoroutine(Wait(Time.deltaTime/10));
        }
        yield return StartCoroutine(Wait(Time.deltaTime/10));
        inputLocked = false;
    }

    IEnumerator Wait(float time) {
        yield return new WaitForSeconds(time);
    }

    void CheckNewPosition(Vector3Int newPosition) {
        bool levelFound = false;

        foreach(Level level in map.levels) {
            if (level.position == newPosition && spriteDict.ContainsKey(level.spriteName)) {
                levelImage.sprite = spriteDict[level.spriteName];
                levelText.text = level.name;
                levelFound = true;
                break;
            }
        }

        if (!levelFound) {
            levelImage.sprite = spriteDict["nothing"];
            levelText.text = "Nowhere";
        }

        if (!lockHintShown && newPosition == lockHintPosition) {
            StartCoroutine(ShowLockHint());
        }

        enter.SetActive(levelFound);
    }

    IEnumerator ShowLockHint() {
        lockHint.SetActive(true);
        yield return new WaitForSeconds(3f);
        lockHint.SetActive(false);
        lockHintShown = true;
        yield return null;  
    }
}
