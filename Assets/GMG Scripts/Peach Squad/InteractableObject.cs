using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    public bool onPlayer = false;

    InputManager inputMgr;
    GameManager gameMgr;

    private InputManager.InputButton interactButton;

    public AudioClip soundFile;
    AudioSource audioSrc;

    GameObject player;
    protected Inventory playerInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.clip = soundFile;
        audioSrc.loop = false;
        audioSrc.playOnAwake = false;

        gameMgr = GameManager.Inst();
        gameMgr.onPause += OnPause;
        inputMgr = gameMgr.InputManager();

        player = GameObject.FindWithTag("Player");
        playerInventory = player.GetComponent<Inventory>();
        interactButton = playerInventory.interactButton;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputMgr.GetKeyDown(interactButton))
        {
            if (onPlayer)
            {
                Action();      
            }
            else { }
        }
        else { }
    }

    public virtual void Action()
    {
        Debug.Log("I'm a default action :)");
    }

    public virtual void PlaySound()
    {
        if (soundFile != null)
        {
            audioSrc.Play();
        }
        else
        {
            Debug.Log("No audio clip selected on "+this.gameObject);
        }
    }

    public void OnPause(bool pause)
    {
        if (pause) audioSrc.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.name == "InteractBox")||(collision.gameObject.CompareTag("Player")))
        {
            onPlayer = true;
        }
        else { }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((onPlayer) && ((collision.gameObject.name == "InteractBox") || (collision.gameObject.CompareTag("Player"))))
        {
            onPlayer = false;
        }
        else { }
    }
}
