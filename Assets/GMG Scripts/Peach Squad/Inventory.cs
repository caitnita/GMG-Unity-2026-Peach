using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Ingredient> inventory;
    public InputManager.InputButton interactButton = InputManager.InputButton.Action1;

    public float maxInventory;

    public bool pickupWait = false;
    public float pickupDelay = 1f;

    public GridLayoutGroup uiBubble;
    public Image image;
    private Vector2 cellChange;
    private Vector2 cachedCellSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cachedCellSize = uiBubble.cellSize;
        ClearInventory();
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

    public IEnumerator PickupDelay()
    {
        pickupWait = true;
        yield return new WaitForSeconds(pickupDelay);
        pickupWait = false;
    }

    public void PickUp(Ingredient ingredient)
    {
        if (inventory.Count == 0)
        {
            uiBubble.gameObject.SetActive(true);
            uiBubble.cellSize = cachedCellSize;
        }
        else { }

        inventory.Add(ingredient);
        var newImage = Instantiate(image, uiBubble.gameObject.transform);
        newImage.sprite = ingredient.sprite;

        var cellChangeAmt = 5f;
        if (inventory.Count > 12)
        {
            cellChangeAmt = 0.5f;
        }
        else if (inventory.Count > 9)
        {
            cellChangeAmt = 2.5f;
        }
        else { }

            cellChange = new Vector2(cellChangeAmt, cellChangeAmt);
        uiBubble.cellSize -= cellChange;
    }

    public void ClearInventory()
    {
        inventory.Clear();
        var items = GameObject.FindGameObjectsWithTag("InventoryItem");
        foreach (var item in items)
        {
            Destroy(item);
        }
        uiBubble.gameObject.SetActive(false);
    }
}
