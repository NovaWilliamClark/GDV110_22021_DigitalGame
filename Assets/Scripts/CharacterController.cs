

/*******************************************************************************************
*
*    File: CharacterController.cs
*    Purpose: Character Controller Movement and Interaction
*    Author: William Clark
*    Date: 10/10/2022
*
**********************************************************************************************/

using UnityEngine;
//using UnityEngine.InputSystem;

public enum GroundType
{
    None,
    Soft,
    Hard
}

public class CharacterController : MonoBehaviour
{
    private readonly Vector3 flippedScale = new Vector3(-1, 1, 1);
    private readonly Quaternion flippedRotation = new Quaternion(0, 0, 1, 0);

    [Header("Character")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private Transform puppet = null;
    //[SerializeField] private CharacterAudio audioPlayer = null;

    [Header("Sanity")]
    [SerializeField] private float sanityLossRate = 0.01f;
    private bool isInLight = false;
    private float sanity = 100f;
    public float getSanity => sanity;

    [Header("Movement")]
    [SerializeField] private float acceleration = 30.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float minFlipSpeed = 0.1f;
    
    private Rigidbody2D controllerRigidBody;
    private Collider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;

    private Vector2 movementInput;
    private Vector2 prevVelocity;
    private GroundType groundType;
    private bool isFlipped;

    private int animatorRunningSpeed;

    private bool CanMove { get; set; }

    private void Start()
    {
        
        controllerRigidBody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        softGroundMask = LayerMask.GetMask("Ground Soft");
        hardGroundMask = LayerMask.GetMask("Ground Hard");
    
        animatorRunningSpeed = Animator.StringToHash("RunningSpeed");
    
        CanMove = true;
        controllerCollider.isTrigger = true;
        Light.onLightEnter += Light_OnLightEnter; 
        Light.onLightExit += Light_OnLightExit;
    }
    
    private void Update()
    {
        //var keyboard = Keyboard.current;

        if (!CanMove)
        {
            return;
        }
        
        // Horizontal Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        
        //if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
        //{
            //moveHorizontal = -1.0f;
        //}
        //else if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
        //{
            //moveHorizontal = 1.0f;
        //}

        movementInput = new Vector2(moveHorizontal, 0);// * Time.deltaTime * maxSpeed;
        
        
        // Roll Dodge
        
        // Sanity
        if (!isInLight)
        {
            sanity -= sanityLossRate;
        }

        if (sanity <= 1f)
        {
            sanity = 1f;
        }
    }

    private void FixedUpdate()
    {
        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
    }

    
    // 
    private void UpdateGrounding()
    {
        // Use Character Collider to check if touching ground layers
        if (controllerCollider.IsTouchingLayers(softGroundMask))
        {
            groundType = GroundType.Soft;
        }
        else if (controllerCollider.IsTouchingLayers(hardGroundMask))
        {
            groundType = GroundType.Hard;
        }
        else groundType = GroundType.None;
        
    }

    private void UpdateVelocity()
    {
        Vector2 velocity = controllerRigidBody.velocity;
        
        // Apply acceleration directly because we will clamp prior to assigning back to the body
        velocity += movementInput * acceleration * Time.fixedDeltaTime;
        
        // We've consumed the movement, reset it.
        movementInput = Vector2.zero;
        
        // Clamp horizontal speed
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        controllerRigidBody.velocity = velocity;
        
        // Update Animator
        if (animator)
        {
            var horizontalSpeedNormalized = Mathf.Abs(velocity.x) / maxSpeed;
            animator.SetFloat(animatorRunningSpeed, horizontalSpeedNormalized);
        }

        // Play Audio
        // audioPlayer.PlaySteps(groundType, horizontalSpeedNormalized);
    }

    private void UpdateDirection()
    {
        if (controllerRigidBody.velocity.x > minFlipSpeed && isFlipped)
        {
            isFlipped = false;
            if (puppet)
            {
                puppet.localScale = Vector3.one;
            }
        }
        else if (controllerRigidBody.velocity.x < -minFlipSpeed && !isFlipped)
        {
            isFlipped = true;
            if (puppet)
            {
                puppet.localScale = flippedScale;
            }
        }
    }

    private void Light_OnLightEnter(Collider2D collider)
    {
        isInLight = true; 
        Debug.Log("Light Entered");
    }

    private void Light_OnLightExit(Collider2D collider)
    {
        isInLight = false; 
        Debug.Log("Light Exited");
    }
    private void OnDestroy()
    {
        Light.onLightEnter -= Light_OnLightEnter;
        Light.onLightExit -= Light_OnLightExit;
    }
}