using UnityEngine;

public class IngredientHolder : InteractableObject
{
    public Ingredient ingredient;

    // We're overriding the Action() function in our base InteractableObject class
    public override void Action()
    {
        if (ingredient == null)
        {
            Debug.Log("Ingredient ScriptableObject not assigned.");
        }
        else
        {
            playerInventory.inventory.Add(ingredient);
            PlaySound();

            Debug.Log("Picked up " + ingredient.name);
        }
    }
}
