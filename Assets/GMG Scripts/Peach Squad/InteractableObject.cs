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
    protected virtual void Start()
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
        // When our selected input button is pressed...
        if (inputMgr.GetKeyDown(interactButton))
        {
            // Make sure that the player is interacting with *this* object (are they touching it?)
            if (onPlayer)
            {
                // Do the thing!
                Action();
            }
            else { }
        }
        else { }

        // These else statements will make sure that if the conditions aren't met, nothing will happen!
    }

    public virtual void Action()
    {
        // Our subclass scripts will override this function.
        // If we ever forget to make an override, we'll see this fun message in the console
        Debug.Log("I'm a default action :)");
    }

    public virtual void PlaySound()
    {
        // If we have a sound file selected...
        if (soundFile != null)
        {
            // Use our audio source to play the sound file.
            audioSrc.Play();
        }
        else
        {
            //Debug.Log("No audio clip selected on "+this.gameObject);
        }
    }

    public void OnPause(bool pause)
    {
        if (pause) audioSrc.Stop();
    }

    // Check for trigger enters that match either the "Player" tag or the "InteractBox" name.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.name == "InteractBox")||(collision.gameObject.CompareTag("Player")))
        {
            onPlayer = true;
        }
        else { }
    }

    // Check for trigger exits that match either the "Player" tag or the "InteractBox" name.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((onPlayer) && ((collision.gameObject.name == "InteractBox") || (collision.gameObject.CompareTag("Player"))))
        {
            onPlayer = false;
        }
        else { }
    }
}
