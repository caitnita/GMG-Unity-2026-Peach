using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Playground/Movement/Move With Arrows")]
[RequireComponent(typeof(Rigidbody2D))]
public class Move : Physics2DObject
{
    [Header("Input keys")]
    public Enums.KeyGroups typeOfControl = Enums.KeyGroups.ArrowKeys;

    [Header("Movement")]
    [Tooltip("Speed of movement")]
    public float speed = 5f;
    public float maxSpeed;
    public Enums.MovementType movementType = Enums.MovementType.AllDirections;

    [Header("Orientation")]
    public bool orientToDirection = false;
    // The direction that will face the player
    public Enums.Directions lookAxis = Enums.Directions.Up;

    private Vector3 movement, cachedDirection;
    private float moveHorizontal;
    private float moveVertical;

    // A GameObject to store our interactBox
    private GameObject interactBox;

    //Animations
    public Animator animator;

    //Physics
    public Rigidbody2D body;

    private void Start()
    {
        // Get our components for our Animator and Rigidbody2D by finding the ones attached to the same gameobject as this script.
        animator = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();

		interactBox = this.gameObject.transform.GetChild(0).gameObject;

        // Set our maximum speed to our inputed Speed value, since we're not using any complex logic.
		maxSpeed = speed;

        // Our player starts facing downwards, so we'll set the rotation of the interactbox to match
        interactBox.transform.localRotation = Quaternion.Euler(0, 0, 270f);
    }
    // Update gets called every frame
    void Update ()
	{
        //#if UNITY_STANDALONE || !UNITY_EDITOR
        // Moving with the arrow keys
        if(typeOfControl == Enums.KeyGroups.ArrowKeys)
		{
			moveHorizontal = Input.GetAxisRaw("Horizontal");
			moveVertical = Input.GetAxisRaw("Vertical");
			//Debug.Log(moveHorizontal + " , " + moveVertical);
		}
		else if (typeOfControl == Enums.KeyGroups.WASD)
		{
			moveHorizontal = Input.GetAxisRaw("Horizontal2");
			moveVertical = Input.GetAxisRaw("Vertical2");
		}
        //#endif
    
        //zero-out the axes that are not needed, if the movement is constrained
        switch(movementType)
		{
			case Enums.MovementType.OnlyHorizontal:
				moveVertical = 0f;
				break;
			case Enums.MovementType.OnlyVertical:
				moveHorizontal = 0f;
				break;
		}
		
        // Set our movement vector to contain both our horizontal and vertical movement
        // (We can access either separately by using movement.x and movement.y)
		movement = new Vector3(moveHorizontal, moveVertical);

        // Every frame, set the animator input values to match our movement values.
        animator.SetFloat("inputX", movement.x);
        animator.SetFloat("inputY", movement.y);

        // If the player is NOT moving...
		if (movement.magnitude == 0)
		{
            // Set the "LastInput" values on the animator to the currently cached direction.
			animator.SetFloat("lastInputX", cachedDirection.x);
			animator.SetFloat("lastInputY", cachedDirection.y);

            // Set the "isWalking" value on the animator to false.
			animator.SetBool("isWalking", false);
		}
		else {
            // Else (if the player IS moving...)

            // Set the cached directions to our current movement values for later.
            cachedDirection.x = movement.x;
            cachedDirection.y = movement.y;

            // If the animator was previously not walking, start walking!
            if (animator.GetBool("isWalking") == false)
			{
				animator.SetBool("isWalking", true);
            }

            // We'll use the below logic to control the direction of our InteractBox object.
            // We will be rotating it around its pivot point, so make sure the pivot is centered on the Player object.
            // We're going to prioritize the horizontal directions (left/right) over vertical (up/down),
            // so that if we're moving diagonally, the InteractBox will go to a horizontal direction.

            // If our X movement is greater than zero (we're moving right)...
			if (movement.x > 0f)
			{
                // Set the local rotation of the interact box to 0,0,0 (default)
				interactBox.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // Else, if our X movement is less than zero (we're moving left)...
			}else if(movement.x < 0f)
			{
                // Set the local rotation of the interact box to 0,0,180 (rotate 180 degrees around the Z-axis, so the interact box is towards the left)
                interactBox.transform.localRotation = Quaternion.Euler(0, 0, 180f);

            // Else (our X movement must be zero), if our Y movement is greater than zero (we're moving up)...
            }else if (movement.y > 0f)
			{
                // Set the local rotation of the interact box to 0,0,90 (rotate 90 degrees around the Z-axis, so the interact box is towards the top)
                interactBox.transform.localRotation = Quaternion.Euler(0, 0, 90f);

             // Else, if our Y movement is less than zero (we're moving down)...
            }else if (movement.y < 0f)
            {
                // Set the local rotation of the interact box to 0,0,270 (rotate 270 degrees around the Z-axis, so the interact box is towards the bottom)
                interactBox.transform.localRotation = Quaternion.Euler(0, 0, 270f);
            }
			else{ }
        }
	}

	// FixedUpdate is called every frame when the physics are calculated
	void FixedUpdate ()
	{
        // Apply the force to the Rigidbody2d
        //rigidbody2D.AddForce(movement * speed * 10f);
        //transform.position += movement * Time.deltaTime * speed * 10f;

        // Define the velocity of the player object as player input, multiplied by our speed value.
        body.linearVelocity = movement * speed;
        body.linearVelocity = Vector2.ClampMagnitude(movement * speed, maxSpeed);
    }
}