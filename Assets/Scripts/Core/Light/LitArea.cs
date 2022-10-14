/*******************************************************************************************
*
*    File: Light.cs
*    Purpose: Light Interaction and Triggers
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/

using System;
using UnityEngine;

namespace Core.LitArea
{
    
    public class LitArea : MonoBehaviour
    {
        public static event Action<Collider2D> onLightEnter;
        public static event Action<Collider2D> onLightExit;
        private void OnTriggerEnter2D(Collider2D other)
        {
            onLightEnter?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            onLightExit?.Invoke(other);
        }
    }
}
