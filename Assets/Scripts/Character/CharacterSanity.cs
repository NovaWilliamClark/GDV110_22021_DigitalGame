using System;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class CharacterSanity : MonoBehaviour, ILightResponder
    {
        [SerializeField] private PlayerData_SO playerData;
        public UnityEvent<float, float> SanityValueChanged;
        public UnityEvent SanityReachedZero;
        [SerializeField] bool sanityEnabled = false;
        public bool Enabled => sanityEnabled;

        private bool isInLight = false;

        public void Disable()
        {
            sanityEnabled = false;
        }

        public void Enable()
        {
            sanityEnabled = true;
        }

        private void Update()
        {
            if (!sanityEnabled) return;
            if (isInLight)
            {
                HealSanity(playerData.sanityGainRate);
            }
            else
            {
                DecreaseSanity(playerData.sanityLossRate, true);
            }
        }

        public void DecreaseSanity(float amount, bool fromDarkness)
        {
            playerData.Sanity -= amount;
            SanityValueChanged?.Invoke(playerData.Sanity, playerData.MaxSanity);
            if (playerData.Sanity <= 0)
            {
                SanityReachedZero?.Invoke();
            }
        }

        public void HealSanity(float amount)
        {
            playerData.Sanity += amount;
            SanityValueChanged?.Invoke(playerData.Sanity, playerData.MaxSanity);
        }
        
        public void OnLightEntered(float intensity)
        {
            isInLight = true;
        }

        public void OnLightExited(float intensity)
        {
            isInLight = false;
        }

        public void OnLightStay(float intensity)
        {
            isInLight = true;
        }
    }
}