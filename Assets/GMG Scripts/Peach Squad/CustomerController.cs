using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : InteractableObject
{
    // We have a Customer scriptable object that holds the info on this customer, such as the name, sprite, animator, and sound effects
    public Customer customer;

    // The customer order is stored as a list of Ingredient scriptable objects
    public List<Ingredient> order;

    // We'll store the spawn location of the customer as a float for later.
    [HideInInspector]
    public float spawnLocation;

    // The amount of time a customer will wait overall
    public float patienceMax;

    // Our timer to track how long the customer is still willing to wait
    public float patience;

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
            // Customer unassigned
        }

        gameManager = GameObject.Find("Game Manager");
        difficultyManager = gameManager.GetComponent<DifficultyManager>();
        customerManager = gameManager.GetComponent<CustomerManager>();
    }

    // We're overriding the Action() function in our base InteractableObject class
    public override void Action()
    {
        // Check the player's inventory
        inventory = playerInventory.inventory;

        // Make sure player is holding something first
        if (inventory.Count != 0)
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

            // If the player checked the order, their inventory should be cleared.
            playerInventory.inventory.Clear();

            // After checking the order, this customer needs to wrap it up!
            StartCoroutine(Leave());
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
