using UnityEngine;

public class TrashCan : InteractableObject
{
    // We're overriding the Action() function in our base InteractableObject class
    public override void Action()
    {
        if (playerInventory.inventory.Count > 0)
        {
            playerInventory.inventory.Clear();
            PlaySound();
            Debug.Log("Emptied inventory");
        }
        else
        {
            Debug.Log("Player not holding anything...");
        }
    }
}
