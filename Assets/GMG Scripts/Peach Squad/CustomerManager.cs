using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    // This script will hold our lists of different customer types, spawn customers,
    // and control how many customer spawn points there are.

    private DifficultyManager DifficultyManager;

    public GameObject customerPrefab;

    public List<Customer> customers;
    public List<Ingredient> ingredients;

    private List<Customer> normalCustomers = new List<Customer>();
    private List<Customer> creepyCustomers = new List<Customer>();
    private List<Customer> cursedCustomers = new List<Customer>();
    private List<Customer> customerPool;

    private List<Ingredient> ingredientPool = new List<Ingredient>();
    private List<Ingredient> cursedIngredients = new List<Ingredient>();
    private List<Ingredient> requiredIngredients = new List<Ingredient>();
    private List<Ingredient> order = new List<Ingredient>();

    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;

    // Timers for our different spawn locations
    public float spawn1Timer;
    private float spawn2Timer;
    private float spawn3Timer;

    private bool spawn1Occupied;
    private bool spawn2Occupied;
    private bool spawn3Occupied;

    public float defaultSpawnTime;
    public float spawnTime;
    
    // The amount of time to delay at the beginning of the game before spawning the first customer
    public float startSpawnDelay;

    // The amount of time to delay after delivering the order, and the customer leaving (regardless of correct/wrong)
    public float destroyDelay;

    public float defaultOrderSize;
    public float orderSizeIncrease;
    private float orderSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DifficultyManager = GetComponent<DifficultyManager>();

        // Duplicate our ingredient list into ingredientPool, so we can edit this list to remove required ingredients and store in a separate list
        ingredientPool = new List<Ingredient>(ingredients);

        // Check each ingredientPool entry for the isRequired/isCursed bool, and add those flagged ingredients to the requiredIngredients/cursedIngredients lists
        for (int i = 0;i < ingredientPool.Count; i++)
        {
            if (ingredientPool[i].isRequired)
            {
                requiredIngredients.Add(ingredientPool[i]);
                ingredientPool.RemoveAt(i);
                i--;
            }
            else if (ingredientPool[i].isCursed)
            {
                cursedIngredients.Add(ingredientPool[i]);
                ingredientPool.RemoveAt(i);
                i--;
            }
            else { }
        }

        // Sort our provided customer list by customer type into separate lists.
        for (int j = 0; j < customers.Count; j++)
        {
            if (customers[j].customerType == 1)
            {
                normalCustomers.Add(customers[j]);
            }
            else if (customers[j].customerType == 2)
            {
                creepyCustomers.Add(customers[j]);
            }
            else if (customers[j].customerType == 3)
            {
                cursedCustomers.Add(customers[j]);
            }
            else
            {
                Debug.Log("Incorrect Customer Value on " + customers[j].name);
            }
        }

        Phase1();
    }

    // Update is called once per frame
    void Update()
    {
        // In Update(), we will track our spawn timers for each customer spawn location.

        // If a spawn location is un-occupied (empty) and the timer is greater than zero...
        if ((!spawn1Occupied) && (spawn1Timer > 0))
        {
            // Decrease the timer every tick.
            spawn1Timer -= Time.deltaTime;
            //Debug.Log(spawn1Timer);
        }
        // But if the above isn't true, and the spot isn't occupied...
        // (this will run once the timer hits zero)
        else if (!spawn1Occupied)
        {
            // We'll make sure the timer is set exactly at zero, just in case :)
            spawn1Timer = 0;

            // and spawn a customer in the first slot.
            Spawn1();
        }
        else { }

        if ((!spawn2Occupied) && (spawn2Timer > 0))
        {
            spawn2Timer -= Time.deltaTime;
        }
        else if (!spawn2Occupied)
        {
            spawn2Timer = 0;
            Spawn2();
        }
        else { }

        if ((!spawn3Occupied) && (spawn3Timer > 0))
        {
            spawn3Timer -= Time.deltaTime;
        }
        else if (!spawn3Occupied)
        {
            spawn3Timer = 0;
            Spawn3();
        }
        else { }
    }

    public void Spawn1()
    {
        // Set the corresponding "occupied" boolean to true, so that we don't keep trying to spawn more customers in the same spot!
        spawn1Occupied = true;

        // Create a copy of the Customer Prefab, at the position and rotation of the spawn1 game object
        GameObject customerSpawn = Instantiate(customerPrefab,spawn1.transform.position,spawn1.transform.rotation);

        // Get the customer controller script on the newly created game object
        CustomerController customerController = customerSpawn.GetComponent<CustomerController>();

        // Set the Customer scriptable object on the game object to a random one from our customer pool,
        // and assign the correct spawn location.
        customerController.customer = GetCustomer();
        customerController.spawnLocation = 1f;
        customerController.order = CreateOrder(customerController.customer.customerType);
    }

    public void Spawn2()
    {
        spawn2Timer = spawnTime;
        spawn2Occupied = true;
    }

    public void Spawn3()
    {
        spawn3Timer = spawnTime;
        spawn3Occupied = true;
    }

    public void EmptySlot(float location)
    {
        if (location == 1f)
        {
            // For the corresponding location, reset the occupied boolean to false,
            // and reset the timer back to the maximum time.
            spawn1Occupied = false;
            spawn1Timer = spawnTime;
        }
        else if (location == 2f)
        {
            spawn2Occupied = false;
            spawn2Timer = spawnTime;
        }
        else if (location == 3f)
        {
            spawn3Occupied = false;
            spawn3Timer = spawnTime;
        }
        else { Debug.Log("ermmm something is wrong"); }
    }

    public Customer GetCustomer()
    {
        return customerPool[Random.Range(0, customerPool.Count)];
    }

    public void Phase1()
    {
        Debug.Log("Starting Phase 1");
        spawn1Occupied = true;
        customerPool = new List<Customer>(normalCustomers);
        spawnTime = defaultSpawnTime;
        orderSize = defaultOrderSize;

        StartCoroutine(SpawnDelay());
    }

    public void Phase2()
    {
        customerPool.AddRange(creepyCustomers);
        orderSize += orderSizeIncrease;
    }

    public void Phase3()
    {
        customerPool = new List<Customer>(creepyCustomers);
        customerPool.AddRange(cursedCustomers);
        orderSize += orderSizeIncrease;
    }

    IEnumerator SpawnDelay()
    {
        // Wait for our defined amount of time, then spawn our first customer
        yield return new WaitForSeconds(startSpawnDelay);
        Spawn1();
    }

    private List<Ingredient> CreateOrder(int customerType)
    {
        // Clear any previous order info
        order.Clear();

        // Set our working ingredient pool back to default
        List<Ingredient> currIngredientPool = new List<Ingredient>(ingredientPool);

        // If customer is cursed type, add cursed ingredients to the ingredient pool.
        if (customerType == 3)
        {
            currIngredientPool.AddRange(cursedIngredients);
        }
        else { }

        // Add all required items to the order.
        order.AddRange(requiredIngredients);

        for (var i = order.Count; i < orderSize; i++)
        {
            var item = Random.Range(0, currIngredientPool.Count);
            order.Add(currIngredientPool[item]);

            // If the ingredient is unique...
            if (currIngredientPool[item].isUnique)
            {
                // Remove it from our current ingredient pool.
                currIngredientPool.RemoveAt(item);
            }
            else { }
        }

        return order;
    }
}
