using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    // This script will hold our lists of different customer types, spawn customers,
    // and control how many customer spawn points there are.

    private DifficultyManager difficultyManager;
    private GameManager gameManager;

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

    [Header("Spawn location GameObjects")]
    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;

    // Timers for our different spawn locations
    public float spawn1Timer;
    public float spawn2Timer;
    public float spawn3Timer;

    private bool spawn1Occupied;
    private bool spawn2Occupied;
    private bool spawn3Occupied;

    [Header("Spawn time settings")]
    public float defaultSpawnTime = 10f;
    public float spawnTimeDecrease = 1f;
    private float spawnTime;
    
    // The amount of time to delay at the beginning of the game before spawning the first customer
    public float startSpawnDelay = 2f;

    // The amount of time to delay after delivering the order, and the customer leaving (regardless of correct/wrong)
    public float destroyDelay = 2f;

    [Header("Order size settings")]
    public float defaultOrderSizeMin = 3f;
    public float defaultOrderSizeMax = 4f;
    public float orderSizeIncrease = 1f;
    private float currOrderSizeMin;
    private float currOrderSizeMax;
    private float orderSize;

    [Header("Patience settings")]
    public bool usePatience;
    public float defaultMaxPatience = 30f;
    public float minPatience = 5f;
    public float patienceDecrease = 1f;
    private float currMaxPatience;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        difficultyManager = GetComponent<DifficultyManager>();
        gameManager = GetComponent<GameManager>();

        spawn1Occupied = true;
        spawn2Occupied = true;
        spawn3Occupied = true;

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
            Spawn(1);
        }
        else { }

        if ((!spawn2Occupied) && (spawn2Timer > 0))
        {
            spawn2Timer -= Time.deltaTime;
        }
        else if (!spawn2Occupied)
        {
            spawn2Timer = 0;
            Spawn(2);
        }
        else { }

        if ((!spawn3Occupied) && (spawn3Timer > 0))
        {
            spawn3Timer -= Time.deltaTime;
        }
        else if (!spawn3Occupied)
        {
            spawn3Timer = 0;
            Spawn(3);
        }
        else { }
    }

    public void Spawn(float location)
    {
        // Create a local variable to hold the game object we're using to set the location of each spawn point
        GameObject spawnLocationObject = null;

        // Set the corresponding "occupied" boolean to true, so that we don't keep trying to spawn more customers in the same spot!
        // Set the spawnLocationObject to the corresponding spawn point object
        if (location == 1)
        {
            spawn1Occupied = true;
            spawnLocationObject = spawn1;
        }
        else if (location == 2)
        {
            spawn2Occupied = true;
            spawnLocationObject = spawn2;
        }
        else if (location == 3)
        {
            spawn3Occupied = true;
            spawnLocationObject = spawn3;
        }
        else { }

        // Create a copy of the Customer Prefab, at the position and rotation of the spawn1 game object
        GameObject customerSpawn = Instantiate(customerPrefab, spawnLocationObject.transform.position, spawnLocationObject.transform.rotation,spawnLocationObject.transform);
        customerSpawn.SetActive(false);

        // Get the customer controller script on the newly created game object
        CustomerController customerController = customerSpawn.GetComponent<CustomerController>();

        // Set the Customer scriptable object on the game object to a random one from our customer pool,
        // and assign the correct spawn location.
        customerController.customer = GetCustomer();
        customerController.spawnLocation = location;
        customerController.order = CreateOrder(customerController.customer.customerType);
        customerController.patienceMax = currMaxPatience;
        customerSpawn.name = customerController.customer.name;

        customerSpawn.SetActive(true);
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
        return customerPool[customerPool.Count-1];
        //return customerPool[Random.Range(0, customerPool.Count)];
    }

    public void Phase1()
    {
        Debug.Log("Starting Phase 1");

        // Our customer pool in phase 1 is entirely normal customers, so we are making a new copy of the normalCustomers list
        customerPool = new List<Customer>(normalCustomers);

        // Set spawn time, order size minimum, and order size maximum to defaults.
        spawnTime = defaultSpawnTime;
        currOrderSizeMax = defaultOrderSizeMax;
        currOrderSizeMin = defaultOrderSizeMin;
        currMaxPatience = defaultMaxPatience;

        spawn1Timer = startSpawnDelay;
        spawn1Occupied = false;
    }

    public void Phase2()
    {
        Debug.Log("Starting Phase 2");

        // Our customer pool in phase 2 is normal AND less creepy customers, so we will add the creepyCustomers list to our current pool created in phase 1
        customerPool.AddRange(creepyCustomers);

        // Multiply current spawn time by 2, now that we have 2 spawns
        spawnTime *= 2f;

        // Change any on screen effects, or music effects
    }

    public void Phase3()
    {
        Debug.Log("Starting Phase 3");

        // Our customer pool in phase 3 is creepy AND cursed customers, with many fewer cute customers,
        // So we will add another copy of the creepyCustomers list, and two copies of the cursed customers list
        customerPool.AddRange(creepyCustomers);
        customerPool.AddRange(cursedCustomers);
        customerPool.AddRange(cursedCustomers);

        // Multiply current spawn time by 1.5, now that we have 3 spawns
        spawnTime *= 1.5f;

        // Change any on screen effects, or music effects
    }

    private List<Ingredient> CreateOrder(int customerType)
    {
        // Clear any previous order info
        order.Clear();

        // Set our working ingredient pool back to default
        List<Ingredient> currIngredientPool = new List<Ingredient>(ingredientPool);

        // Add all required items to the order.
        order.AddRange(requiredIngredients);
        orderSize = Random.Range(currOrderSizeMin, currOrderSizeMax);

        // If customer is creepy type...
        if (customerType == 2)
        {
            // Increase ordersize by 1.25
            orderSize = Mathf.Floor(orderSize * 1.25f);

        }
        // If customer is cursed type...
        else if (customerType == 3)
        {
            // Increase ordersize by 1.5
            orderSize = Mathf.Floor(orderSize * 1.5f);
            // Add cursed ingredients to the ingredient pool.
            currIngredientPool.AddRange(cursedIngredients);
        }
        else { }

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

    public void IncreaseOrderSize()
    {
        // Increase the minimum order size by the listed amount, but increase the maximum by the listed amount times a factor of 1.5
        currOrderSizeMin += orderSizeIncrease;
        currOrderSizeMax += Mathf.Floor(orderSizeIncrease*1.5f);
    }

    public void DecreaseTimers()
    {
        // If our spawn time is greater than 1, recalculate using our number of orders completed, and softening by number of orders failed
        if (spawnTime > 1)
        {
            spawnTime -= spawnTimeDecrease * difficultyManager.ordersCompleted / (difficultyManager.ordersFailed+1);
        }
        else { }

        // If we go below a 1 second spawn timer after the above, reset it back to 1.
        if (spawnTime < 1)
        {
            spawnTime = 1;
        }
        else { }

        // If our maximum patience is greater than the minimum, recalculate using our number of orders completed, and softening by number of orders failed
        if (currMaxPatience > minPatience)
        {
            currMaxPatience -= patienceDecrease * difficultyManager.ordersCompleted / (difficultyManager.ordersFailed + 1);
        }

        // If we go below our minimum patience after the above, reset it back to the minimum.
        if (currMaxPatience < minPatience)
        {
            currMaxPatience = minPatience;
        }
        else { }
    }
}
