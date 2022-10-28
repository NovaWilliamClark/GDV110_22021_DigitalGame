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
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Nightlight : MonoBehaviour
{
    private Animator _animator;
    private Light2D _light;
    public AudioClip ActivateSFX;
    public float sfxVolume;
    private bool _activated = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _light = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        Debug.Log(_light.intensity);
        _light.intensity = 0.0f;
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_animator && other.CompareTag("Player"))
        {
            if (!_activated)
            {
                _activated = true;
                _animator.SetTrigger("Activate");
                AudioManager.Instance.PlaySound(ActivateSFX);
            }
        }
    }
}