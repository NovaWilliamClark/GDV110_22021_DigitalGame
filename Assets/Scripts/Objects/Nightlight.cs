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
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Nightlight : InteractionPoint
{
    private Animator _animator;
    private Light2D _light;
    public AudioClip ActivateSFX;
    public float sfxVolume;
    private bool _activated = false;
    private float lightIntensity;

    private LitArea litArea;
    public bool requiresBattery = true;
    public bool turnedOn = false;

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

    protected override void Update()
    {
    }

    protected override void Interact(CharacterController cc)
    {
        _light.intensity = lightIntensity;
        litArea.isEnabled = true; 
    }
}