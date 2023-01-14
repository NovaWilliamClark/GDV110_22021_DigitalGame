/*******************************************************************************************
*
*    File: CharacterController.cs
*    Purpose: Character Controller Movement and Interaction
*    Author: William Clark
*    Date: 10/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using Character;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Objects;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum GroundType
{
    None,
    Soft,
    Hard
}

public class CharacterController : MonoBehaviour
{
    private readonly Vector3 flippedScale = new Vector3(-1, 1, 1);

    [Header("Character")]
    [SerializeField] public Animator animator = null;

    public Animator expressionAnimator;
    [SerializeField] private Transform puppet = null;
    //[SerializeField] private CharacterAudio audioPlayer = null;
    [SerializeField] public Inventory inventory;
    [SerializeField] private CharacterEquipment equipment;

    [Header("Data")] 
    private PlayerData_SO playerData;

    public PlayerData_SO PlayerData => playerData;

    //[SerializeField] private Inventory inventory;
    public Inventory GetInventory => inventory;
    public CharacterSanity GetCharacterSanity => characterSanity;
    public CharacterEquipment Equipment => equipment;
    public float getSanity { get; private set; } = 100f;
    public event EventHandler<float> SanityChanged; 

    [Header("Movement")]
    [SerializeField] public float acceleration = 30.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float minFlipSpeed = 0.1f;
    private float moveHorizontal;
    public Vector2 movementVelocity;
    
    private Rigidbody2D controllerRigidBody;
    private Collider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;
    private GroundType groundType;

    private Vector2 movementInput;
    private Vector2 prevVelocity;
    private bool isFlipped;
    
    [Header("Flashlight")]
    private CharacterSanity characterSanity;
    private PlayerInput input;
    public PlayerInput Input => input;

    [Header("Animation")]
    private int animatorMoveSpeed;
    [FormerlySerializedAs("OnDeath")] public UnityEvent onDeath;

    [Header("Push & Pull")]
    public bool isMovingObject;

    public MovableObject movableObject;
    private float movementAcceleration;
    private bool flashlightWasOn;
    private bool falling;
    public bool IsFalling => falling;

    private bool CanMove { get; set; }

    public void Init(PlayerData_SO spawnPlayerData)
    {
        playerData = spawnPlayerData;
        FetchPersistentData();
        //inventory = GetComponentInChildren<Inventory>();
        inventory.gameObject.SetActive(true);
        controllerRigidBody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        softGroundMask = LayerMask.GetMask("Ground Soft");
        hardGroundMask = LayerMask.GetMask("Ground Hard");

        animatorMoveSpeed = Animator.StringToHash("MoveSpeed");

        input = new PlayerInput();
        input.Enable();
        input.Player.OpenInventory.performed += OnInventoryInput;

        characterSanity = this.GetComponent<CharacterSanity>();
        characterSanity.SanityReachedZero.AddListener(PerformDeath); 
    
        CanMove = true;
    }

    private void OnInventoryInput(InputAction.CallbackContext obj)
    {
        if (playerData.equipmentState.hasBag)
        {
            if (!inventory.IsOpen)
            {
                inventory.OpenInventory();
            }
            else
            {
                inventory.CloseInventory();
            }
        }
    }

    private void Update()
    {
        animator.SetBool("IsPushing", isMovingObject);
        var keyboard = Keyboard.current;

        if (!CanMove || keyboard == null)
        {
            return;
        }

        // Horizontal Movement
        moveHorizontal = 0.0f;

        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
        {
            isFlipped = true;
            moveHorizontal = -1.0f;
        }
        else if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
        {
            isFlipped = false;
            moveHorizontal = 1.0f;
        }

        movementInput = new Vector2(moveHorizontal, 0);
        
        UpdateDirection();
    }

    private void FixedUpdate()
    {
        //MoveObject();
        UpdateGrounding();
        UpdateVelocity();
    }
    
    private void UpdateGrounding()
    {
        animator.SetFloat("Velocity_Y", Mathf.Abs(controllerRigidBody.velocity.y));
        falling = Mathf.Abs(controllerRigidBody.velocity.y) > 0.5f;
    }

    private void UpdateVelocity()
    {
        movementVelocity = controllerRigidBody.velocity;
        
        // Apply acceleration directly because we will clamp prior to assigning back to the body
        movementVelocity += (movementInput * (acceleration * Time.fixedDeltaTime));
        
        // We've consumed the movement, reset it.
        movementInput = Vector2.zero;

        // Clamp horizontal speed
        movementVelocity.x = Mathf.Clamp(movementVelocity.x, -maxSpeed, maxSpeed);
        
        controllerRigidBody.velocity = movementVelocity;
        
        // Update Animator
        if (animator)
        {
            var horizontalSpeedNormalized = Mathf.Abs(movementVelocity.x) / maxSpeed;
            animator.SetFloat(animatorMoveSpeed, horizontalSpeedNormalized);
        }

        // Play Audio
        // audioPlayer.PlaySteps(groundType, horizontalSpeedNormalized);
    }

    private void UpdateDirection()
    {
        if (!isMovingObject)
        {
            if (controllerRigidBody.velocity.x > minFlipSpeed && !isFlipped)
            {
                if (puppet)
                {
                    puppet.localScale = Vector2.one;
                }
            }
            else if (controllerRigidBody.velocity.x < -minFlipSpeed && isFlipped)
            {
           
                if (puppet)
                {
                    puppet.localScale = flippedScale;
                }
            }   
        }
    }

    public bool IsGrounded()
    {
        if (groundType == GroundType.Hard || groundType == GroundType.Soft)
        {
            return true;
        }
        
        return false;
    }

    public bool IsFacingLeft()
    {
        return isFlipped;
    }

    private void PerformDeath()
    {
        // play the cinematic and die
        animator.SetTrigger("Death");
        ToggleActive(false);
        onDeath.Invoke();
    }
    
    private IEnumerator RestartLevel()
    {
        onDeath.Invoke();
        
        yield return new WaitForSeconds(5f);
        TransitionManager.Instance.LoadScene("Prototype_BensBedroom");
    }

    public void SetIsFlipped(bool value)
    {
        if (value == true)
        {
            puppet.localScale = flippedScale;
            isFlipped = true;
        }
        else
        {
            puppet.localScale = Vector2.one;
            isFlipped = false;
        }
    }

    private void FetchPersistentData()
    {
        //getSanity = playerData.sanity;
        //sanityGainRate = playerData.sanityGainRate;
        //sanityLossRate = playerData.sanityLossRate;
        inventory.Init(playerData.inventoryItems);
        equipment.Init(playerData);
    }
    public void SetPersistentData()
    {
        //playerData.sanity = getSanity;
        //playerData.sanityGainRate = sanityGainRate;
        //playerData.sanityLossRate = sanityLossRate;
        //playerData.inventoryItems.Clear();
        playerData.SetItems(inventory);
        equipment.ToggleFlashlight(false);
    }

    public void AddToInventory(ItemData itemData)
    {
        GetInventory.AddToInventory(itemData);
    }

    public void SetAnimationControl(bool disabled = false)
    {
        // quick hack so Timeline director has full control of animations
        animator.SetBool("Enabled", !disabled);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Enemies")) return;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Enemies")) return;
        // TODO: The controlling of rigidbody values on another object should be moved to the object itself
        // TODO: BUG - Box should be declared kinematic when on top - box should store it's state
    }

    public void ToggleActive(bool value)
    {
        CanMove = value == true ? true : false;
        playerData.flashlightAvailable = value == true ? true : false;
        if (equipment)
        {

            if (value)
            {
                equipment.EnableInput();
            }
            else
            {
                equipment.DisableInput();
            }
        }
        if (characterSanity)
            characterSanity.inventoryClosed = value == true ? true : false;
    }
}