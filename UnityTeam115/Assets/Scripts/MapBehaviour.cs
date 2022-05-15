using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class MapBehaviour : MonoBehaviour
{

    GameObject player;
    Grid grid;
    Tilemap tileMap, objectMap;
    Vector2 mousePosition;
    bool inputLocked;
    Image levelImage;
    TMP_Text levelText;
    public Sprite[] levelSprites;
    IDictionary<string, Sprite> spriteDict;
    public TextAsset levelData;

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
        tileMap = GameObject.Find("TileMap").GetComponent<Tilemap>();
        objectMap = GameObject.Find("ObjectMap").GetComponent<Tilemap>();
        levelImage = GameObject.Find("Level").GetComponent<Image>();
        levelText = GameObject.Find("LevelText").GetComponent<TMP_Text>();
    }

    void Start() {
        map = JsonUtility.FromJson<Levels>(levelData.text);
        spriteDict = new Dictionary<string, Sprite>();
        foreach(Sprite sprite in levelSprites) {
            spriteDict.Add(sprite.name, sprite);
        }
    }

    void InitializePlayerPosition() {
        player.transform.position = grid.CellToWorld(new Vector3Int(-6, -1, 0));
    }

    void OnAim() {
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    void OnClick() {
        Debug.Log(mousePosition + " <-> " + grid.WorldToCell(mousePosition));
    }

    void OnMoveRight() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x+1, newPosition.y, newPosition.z);
        if (tileMap.GetTile(newPosition) != null && !inputLocked) {
            inputLocked = true;
            StartCoroutine(MoveToPosition(grid.CellToWorld(newPosition)));
            CheckNewPosition(newPosition);
        }
    }

    void OnMoveLeft() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x-1, newPosition.y, newPosition.z);
        if (tileMap.GetTile(newPosition) != null && !inputLocked) {
            inputLocked = true;
            StartCoroutine(MoveToPosition(grid.CellToWorld(newPosition)));
            CheckNewPosition(newPosition);
        }
    }

    void OnMoveUp() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x, newPosition.y+1, newPosition.z);
        if (tileMap.GetTile(newPosition) != null && !inputLocked) {
            inputLocked = true;
            StartCoroutine(MoveToPosition(grid.CellToWorld(newPosition)));
            CheckNewPosition(newPosition);
        }
    }

    void OnMoveDown() {
        Vector3Int newPosition = grid.WorldToCell(player.transform.position);
        newPosition = new Vector3Int(newPosition.x, newPosition.y-1, newPosition.z);
        if (tileMap.GetTile(newPosition) != null && !inputLocked) {
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
    }
}
