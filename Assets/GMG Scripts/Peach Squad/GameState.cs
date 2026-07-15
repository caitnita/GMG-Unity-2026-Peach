using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    InputManager inputMgr;

    public int gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputMgr = GameObject.Find("Game Manager").GetComponent<InputManager>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            gameState = 0;
        }
    }

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Load scenes by reference in Scene List.
    // 0: Main Menu
    // 1: Cutscenes
    // 2: Main Game Level
    // 3: Credits

    // Game States:
    // 0: Main menu
    // 1: Cutscene 1
    // 2: Main game
    // 3: Cutscene 2
    // 4: Game over/credits
    public void LoadScene()
    {
        Debug.Log("Load next scene! :)");
        /*
        if (gameState == 0)
        {
            gameState = 2;
            SceneManager.LoadScene(1);
        }
        else if (gameState == 1)
        {
            gameState++;
            SceneManager.LoadScene(2);
        }
        else if (gameState == 2)
        {
            gameState++;
            SceneManager.LoadScene(1);
        }
        else if (gameState == 3)
        {
            gameState++;
            SceneManager.LoadScene(3);
        }
        else if (gameState == 4)
        {
            gameState = 0;
            SceneManager.LoadScene(0);
        }*/
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("I'm quitting the game :D");
    }
}
