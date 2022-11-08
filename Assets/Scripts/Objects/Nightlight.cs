/*******************************************************************************************
*
*    File: Nightlight.cs
*    Purpose: Prototype Nightlight - Just visual
*    Author: Joshua Taylor
*    Date: 14/10/2022
*
**********************************************************************************************/

using Audio;
using System.Collections.Generic;
using System.Collections;
using System;
using Core.LitArea;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Nightlight : InteractionPoint
{
    [Header("Nightlight")]
    private Animator _animator;
    private Light2D _light;
    public AudioClip ActivateSFX;
    public float sfxVolume;
    private bool _activated = false;
    private float lightIntensity;

    private LitArea litArea;
    public bool requiresBattery = true;
    public bool turnedOn = false;
    [FormerlySerializedAs("batteryItem")] [SerializeField] private ItemData batteryItemData;
    [SerializeField] private string missingBatteryMessage;
    private bool hasItem = false;

    protected override void Awake()
    {
        base.Awake();
        litArea = GetComponent<LitArea>();
        _light = GetComponentInChildren<Light2D>();

    }
    
    protected override void Start()
    {
        base.Start();
        litArea.isEnabled = false;
        lightIntensity = _light.intensity;
        _light.intensity = 0f;
        if (!requiresBattery && automaticInteraction)
        {
                Interact(null);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            if (!canInteract) return;
            if (!promptBox) return;
            
            playerRef = other.GetComponent<CharacterController>();

            hasItem = playerRef.GetInventory.HasItem(batteryItemData);
            var msg = !hasItem ? missingBatteryMessage : promptMessage;
            promptBox.GetComponentInChildren<TMP_Text>().text = msg;
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

    protected override void Update()
    {
        base.Update();
    }

    protected override void Interact(CharacterController cc)
    {
        if (requiresBattery)
        {
            hasItem = playerRef.GetInventory.HasItem(batteryItemData);
            if (!hasItem) return;
            playerRef.GetInventory.UseItem(batteryItemData);
        }

        _light.intensity = lightIntensity;
        litArea.isEnabled = true;
        hasInteracted = true;
        canInteract = false;
    }
}