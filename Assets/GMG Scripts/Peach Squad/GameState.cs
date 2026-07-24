using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    InputManager inputMgr;

    public int gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameObject.Find("Game Manager"))
        {
            inputMgr = GameObject.Find("Game Manager").GetComponent<InputManager>();
        }
        else { }

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(EscapeKeyHeld());
        }
        
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            StopAllCoroutines();
        }
    }

    // Load scenes by reference in Scene List.
    // 0: Main Menu
    // 1: Cutscenes
    // 2: Main Game Level
    // 3: Game over
    // 4: Credits

    // Game States:
    // 0: Main menu
    // 1: Cutscene 1
    // 2: Main game
    // 3: Cutscene 2
    // 4: Game over
    public void LoadScene()
    {
        Debug.Log("Load next scene! :)");
        if (gameState == 0)
        {
            gameState++;
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
        }
    }

    public void RestartGame()
    {
        gameState = 1;
        LoadScene();
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("I'm quitting the game :D");
    }

    public void Credits()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            // Return to correct scene
            if (gameState == 0)
            {
                SceneManager.LoadScene(0);
            }
            else if (gameState == 4)
            {
                SceneManager.LoadScene(3);
            }
            else
            {
                Debug.Log("Wrong game state value");
            }
        }
        else
        {
            // Go to credits
            SceneManager.LoadScene(4);
        } 
    }

    IEnumerator EscapeKeyHeld()
    {
        yield return new WaitForSecondsRealtime(3f);
        ExitGame();
    }
}
