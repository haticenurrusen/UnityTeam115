using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    public void newGame() {
        SceneManager.LoadScene("Map");
    }

    public void exit() {
        Application.Quit();
    }
}
