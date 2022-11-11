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
        public bool isEnabled = true;
        // public static event Action<Collider2D> onLightEnter;
        // public static event Action<Collider2D> onLightExit;
        // public LayerMask ignoreLayers;

        [SerializeField] private CircleCollider2D collider;

        [SerializeField] private LayerMask affectedMasks;
        private void Awake()
        {
            collider = GetComponent<CircleCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isEnabled) return;
            if (other.GetComponent(typeof(ILightResponder)) is ILightResponder responder)
            {
                responder.OnLightEntered(1f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isEnabled) return;
            if (other.GetComponent(typeof(ILightResponder)) is ILightResponder responder)
            {
                responder.OnLightExited(1f);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isEnabled) return;
            if (other.GetComponent(typeof(ILightResponder)) is ILightResponder responder)
            {
                responder.OnLightStay(1f);
            }
        }

        public void SetEnabled(bool enable)
        {
            isEnabled = enable;
            var inArea = Physics2D.OverlapCircle(transform.position + (Vector3) collider.offset, collider.radius, affectedMasks);
            if (inArea)
            {
                if (inArea.GetComponent(typeof(ILightResponder)) is ILightResponder responder)
                {
                    if (!isEnabled)
                    {
                        responder.OnLightExited(1f);
                    }
                    else
                    {
                        responder.OnLightStay(1f);
                    }
                }
            }
        }
    }
}
