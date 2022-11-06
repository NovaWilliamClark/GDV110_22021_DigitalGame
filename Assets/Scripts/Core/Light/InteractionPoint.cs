/*******************************************************************************************
*
*    File: InteractionPoint.cs
*    Purpose: Abstract base class for object and environment interactions
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/

using System;
using DG.Tweening;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class InteractionPoint : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] protected GameObject promptBox;
    [SerializeField] protected string promptMessage;

    [SerializeField] protected bool automaticInteraction = false;
    private BoxCollider2D triggerArea;
    protected bool canInteract = true;
    protected bool hasInteracted = false;
    protected bool playerInRange;
    [SerializeField] protected float fxRange = 7.5f;
    private CharacterController playerRef;

    public Color outlineColour = new Color(78f, 93f, 111f, 1f);
    protected SpriteRenderer renderer;
    protected bool tweening;
    private bool outlineActive;

    protected PlayerInput input;

     protected virtual void Awake()
     {
         triggerArea = GetComponent<BoxCollider2D>();
         
         renderer = GetComponent<SpriteRenderer>();
         if (renderer)
         {
             renderer.material.SetColor("_Outline_Colour", outlineColour);
             renderer.material.SetFloat("_varTime", 0f);
         }
         if (triggerArea)
         {
             triggerArea.isTrigger = true;
         }
     }

     protected virtual void Start()
     {
         
     }

     protected virtual void OnEnable()
     {
         input = new PlayerInput();
         input.Enable();
         input.Player.Interact.performed += OnInteractInput;
     }
     
     private void OnInteractInput(InputAction.CallbackContext obj)
     {
         if (playerRef && !hasInteracted)
         {
             hasInteracted = true;
             Interact(playerRef);
         }
     }

     protected virtual void OnDisable()
     {
         input.Disable();
         input.Player.Interact.performed -= OnInteractInput;
     }

     protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            if (!canInteract) return;
            if (!promptBox) return;
            
            playerRef = other.GetComponent<CharacterController>();
            
            promptBox.GetComponentInChildren<TMP_Text>().text = promptMessage;
            promptBox.SetActive(true);
        }
        else
        {
            Debug.Log("Auto Interaction!");
            Interact(other.GetComponent<CharacterController>());
        }
        
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            //if (!(Input.GetButton("Interact"))) return;
        }
    }

    protected virtual void Update()
    {
        if (!renderer) return; 
        if (playerInRange)
        {
            if (!tweening && !outlineActive)
            {
                tweening = true;
                renderer.material.DOFloat(1f, "_varTime", .5f).OnComplete(() =>
                {
                    tweening = false;
                    outlineActive = true;
                });
            }
        }
        else
        {
            if (!tweening && outlineActive)
            {
                tweening = true;
                renderer.material.DOFloat(0f, "_varTime", .5f).OnComplete(() =>
                {
                    tweening = false;
                    outlineActive = false;
                });
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!canInteract)
        {
            playerInRange = false;
            return;
        }
        playerInRange = Physics2D.OverlapCircle(transform.position, fxRange, LayerMask.GetMask("Player"));
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if(!other.GetComponent<CharacterController>()) return;
            playerRef = null;
            DisablePrompt();
        }
    }

    // protected void DisableInteraction()
    // {
    //     DisablePrompt();
    //     canInteract = false;
    //     triggerArea.enabled = false;
    // }

    protected void DisablePrompt()
    {
        if (promptBox)
        {
            promptBox.SetActive(false);
        }
    }

    protected abstract void Interact(CharacterController cc);

    private void OnDrawGizmosSelected()
    {
        DrawGizmoDisc(transform,fxRange);
    }

    protected void DrawGizmoDisc(Transform t, float radius)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(255f, 255f, 255f, 0.2f);
        Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, new Vector3(1f, 1f, 0.01f));
        Gizmos.DrawSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
    }
}