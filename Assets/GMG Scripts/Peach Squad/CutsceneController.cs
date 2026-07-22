using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour
{
    public Image image;
    public List<Sprite> cutscene1 = new List<Sprite>();
    public List<Sprite> cutscene2 = new List<Sprite>();
    private List<Sprite> currCutscene;

    public int waitTime;
    public bool waiting;

    private GameState gameState;

    private int counter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Wait());
        gameState = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<GameState>();

        if (gameState.gameState == 1)
        {
            currCutscene = new List<Sprite>(cutscene1);
        }
        else if (gameState.gameState == 3)
        {
            currCutscene = new List<Sprite>(cutscene2);
        }
        else
        {
            Debug.Log("Missing cutscene list, or wrong game state value");
        }
        
        image.sprite = currCutscene[0];
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { }
        else if (Input.anyKeyDown)
        {
            CheckAdvance();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void CheckAdvance()
    {
        if (!waiting)
        {
            Advance();
        }
        else { }
    }

    public void Advance()
    {
        if (counter < (currCutscene.Count-1))
        {
            StartCoroutine(Wait());
            counter++;
            image.sprite = currCutscene[counter];
        }
        else
        {
            gameState.LoadScene();
        }   
    }

    IEnumerator Wait()
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(waitTime);
        waiting = false;
    }
}
