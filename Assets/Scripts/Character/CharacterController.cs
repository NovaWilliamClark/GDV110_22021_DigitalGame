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
using UnityEngine.SceneManagement;
using Core.LitArea;
using Objects;
using UnityEngine.Rendering.UI;

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
    [SerializeField] public Animator animator = null;
    [SerializeField] private Transform puppet = null;
    //[SerializeField] private CharacterAudio audioPlayer = null;

    [Header("Data")] 
    [SerializeField] private PlayerData_SO playerData;

    [Header("Sanity")]
    [SerializeField] private float sanityLossRate = 0.5f;
    [SerializeField] private float sanityGainRate = 0.25f;
    private bool isInLight = false;
    private bool allowLightInteraction = true;

    //[SerializeField] private Inventory inventory;
    public Inventory GetInventory => inventory;
    private Inventory inventory;
    public float getSanity { get; private set; } = 100f;

    [Header("Movement")]
    [SerializeField] private float acceleration = 30.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float minFlipSpeed = 0.1f;
    

    private Rigidbody2D controllerRigidBody;
    private Collider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;
    private GroundType groundType;

    private Vector2 movementInput;
    private Vector2 prevVelocity;
    private bool isFlipped;
    private bool meleeAttack;

    private int animatorMoveSpeed;
    public UnityEvent OnDeath;
    private MeleeWeapon _meleeWeapon;

    private bool CanMove { get; set; }

    private void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
        inventory.gameObject.SetActive(false);
        FetchPersistentData();
        
        controllerRigidBody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        softGroundMask = LayerMask.GetMask("Ground Soft");
        hardGroundMask = LayerMask.GetMask("Ground Hard");

        _meleeWeapon = GetComponentInChildren<MeleeWeapon>();
    
        animatorMoveSpeed = Animator.StringToHash("MoveSpeed");
    
        CanMove = true;
        //controllerCollider.isTrigger = true;
        LitArea.onLightEnter += Light_OnLightEnter; 
        LitArea.onLightExit += Light_OnLightExit;
    }
    
    private void Update()
    {
        var keyboard = Keyboard.current;

        if (!CanMove || keyboard == null)
        {
            return;
        }

        // Horizontal Movement
        float moveHorizontal = 0.0f;

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

        //Interaction
        if (Input.GetButtonDown("Inventory"))
        {
            ShowInventory();
        }
        
        // Roll Dodge


        // Sanity
        if (allowLightInteraction)
        {
            if (!isInLight)
            {
                TakeSanityDamage(sanityLossRate, true);
            }
            else
            {
                getSanity += sanityGainRate;
            }
            
            if (getSanity >= 100)
            {
                getSanity = 100f;
            }
        }
        
        UpdateDirection();
        CheckMeleeInput();
    }

    private void FixedUpdate()
    {
        UpdateGrounding();
        UpdateVelocity();
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
            animator.SetFloat(animatorMoveSpeed, horizontalSpeedNormalized);
        }

        // Play Audio
        // audioPlayer.PlaySteps(groundType, horizontalSpeedNormalized);
    }

    private void UpdateDirection()
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

    private void Light_OnLightEnter(Collider2D collider)
    {
        if (collider != controllerCollider)
        {
            Debug.Log("Not player collider");
        }
        isInLight = true; 
        Debug.Log("Light Entered");
    }

    private void Light_OnLightExit(Collider2D collider)
    {
        if (collider != controllerCollider)
        {
            Debug.Log("Not player collider");
        }
        isInLight = false; 
        Debug.Log("Light Exited");
    }
    private void OnDestroy()
    {
        LitArea.onLightEnter -= Light_OnLightEnter;
        LitArea.onLightExit -= Light_OnLightExit;
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
    
    public void TakeSanityDamage(float damageTaken, bool fromDarkness)
    {
        if (fromDarkness)
        {
            getSanity -= damageTaken;
            if (getSanity <= 0)
            {
                getSanity = 1;
            }
        }
        else
        {
            getSanity -= damageTaken;
            if (getSanity <= 0)
            {
                getSanity = 0;
                allowLightInteraction = false; //Stop processing sanity light interaction 
                CanMove = false;
                // Play Death Animation
                StartCoroutine(RestartLevel());
            }
        }
    }
    
    private void CheckMeleeInput()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            meleeAttack = true;
        }
        else
        {
            meleeAttack = false;
        }
            
        if (meleeAttack && IsGrounded())
        {
            _meleeWeapon.Attack();
        }
    }

    private IEnumerator RestartLevel()
    {
        OnDeath.Invoke();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Prototype_BensBedroom");
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
        getSanity = playerData.sanity;
        sanityGainRate = playerData.sanityGainRate;
        sanityLossRate = playerData.sanityLossRate;
        inventory.Init(playerData.inventoryItems);
    }
    public void SetPersistentData()
    {
        playerData.sanity = getSanity;
        playerData.sanityGainRate = sanityGainRate;
        playerData.sanityLossRate = sanityLossRate;
        //playerData.inventoryItems.Clear();
        playerData.SetItems(inventory);
    }
    
    private void ShowInventory()
    {
        GetInventory.gameObject.SetActive(true);
    }
    public void AddToInventory(Item item)
    {
        GetInventory.AddToInventory(item);
    }
}