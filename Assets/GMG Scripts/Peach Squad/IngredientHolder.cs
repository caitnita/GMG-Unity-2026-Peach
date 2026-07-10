using UnityEngine;

public class IngredientHolder : InteractableObject
{
    public Ingredient ingredient;

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
        }
    }
}
