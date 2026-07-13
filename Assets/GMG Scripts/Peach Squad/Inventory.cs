using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Ingredient> inventory;
    public InputManager.InputButton interactButton = InputManager.InputButton.Action1;

    public float maxInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((maxInventory != 0) && (inventory.Count > maxInventory))
        {
            Debug.Log("Too many items in inventory, removing most recent addition!");
            inventory.RemoveAt(inventory.Count-1);
        }
        else { }
    }
}
