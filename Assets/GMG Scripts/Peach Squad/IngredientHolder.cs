using UnityEngine;

public class IngredientHolder : InteractableObject
{
    [Header("Sound file is assigned from ingredient scriptable object")]
    public Ingredient ingredient;

    // We're overriding the Start() function in our base InteractableObject class...
    protected override void Start()
    {
        if (ingredient == null)
        {
            Debug.Log(this.gameObject.name + " game object is missing an ingredient assignment.");
            this.gameObject.SetActive(false);
        }
        else if (ingredient.sound != null)
        {
            soundFile = ingredient.sound;
        }
        else
        {
            Debug.Log(ingredient.name + " Ingredient scriptable object is missing a sound file.");
        }

        // But we still want the old Start() to run, so we're using base.Start() in our override.
        base.Start();
    }

    // We're overriding the Action() function in our base InteractableObject class
    public override void Action()
    {
        if (ingredient == null)
        {
            Debug.Log("Ingredient ScriptableObject not assigned.");
        }
        else
        {
            if (!playerInventory.pickupWait)
            {
                playerInventory.PickUp(ingredient);
                PlaySound(null);

                Debug.Log("Picked up " + ingredient.name);

                StartCoroutine(playerInventory.PickupDelay());
            }
            else
            {
                // Player picked something up too recently
            }

        }
    }
}
