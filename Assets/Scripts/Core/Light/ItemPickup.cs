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
using DG.Tweening;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemPickup : InteractionPoint
{
    public Item GetItem => item;
    [SerializeField] private Item item;
    [SerializeField] private SpriteRenderer glowRenderer;

    [SerializeField] private float fxRadius = 10f;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;
    private float curTime = 0f;
    private bool setActive = false;
    [SerializeField] private float activeDuration = 1f;
    private Sequence materialSequence;
    private List<ParticleSystem> particles;
        
    protected override void Awake()
    {
        base.Awake();
        renderer = GetComponent<SpriteRenderer>();
        particles = GetComponentsInChildren<ParticleSystem>().ToList();
    }

    protected override void Start()
    {
        base.Start();

        if (item != null)
        {
            if (item.itemSprite != null && renderer != null && renderer.sprite == null)
            {
                if (glowRenderer)
                {
                    glowRenderer.sprite = item.itemSprite;
                }
                renderer.sprite = item.itemSprite;
            }
        }
    }

    protected override void Update()
    {
        if (!canInteract) return;
        if (playerInRange)
        {
            if (!tweening && !setActive)
            {
                tweening = true;
                materialSequence = DOTween.Sequence();
                materialSequence
                    .Insert(0, glowRenderer.DOFade(1f, activeDuration))
                    .Insert(0, renderer.DOFade(0f, activeDuration))
                    .SetAutoKill(false)
                    .OnComplete(() =>
                    {
                        tweening = false;
                        setActive = true;
                    });
            }
        }
        else
        {
            if (!tweening && setActive)
            {
                tweening = true;
                materialSequence.OnPlay(() =>
                {
                    tweening = false;
                    setActive = false;
                });
                materialSequence.PlayBackwards();
            }
        }
    }

    protected override void FixedUpdate()
    {
        playerInRange = Physics2D.OverlapCircle(transform.position, fxRadius, LayerMask.GetMask("Player"));
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!canInteract) return;
        if (item != null)
        {
            if (promptMessage == String.Empty)
            {
                promptMessage = $"Pickup {item.itemName} - RMB";
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
        cc.AddToInventory(item);
        item.hasBeenPickedUp = true;
        canInteract = false;
        DisablePrompt();
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
        gameObject.SetActive(false);
    }
}