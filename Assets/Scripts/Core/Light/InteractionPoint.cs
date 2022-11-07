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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using Sequence = DG.Tweening.Sequence;

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
    private CharacterController playerRef;

    [Header("Visuals")]
    public Color outlineColour = new Color(78f, 93f, 111f, 1f);
    protected SpriteRenderer renderer;
    protected SpriteRenderer glowRenderer;
    public Material glowMaterial;
    protected bool tweening;
    private bool outlineActive;
    private Sequence visualSequence;
    [SerializeField] protected float fxRange = 7.5f;
    
    protected PlayerInput input;

     protected virtual void Awake()
     {
         triggerArea = GetComponent<BoxCollider2D>();
         
         renderer = GetComponent<SpriteRenderer>();
         if (renderer)
         {
             var rend = new GameObject("Sprite Outline");
             rend.transform.localScale = transform.localScale;
             rend.transform.position = transform.position;
             rend.transform.parent = transform;
             glowRenderer = rend.AddComponent<SpriteRenderer>();
             glowRenderer.sprite = renderer.sprite;
             glowRenderer.color = renderer.color;
             glowRenderer.sortingLayerName = renderer.sortingLayerName;
             glowRenderer.sortingOrder = renderer.sortingOrder - 1;
             glowRenderer.material = glowMaterial;
             glowRenderer.color = outlineColour;
             glowRenderer.DOFade(0f, 0f);
             
             glowRenderer.material.SetColor("_Outline_Colour", outlineColour);
             glowRenderer.material.SetFloat("_varTime", 0f);
             
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
            if (hasInteracted) return;
            Debug.Log("Auto Interaction!");
            hasInteracted = true;
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
        
        if (!canInteract || !playerInRange)
        {
            if (!tweening && outlineActive)
            {
                tweening = true;
                visualSequence.OnPlay(() =>
                {
                    tweening = false;
                    outlineActive = false;
                });
                visualSequence.PlayBackwards();
            }
        }
        if (hasInteracted) return;
        if (playerInRange)
        {
            if (!tweening && !outlineActive)
            {
                tweening = true;
                visualSequence = DOTween.Sequence();
                visualSequence
                    .Insert(0, glowRenderer.material.DOFloat(1f, "_varTime", .5f))
                    .Insert(0, glowRenderer.DOFade(1f, 0.5f))
                    .OnComplete(() =>
                    {
                        tweening = false;
                        outlineActive = true;
                    })
                    .SetAutoKill(false);
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

    private void OnDrawGizmos()
    {
        DrawGizmoDisc(transform, fxRange);
    }

    protected void DrawGizmoDisc(Transform t, float radius)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = playerInRange ? Color.green : new Color(255f, 255f, 255f, 0.2f);
        Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, new Vector3(1f, 1f, 0.01f));
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
    }
}