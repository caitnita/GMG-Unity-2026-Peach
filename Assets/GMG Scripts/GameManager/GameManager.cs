using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameSettings
{
    public static bool IsDebugMode = false;
    public bool GenerationMode = false;

    public float MasterVolume = 1f;
    public float MusicVolume = 0.8f;
    public float SfxVolume = 1f;
}

public class GameManager : MonoBehaviour {

	private static GameManager gameMgr;
    public GameSettings Settings{ get; private set; }

    public InputManager.InputButton pauseButton;
    public GameObject pauseMenu;

    private float unpauseDelayTime = 1;
    private bool unpauseDelay = false;

    public bool playerOccupied = false;

    public static GameManager Inst()
	{
		if (gameMgr != null) return gameMgr;

		GameManager[] gameMgrs = Object.FindObjectsByType(typeof(GameManager),FindObjectsSortMode.None) as GameManager[];
		foreach (GameManager gameManager in gameMgrs)
		{
			gameMgr = gameManager;
		}

		if (gameMgr == null)
		{
			GameObject gObject = Instantiate(Resources.Load("GameManager")) as GameObject;
			gameMgr = gObject.GetComponent<GameManager>();
		}

		if (gameMgr == null)
		{
			GameObject gameObj = new GameObject();
			gameMgr = gameObj.AddComponent<GameManager>();
		}

		return gameMgr;
	}

    public bool isPaused = false;
    public bool isStopped = false;
    public delegate void PauseHandler(bool pause);
    public event PauseHandler onPause;
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        if (onPause != null) onPause(true);
        Cursor.visible = true;
        if (pauseMenu)
        {
            pauseMenu.SetActive(true);
        }
    }
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (onPause != null) onPause(false);
        Cursor.visible = false;
        if (pauseMenu){
            pauseMenu.SetActive(false);
        }
        StopCoroutine(UnpauseDelay());
        unpauseDelay = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void StopGame()
    {
        isPaused = true;
        isStopped = true;
        Time.timeScale = 0;
    }

    //Getters
    	InputManager inputMgr;
    	public InputManager InputManager()
    	{
    		if (inputMgr != null) return inputMgr;
    		inputMgr = GetComponent<InputManager>();

    		if (inputMgr == null)
    		{
    			inputMgr = gameObject.AddComponent<InputManager>();
    		}

    		return inputMgr;
    	}

		/*DialogueManager dialogueMgr;
    	public DialogueManager DialogueManager()
    	{
    		if (dialogueMgr != null) return dialogueMgr;
    		dialogueMgr = GetComponent<DialogueManager>();

    		if (dialogueMgr == null)
    		{
    			dialogueMgr = gameObject.AddComponent<DialogueManager>();
    		}
    		
    		return dialogueMgr;
    	}*/
    	
	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        if (pauseMenu)
        {
            pauseMenu.gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        // When our selected input button is pressed...
        if (inputMgr.GetKeyDown(pauseButton))
        {
            if (!isPaused)
            {
                PauseGame();
                StartCoroutine(UnpauseDelay());
            }
            else if ((isPaused)&&(!unpauseDelay)&&(!isStopped))
            {
                UnpauseGame();
            }
        }
        else { }
    }

    IEnumerator UnpauseDelay()
    {
        unpauseDelay = true;
        yield return new WaitForSecondsRealtime(unpauseDelayTime);
        unpauseDelay = false;
    }
}
