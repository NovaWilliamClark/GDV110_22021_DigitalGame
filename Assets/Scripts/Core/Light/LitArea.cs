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
        // public static event Action<Collider2D> onLightEnter;
        // public static event Action<Collider2D> onLightExit;
        // public LayerMask ignoreLayers;
        private void OnTriggerEnter2D(Collider2D other)
        {
            // if (((1 << other.gameObject.layer) & ignoreLayers) == 0)
            // {
            //     onLightEnter?.Invoke(other);
            // }
            if (other.GetComponent(typeof(ILightResponder)) is ILightResponder responder)
            {
                responder.OnLightEntered(1f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // if (((1 << other.gameObject.layer) & ignoreLayers) == 0)
            // {
            //     onLightExit?.Invoke(other);
            // }
            if (other.GetComponent(typeof(ILightResponder)) is ILightResponder responder)
            {
                responder.OnLightExited(1f);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.GetComponent(typeof(ILightResponder)) is ILightResponder responder)
            {
                responder.OnLightStay(1f);
            }
        }
    }
}
