/*******************************************************************************************
*
*    File: Nightlight.cs
*    Purpose: Prototype Nightlight - Just visual
*    Author: Joshua Taylor
*    Date: 14/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Nightlight : MonoBehaviour
{
    private Animator _animator;
    private Light2D _light;

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
            _animator.SetTrigger("Activate");
        }
    }
}
