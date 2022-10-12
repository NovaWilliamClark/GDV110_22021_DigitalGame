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

namespace Core
{
    internal class Light : MonoBehaviour
    {
        public static event Action<Collider2D> onLightEnter;
        public static event Action<Collider2D> onLightExit;
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            onLightEnter?.Invoke(other);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            onLightExit?.Invoke(other);
        }
    }
}
