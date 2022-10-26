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
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using AudioType = UnityEngine.AudioType;

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
    [SerializeField] private Transform puppet = null;
    //[SerializeField] private CharacterAudio audioPlayer = null;

    [Header("Data")] 
    [SerializeField] private PlayerData_SO playerData;

    [Header("Sanity")]
    [SerializeField] private float sanityLossRate = 0.5f;
    [SerializeField] private float sanityGainRate = 0.25f;
    private bool isInLight = false;
    private float sanity = 100f;
    private bool allowLightInteraction = true;
    public float getSanity => sanity;

    [Header("Movement")]
    [SerializeField] private float acceleration = 30.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float minFlipSpeed = 0.1f;
    private Vector2 movementVelocity;
    private float moveHorizontal;
    
    private Rigidbody2D controllerRigidBody;
    private Collider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;
    private GroundType groundType;

    private Vector2 movementInput;
    private Vector2 prevVelocity;
    private bool isFlipped;
    
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
        controllerRigidBody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        softGroundMask = LayerMask.GetMask("Ground Soft");
        hardGroundMask = LayerMask.GetMask("Ground Hard");

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
                sanity += sanityGainRate;
            }
            
            if (sanity >= 100)
            {
                sanity = 100f;
            }
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
            sanity -= damageTaken;
            if (sanity <= 0.001f)
            {
                sanity = 0.001f;
            }
        }
        else
        {
            sanity -= damageTaken;
            if (sanity <= 0)
            {
                sanity = 0;
                allowLightInteraction = false; //Stop processing sanity light interaction 
                CanMove = false;
                // Play Death Animation
                StartCoroutine(RestartLevel());
            }
        }
    }
    
    private IEnumerator RestartLevel()
    {
        onDeath.Invoke();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Prototype_BensBedroom");
    }

    public void FetchPersistentData()
    {
        sanity = playerData.sanity;
        sanityGainRate = playerData.sanityGainRate;
        sanityLossRate = playerData.sanityLossRate;
    }
    public void SetPersistentData()
    {
        playerData.sanity = sanity;
        playerData.sanityGainRate = sanityGainRate;
        playerData.sanityLossRate = sanityLossRate;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        movableObjScript = other.gameObject.GetComponent<MovableObject>();
        if (!movableObjScript)
        {return;}
        
        objToMove = other.gameObject;
        canMoveObject = true;
        objToMove.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        Debug.Log("Can Move Object");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
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
            isMovingObject = true;
            // Slow PLayer Movement 
            acceleration = movableObjScript.moveVelocity;
            Debug.Log("Velocity Updated:" + movableObjScript.moveVelocity);
        
            //Move Object   
            objToMove.transform.Translate(movementVelocity.x / 50, 0, 0);
        }
        else
        {
            isMovingObject = false;
            acceleration = 30.0f;
        }
    }
}