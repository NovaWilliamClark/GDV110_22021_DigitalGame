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
    [RequireComponent(typeof(BoxCollider2D))]
    public class Light : MonoBehaviour
    {
        public static event Action<Collider2D> onLightEnter;
        public static event Action<Collider2D> onLightExit;

        private void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<CharacterController>())
            {
                onLightEnter?.Invoke(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<CharacterController>())
            {
                onLightExit?.Invoke(other);
            }
        }
    }
}
