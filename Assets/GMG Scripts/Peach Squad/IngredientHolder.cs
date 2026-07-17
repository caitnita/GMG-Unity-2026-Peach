using UnityEngine;

public class IngredientHolder : InteractableObject
{
    [Header("Sound file is assigned from ingredient scriptable object")]
    public Ingredient ingredient;

    // We're overriding the Start() function in our base InteractableObject class...
    protected override void Start()
    {
        soundFile = ingredient.sound;
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
                playerInventory.inventory.Add(ingredient);
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
