using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // This manager script will count our completed/failed orders, and hold information on difficulty phases.

    [Header("Order Counters")]
    // How many orders has the player successfully completed?
    public float ordersCompleted = 0f;
    // How many orders has the player failed?
    public float ordersFailed = 0f;
    // Our difficulty modifier, which we will use to calculate customer patience / spawn rates and track difficulty phases.
    public float difficultyModifier = 1f;
    
    // We will keep a separate counter of completed orders, to track our progress to the increase in difficulty modifier.
    float ordersPerLevelCounter = 0f;

    [Header("Difficulty Phase Settings")]
    // How many orders must be completed to increase the difficulty modifier
    public float ordersPerLevel = 5f;
    // At which difficulty level will each new phase start?
    // This will affect changes to our game like increased order length, creepier customers, and on screen effects
    public float difficultyPhase2 = 1f;
    public float difficultyPhase3 = 2f;

    // Our tracker for which phase of difficulty we're on.
    // We're hiding this in the inspector to keep us from editing it directly during play mode.
    [HideInInspector]
    public float difficultyPhaseCounter = 1;

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

            // If our difficulty modifier hits the threshold for a new phase...
            if (difficultyModifier == difficultyPhase2 | difficultyModifier == difficultyPhase3)
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
        }
        else { }
    }

    // We will call this from our customer game objects when an order is failed.
    public void FailOrder()
    {
        // Increase our failed order counter by one.
        ordersFailed++;
        Debug.Log("Orders failed: " + ordersFailed);
    }
}
