using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // This manager script will count our completed/failed orders, and hold information on difficulty phases.

    [Header("Order Counters")]
    // How many orders has the player successfully completed?
    public int ordersCompleted = 0;
    // How many orders has the player failed?
    public int ordersFailed = 0;
    // Our difficulty modifier, which we will use to calculate customer patience / spawn rates and track difficulty phases.
    public int difficultyModifier = 1;

    // We will keep a separate counter of completed orders, to track our progress to the increase in difficulty modifier.
    int ordersPerLevelCounter = 0;

    [Header("Difficulty Phase Settings")]
    // How many orders must be completed to increase the difficulty modifier
    public int ordersPerLevel = 5;
    // At which difficulty level will each new phase start?
    // This will affect changes to our game like increased order length, creepier customers, and on screen effects
    [Header("Additive (Phase 3 will start x levels after Phase 2)")]
    public int difficultyPhase2;
    public int difficultyPhase3;

    // Our tracker for which phase of difficulty we're on.
    // We're hiding this in the inspector to keep us from editing it directly during play mode.
    //[HideInInspector]
    public int difficultyPhaseCounter = 1;

    private CustomerManager customerManager;

    private void Start()
    {
        customerManager = GetComponent<CustomerManager>();
    }

    // We will call this from our customer game objects when an order is completed.
    public void CompleteOrder()
    {
        // Increase our completed order counters by one.
        ordersCompleted++;
        ordersPerLevelCounter++;

        Debug.Log("Orders completed: " + ordersCompleted);

        // If our per-level counter hits the threshold...
        if (ordersPerLevelCounter == ordersPerLevel)
        {
            // Increase the difficulty modifier by one
            difficultyModifier++;
            Debug.Log("Increase difficulty modifier");

            // Calculate changes to minimum/maximum order size after every difficulty modifier increase.
            customerManager.IncreaseOrderSize();

            // If our difficulty modifier hits the threshold for a new phase...
            if (difficultyModifier == (1 + difficultyPhase2) || difficultyModifier == (1+difficultyPhase2+difficultyPhase3))
            {
                // Increase the difficulty phase by one
                difficultyPhaseCounter++;

                // Check which phase we're on, and tell the Customer Manager
                if (difficultyPhaseCounter == 2)
                {
                    customerManager.Phase2();
                }else if (difficultyPhaseCounter == 3)
                {
                    customerManager.Phase3();
                }
            }
            // When we're done, make sure to set our per-level counter back to zero.
            ordersPerLevelCounter = 0;
            Debug.Log("Reset ordersPerLevelCounter to zero");
        }
        else { }

        // Calculate changes to the customer respawn timer and patience timer after every completed order.
        customerManager.DecreaseTimers();
    }

    // We will call this from our customer game objects when an order is failed.
    public void FailOrder()
    {
        // Increase our failed order counter by one.
        ordersFailed++;
        Debug.Log("Orders failed: " + ordersFailed);
    }
}
