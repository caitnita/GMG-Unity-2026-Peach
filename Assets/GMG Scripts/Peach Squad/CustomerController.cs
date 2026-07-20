using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : InteractableObject
{
    // We have a Customer scriptable object that holds the info on this customer, such as the name, sprite, animator, and sound effects
    public Customer customer;

    // The customer order is stored as a list of Ingredient scriptable objects
    public List<Ingredient> order;

    // We'll store the spawn location of the customer as a float for later.
    [HideInInspector]
    public float spawnLocation;

    // A gameobject that will visualize our customer's patience for us
    public Image patienceBar;

    // The amount of time a customer will wait overall
    [HideInInspector]
    public float patienceMax;

    // Our timer to track how long the customer is still willing to wait
    public float patience;

    // A check for whether or not we've already played the patience sound at half patience
    private bool playedWaitSound = false;

    // A check for if the customer is already on their way out, so we don't keep checking other properties
    private bool isLeaving = false;

    // We'll need to access this customer's animator to play animations or set animator variables,
    // and the sprite renderer to set the default sprite.
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // We'll need to access the Game Manager object to find and access the Difficulty Manager and Customer Manager scripts
    private GameObject gameManager;
    private DifficultyManager difficultyManager;
    private CustomerManager customerManager;

    // We'll create a new list to store the current player inventory for reference when the player interacts with the customer
    private List<Ingredient> inventory;

    public Color fullPatienceColor;
    public Color emptyPatienceColor;
    private Gradient gradient = new Gradient();
    private Vector3 patienceBarScale = new Vector3(1, 1, 1);
    private GradientColorKey[] colors = new GradientColorKey[2];
    private GradientAlphaKey[] alphas = new GradientAlphaKey[2];

    public GridLayoutGroup uiBubble;
    public Image image;

    // We're overriding the Start() function in our base InteractableObject class...
    protected override void Start()
    {
        // But we still want the old Start() to run, so we're using base.Start() in our override.
        base.Start();

        //Debug.Log("Do I know my name yet? " + customer.name);

        // We'll get our necessary components and scripts connected to the right places
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (customer != null)
        {
            spriteRenderer.sprite = customer.sprite;
            anim = customer.animator;
            PlaySound(customer.spawnSound);
        }
        else
        {
            Debug.Log("Destroying placeholder customer");
            Destroy(this.gameObject);
        }

        // Find our required components
        gameManager = GameObject.Find("Game Manager");
        difficultyManager = gameManager.GetComponent<DifficultyManager>();
        customerManager = gameManager.GetComponent<CustomerManager>();

        // Set up patient bar and gradient colors
        patience = patienceMax;
        colors[0] = new GradientColorKey(emptyPatienceColor, 0.0f);
        colors[1] = new GradientColorKey(fullPatienceColor, 1.0f);
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);
        gradient.SetKeys(colors,alphas);

        // Set up our order UI Bubble
        for (int i = 0;i<order.Count;i++)
        {
            var newImage = Instantiate(image, uiBubble.gameObject.transform);
            newImage.sprite = order[i].sprite;
            newImage.name = order[i].name;
        }

        var cellSize = 100f;
        if (order.Count > 12)
        {
            cellSize = 25;
        }
        else if (order.Count > 9)
        {
            cellSize = 50;
        }
        else if (order.Count > 4)
        {
            cellSize = 75f;
        }
        else { }
        uiBubble.cellSize = new Vector2(cellSize, cellSize);
    }

    protected override void Update()
    {
        base.Update();
        // If we checked usePatience in the CustomerManager script...
        if (customerManager.usePatience)
        {
            // and if patience is greater than zero, and we're not already leaving
            if ((patience > 0) && (!isLeaving))
            {
                // Decrease patience by Time.deltaTime
                patience -= Time.deltaTime;

                // Set our patienceBarScale x value to our percentage of remaining patience
                patienceBarScale.x = patience / patienceMax;

                // Update the transform scale of the patienceBar UI image to our private Vector
                patienceBar.transform.localScale = patienceBarScale;

                // Update the color of the patienceBar image using our Gradient Color Key
                patienceBar.color = gradient.Evaluate(patience / patienceMax);

                // If we haven't played the patience sound yet,
                // AND our current patience is less than half our maximum patience...
                if ((!playedWaitSound) && (patience < (patienceMax / 2)))
                {
                    // Set the playedWaitSound check to true, and play the wait sound.
                    playedWaitSound = true;
                    PlaySound(customer.waitSound);
                }
            }
            // Else (if patience is at zero or less) AND we're not already leaving
            else if (!isLeaving)
            {
                StartCoroutine(Leave());
                Debug.Log(this.gameObject.name + " is out of patience!");
                PlaySound(customer.failureSound);
                difficultyManager.FailOrder();
            }
        }
        else { }
    }

    // We're overriding the Action() function in our base InteractableObject class
    public override void Action()
    {
        Debug.Log("Action");
        // Check the player's inventory
        inventory = playerInventory.inventory;

        // Make sure player is holding something first, and that the customer isn't already leaving
        if ((inventory.Count != 0) && (!isLeaving))
        {
            if (CheckOrder())
            {
                Debug.Log("Correct order!");

                PlaySound(customer.successSound);

                // Tell the difficulty manager!
                difficultyManager.CompleteOrder();
            }
            else
            {
                Debug.Log("Wrong order!");

                PlaySound(customer.failureSound);

                // Tell the difficulty manager!
                difficultyManager.FailOrder();
            }
            // After checking the order, this customer needs to wrap it up!
            StartCoroutine(Leave());

            // If the player checked the order, their inventory should be cleared.
            playerInventory.ClearInventory();
        }
        else {
            PlaySound(customer.waitSound);
        }
    }

    bool CheckOrder()
    {
        // If the length of the order list and inventory list don't match...
        if (order.Count != inventory.Count)
        {
            // ... the order is defintely wrong!
            return false;
        }
        else
        {
            // If the lengths match, check each item in both lists against eachother to see if they match.
            for (int i = 0; i < order.Count; i++)
            {
                if (order[i] != inventory[i])
                {
                    // If there's ever a mismatch, the order is wrong.
                    return false;
                }
                else { }
            }
            // If there's no mismatches, the order is correct!
            return true;
        }
    }

    IEnumerator Leave()
    {
        // We'll wait a minute before deleting the customer, so they have time for a sound effect, animation,
        // and for the player to be able to register if they succeeded on or failed the order.
        isLeaving = true;

        float soundLength;
        soundLength = Mathf.Max(customer.failureSound.length, customer.successSound.length);
        if (soundLength > customerManager.destroyDelay)
        {
            yield return new WaitForSeconds(soundLength);
        }
        else
        {
            yield return new WaitForSeconds(customerManager.destroyDelay);
        }
            
        // Tell the Customer Manager script that this customer's spawn slot is now empty!
        customerManager.EmptySlot(spawnLocation);

        // delete this guy!
        Debug.Log(this.customer.name + " is leaving!");
        Destroy(this.gameObject);
    }
}
