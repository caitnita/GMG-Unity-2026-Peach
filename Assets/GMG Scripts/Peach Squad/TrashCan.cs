using UnityEngine;

public class TrashCan : InteractableObject
{
    // We're overriding the Action() function in our base InteractableObject class
    public override void Action()
    {
        if (!playerInventory.pickupWait)
        {
            if (playerInventory.inventory.Count > 0)
            {
                playerInventory.ClearInventory();
                PlaySound(null);
                Debug.Log("Emptied inventory");
            }
            else
            {
                Debug.Log("Player not holding anything...");
            }
        }
        else
        {
            // Player picked up an item too recently
        }

    }
}
