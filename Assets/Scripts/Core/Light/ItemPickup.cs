/*******************************************************************************************
*
*    File: ItemPickup.cs
*    Purpose: Inherits from InteractionPoint. Holds an object for player pickup
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Audio;
using DG.Tweening;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemPickup : InteractionPoint
{
    public ItemData GetItemData => itemData;
    [FormerlySerializedAs("item")] [SerializeField] private ItemData itemData;

    [SerializeField] private float fxRadius = 10f;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;
    private float curTime = 0f;
    private bool setActive = false;
    [SerializeField] private float activeDuration = 1f;
    private Sequence materialSequence;
    private List<ParticleSystem> particles;

    private bool matTween;
    private bool matActive;

    [Header("Sound")] [SerializeField] private List<AudioClip> pickupSfx;

    [HideInInspector] public UnityEvent<ItemPickup> ItemPickedUp;

    protected override void Awake()
    {
        base.Awake();
        particles = GetComponentsInChildren<ParticleSystem>().ToList();
    }

    protected override void Start()
    {
        base.Start();

        if (itemData != null)
        {
            if (itemData.itemSprite != null && renderer != null && renderer.sprite == null)
            {
                if (glowRenderer)
                {
                    glowRenderer.sprite = itemData.itemSprite;
                }
                renderer.sprite = itemData.itemSprite;
            }
        }

        foreach (var particle in particles)
        {
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            var random = new System.Random();
            int num = random.Next(Int32.MinValue, Int32.MaxValue);
            particle.randomSeed = (uint) (num + (uint) Int32.MaxValue);
            particle.Play();
        }
    }

    protected override void Update()
    {
        if (!canInteract) return;
        base.Update();

        if (playerInRange)
        {
            if (!matActive)
            {
                renderer.material = activeMaterial;
                matActive = true;
            }
        }

        if (!playerInRange || hasInteracted)
        {
            if (matActive)
            {
                matActive = false;
                renderer.material = inactiveMaterial;
            }
        }
    }

    // protected override void Update()
    // {
    //     if (!canInteract) return;
    //     if (playerInRange)
    //     {
    //         if (!tweening && !setActive)
    //         {
    //             tweening = true;
    //             materialSequence = DOTween.Sequence();
    //             materialSequence
    //                 .Insert(0, glowRenderer.DOFade(1f, activeDuration))
    //                 .Insert(0, renderer.DOFade(0f, activeDuration))
    //                 .SetAutoKill(false)
    //                 .OnComplete(() =>
    //                 {
    //                     tweening = false;
    //                     setActive = true;
    //                 });
    //         }
    //     }
    //     else
    //     {
    //         if (!tweening && setActive)
    //         {
    //             tweening = true;
    //             materialSequence.OnPlay(() =>
    //             {
    //                 tweening = false;
    //                 setActive = false;
    //             });
    //             materialSequence.PlayBackwards();
    //         }
    //     }
    // }

    protected override void FixedUpdate()
    {
        playerInRange = Physics2D.OverlapCircle(transform.position, fxRadius, LayerMask.GetMask("Player"));
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!canInteract) return;
        if (itemData != null)
        {
            if (promptMessage == String.Empty)
            {
                promptMessage = $"Pickup {itemData.itemName} - RMB";
            }
            foreach (var particle in particles)
            {
                particle.Stop(false);
            }
            base.OnTriggerEnter2D(other);
        }
        else
        {
            Debug.LogWarning("Item has not been set on pickup!");
        }
    }

    protected override void Interact(CharacterController cc)
    {
        if (!canInteract) return;
        cc.AddToInventory(itemData);
        AudioManager.Instance.PlaySound(pickupSfx[Random.Range(0, pickupSfx.Count-1)]);
        //itemData.hasBeenPickedUp = true;
        canInteract = false;
        DisablePrompt();
        renderer.DOFade(0f, .2f);
        glowRenderer.DOFade(0f, .2f);
        StartCoroutine(WaitForParticles());
    }

    private IEnumerator WaitForParticles()
    {
        foreach (var particle in particles)
        {
            particle.Stop(false);
        }
        yield return new WaitForSeconds(2f);
        Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
        //ItemPickedUp?.Invoke();
        if (promptBox.isAnimating)
        {
            
        }
        gameObject.SetActive(false);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (canInteract)
        {
            foreach (var particle in particles)
            {
                particle.Play();
            }
        }
    }
    
    public override void SetInteractedState(object state)
    {
        base.SetInteractedState(state);
        var interactionState = state as InteractionState;
        gameObject.SetActive(false);
    }
}