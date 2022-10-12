/*******************************************************************************************
*
*    File: SanityMeter.cs
*    Purpose: UI Element to show Player Sanity Levels
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class SanityMeter : MonoBehaviour
{
    [SerializeField] private Slider m_SanityMeter;
    private bool m_DecreaseSlider = false;
    private CharacterController m_Player;

    private void Awake()
    {
        m_Player = FindObjectOfType<CharacterController>();
    }

    private void Start()
    {
        m_DecreaseSlider = true;
    }

    private void Update()
    {
        if (!m_DecreaseSlider)
        {
            return;
        }

        if (m_SanityMeter.value <= 0f)
        {
            return;
        }

        m_SanityMeter.value = m_Player.getSanity;
    }
}