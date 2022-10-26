/*******************************************************************************************
*
*    File: SanityMeter.cs
*    Purpose: UI Element to show Player Sanity Levels
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class SanityMeter : MonoBehaviour
    {
        [SerializeField] private Slider sanityMeter;
        public bool decreaseSlider;
        private CharacterController player;

        private void Update()
        {
            if (!decreaseSlider)
            {
                return;
            }

            if (sanityMeter.value <= 0f)
            {
                Image[] list = GetComponentsInChildren<Image>();
                list.ElementAt(1).enabled = false;
                return;
            }

            sanityMeter.value = player.getSanity;
        }

        public void SetPlayer(CharacterController characterController)
        {
            player = characterController;
            decreaseSlider = true;
        }
    }
}