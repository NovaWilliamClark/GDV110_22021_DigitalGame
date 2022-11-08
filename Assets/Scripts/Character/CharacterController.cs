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
using Audio;
using Character;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Core.LitArea;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Objects;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using AudioType = UnityEngine.AudioType;
using UnityEngine.Rendering.UI;
using UnityEngine.U2D.IK;
using Random = Unity.Mathematics.Random;

public enum GroundType
{
    None,
    Soft,
    Hard
}

public class CharacterController : MonoBehaviour
{
    public static event Action<bool> OnObjectMove;
    private readonly Vector3 flippedScale = new Vector3(-1, 1, 1);

    [Header("Character")]
    [SerializeField] public Animator animator = null;
    [SerializeField] private Transform puppet = null;
    //[SerializeField] private CharacterAudio audioPlayer = null;
    [SerializeField] private Inventory inventory;
    [SerializeField] private CharacterEquipment equipment;

    [Header("Data")] 
    [SerializeField] private PlayerData_SO playerData;

    //[SerializeField] private Inventory inventory;
    public Inventory GetInventory => inventory;
    public CharacterEquipment Equipment => equipment;
    public float getSanity { get; private set; } = 100f;
    public event EventHandler<float> SanityChanged; 

    [Header("Movement")]
    [SerializeField] private float acceleration = 30.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float minFlipSpeed = 0.1f;
    private float moveHorizontal;
    private Vector2 movementVelocity;
    
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

    [Header("Animation")]
    private int animatorMoveSpeed;
    [FormerlySerializedAs("OnDeath")] public UnityEvent onDeath;

    [Header("Push & Pull")]
    private bool canMoveObject;
    private bool isMovingObject;
    private float movementAcceleration;
    private GameObject objToMove;
    private MovableObject movableObjScript;

    private bool CanMove { get; set; }

    private void Start()
    {
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

        characterSanity = this.GetComponent<CharacterSanity>();
    
        CanMove = true;
    }
    
    private void Update()
    {
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

        //Interaction
        if (Input.GetButtonDown("Inventory"))
        {
            ShowInventory();
        }
        


        UpdateDirection();
    }

    private void FixedUpdate()
    {
        MoveObject();
        UpdateGrounding();
        UpdateVelocity();
    }
    
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
    }
    
    private void ShowInventory()
    {
        GetInventory.OpenInventory();
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
        movableObjScript = other.gameObject.GetComponent<MovableObject>();
        if (!movableObjScript) return;
        
        objToMove = other.gameObject;
        canMoveObject = true;
        objToMove.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        Debug.Log("Can Move Object");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // TODO: The controlling of rigidbody values on another object should be moved to the object itself
        // TODO: BUG - Box should be declared kinematic when on top - box should store it's state
        if (!objToMove) return;
        objToMove.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        canMoveObject = false;
        objToMove = null;
        movableObjScript = null;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void MoveObject()
    {
        if (canMoveObject & Input.GetKey(KeyCode.Mouse1))
        {
            OnObjectMove?.Invoke(true);
            isMovingObject = true;
            // Slow PLayer Movement 
            acceleration = movableObjScript.moveVelocity;
            Debug.Log("Velocity Updated:" + movableObjScript.moveVelocity);
        
            //Move Object   
            objToMove.transform.Translate(movementVelocity.x / 50, 0, 0);
        } else if (Input.GetKeyUp(KeyCode.Space))
        {
            isMovingObject = false;
            Debug.Log("Pressed space");
            var col = objToMove.gameObject.GetComponent<Collider2D>();
            // teleport to above the box
            var top = col.bounds.center;
            top.y = col.bounds.max.y;
            top.y -= GetComponent<Collider2D>().bounds.center.y;

            transform.position = top;
        }
        else
        {
            OnObjectMove?.Invoke(false);
            isMovingObject = false;
            //acceleration = 30.0f;
        }
    }

    public void ToggleMovement(bool value)
    {
        CanMove = value == true ? true : false;
    }
}