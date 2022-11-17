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
using Audio;
using DG.Tweening;
using Objects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;
using Sequence = DG.Tweening.Sequence;

[RequireComponent(typeof(Collider2D), typeof(PersistentObject))]
public abstract class InteractionPoint : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] protected UIPromptBox promptBox;
    [SerializeField] protected string promptMessage;
    [SerializeField] protected bool previewPrompt = true;

    [SerializeField] protected bool automaticInteraction = false;
    private BoxCollider2D triggerArea;
    [SerializeField] protected bool canInteract = true;
    protected bool hasInteracted = false;
    protected bool playerInRange;
    [SerializeField] protected bool canReInteract = false;
    protected CharacterController playerRef;

    [Header("Trigger Requirements")] [SerializeField]
    protected bool requiresItem;

    [SerializeField] protected string missingItemPromptMsg;

    protected bool hasItem;

    [FormerlySerializedAs("requiredImte")] [SerializeField] protected ItemData requiredItem;
    
    [Header("Visuals")]
    [ColorUsage(true,true)]
    public Color outlineColour = new Color(78f, 93f, 111f, 1f);

    [SerializeField] protected bool showVisuals = true;
    [SerializeField] protected SpriteRenderer renderer;
    protected SpriteRenderer glowRenderer;
    public Material glowMaterial;
    protected bool tweening;
    private bool outlineActive;
    private Sequence visualSequence;
    [SerializeField] protected float fxRange = 7.5f;

    [Header("Audio")] [SerializeField] protected AudioClip useSfx;
    [SerializeField] protected float volume;

    protected PersistentObject persistentObject;

    protected PlayerInput input;
    protected LevelController levelController;

    [HideInInspector] public UnityEvent<InteractionPoint,InteractionState> Interacted;

     protected virtual void Awake()
     {
         triggerArea = GetComponent<BoxCollider2D>();
         levelController = FindObjectOfType<LevelController>();
         if (!renderer)
            renderer = GetComponent<SpriteRenderer>();
         if (renderer && showVisuals)
         {
             var rend = new GameObject("Sprite Outline");
             rend.transform.position = renderer.transform.position;
             rend.transform.parent = renderer.transform;
             rend.transform.localScale = Vector3.one;
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

         persistentObject = GetComponent<PersistentObject>();
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
         if ((playerRef && !hasInteracted) || playerRef && canReInteract)
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
         if (!other.CompareTag("Player")) return;
        
         if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            if (!canInteract) return;
            if (!promptBox) return;

            playerRef = other.GetComponent<CharacterController>();
            
            string msg;
            if (requiresItem)
            {
                hasItem = playerRef.GetInventory.HasItem(requiredItem);

                msg = !hasItem ? missingItemPromptMsg : promptMessage;
            }
            else
            {
                msg = promptMessage;
            }
            
            promptBox.gameObject.SetActive(true);
            promptBox.Show(msg);
        }
        else
        {
            if (hasInteracted) return;
            Debug.LogFormat("{0}: Auto Interaction!", name);
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

        if (!showVisuals) return;
        
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
        if (canInteract && playerInRange)
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
                visualSequence.Play();
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
        if (!other.CompareTag("Player")) return;
        if (!automaticInteraction)
        {
            if(!other.GetComponent<CharacterController>()) return;
            playerRef = null;
            DisablePrompt();
        }
    }

    public virtual void SetInteractedState(object state)
    {
        // basically change me to be "active" as if the player has already used me
        
    }

    protected void DisablePrompt()
    {
        if (promptBox && !automaticInteraction)
        {
            promptBox.Hide();
        }
    }

    protected void ResetPrompt(string textToShow)
    {
        if (promptBox)
        {
            var msg = String.IsNullOrEmpty(textToShow) || textToShow == "" ? promptMessage : textToShow;
            promptBox.Show(msg);
        }
    }

    protected virtual void Interact(CharacterController cc)
    {
        if (!ValidateItemRequired(cc))
        {
            return;
        }
        AudioManager.Instance.PlaySound(useSfx, volume);
        Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
    }

    public bool ValidateItemRequired(CharacterController cc)
    {
        if (!requiresItem) return true;
        
        hasItem = cc.GetInventory.HasItem(requiredItem);
        if (!hasItem)
        {
            hasInteracted = false;
            return false;
        }
        AudioManager.Instance.PlaySound(requiredItem.useSfx, requiredItem.useSfxVolume);
        cc.GetInventory.UseItem(requiredItem);
        return true;

    }

    protected virtual void OnDrawGizmos()
    {
        if (showVisuals)
            DrawGizmoDisc(transform, fxRange);
        
        if (!Application.isPlaying)
        {
            if (!promptBox) return;
            if (previewPrompt)
            {
                promptBox.gameObject.SetActive(true);
                promptBox.ShowPreview(promptMessage);
            }
            else
            {
                promptBox.gameObject.SetActive(false);
            }
        }
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