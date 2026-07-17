using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem.Android;

public class CutsceneController : MonoBehaviour
{
    public Image image;
    public List<Sprite> cutscene1 = new List<Sprite>();

    private GameState gameState;

    private int counter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image.sprite = cutscene1[0];

        gameState = GameObject.Find("SceneManager").GetComponent<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Advance()
    {
        if (counter < (cutscene1.Count-1))
        {
            counter++;
            image.sprite = cutscene1[counter];
        }
        else
        {
            gameState.LoadScene();
        }   
    }
}
