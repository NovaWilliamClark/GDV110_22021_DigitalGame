/*******************************************************************************************
*
*    File: SanityMeter.cs
*    Purpose: UI Element to show Player Sanity Levels
*    Author: Sam Blakely
*    Date: 11/10/2022
*
**********************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class SanityMeter : MonoBehaviour
{
    [SerializeField] private Slider sanityMeter;
    private bool decreaseSlider = false;
    private CharacterController player;

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();
    }

    private void Start()
    {
        decreaseSlider = true;
    }

    private void Update()
    {
        if (!decreaseSlider)
        {
            return;
        }

        if (sanityMeter.value <= 0f)
        {
            return;
        }
        
        sanityMeter.value = player.getSanity;
    }
}