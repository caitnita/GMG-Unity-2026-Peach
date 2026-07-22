using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonActionAssign : MonoBehaviour
{
    private GameState gameState;
    public bool isLoadGame;
    public bool isLoadScene;
    public bool isQuit;
    private Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        button = GetComponent<Button>();

        gameState = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<GameState>();
        if (isLoadGame)
        {
            button.onClick.AddListener(Restart);
        }
        else if (isLoadScene)
        {
            button.onClick.AddListener(LoadScene);
        }
        else if (isQuit)
        {
            button.onClick.AddListener(Quit);
        }
    }

    void Restart()
    {
        gameState.RestartGame();
    }

    void LoadScene()
    {
        gameState.LoadScene();
    }

    private void Quit()
    {
        gameState.ExitGame();
    }
}
