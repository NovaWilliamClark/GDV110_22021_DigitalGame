using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class MouseFollow : MonoBehaviour
{
    public GameObject player;
    private CharacterController characterController;
    [SerializeField] private Transform pivot;
    [SerializeField] private PlayerData_SO playerData;
    public GameObject flashlightVisual;
    private SpriteRenderer flashlightVisualRenderer;
    [SerializeField] private Light2D light;

    [SerializeField] private Vector2 leftPosOffset;
    [SerializeField] private Vector2 rightPosOffset;

    [SerializeField] private GameObject handPosition;

    private Vector3 flashlightRendererStartPos;
    
    public void Init(PlayerData_SO data, CharacterController player)
    {
        playerData = data;
        characterController = player;
    }
    
    private void Awake()
    {
        flashlightVisualRenderer = flashlightVisual.GetComponentInChildren<SpriteRenderer>();
        flashlightRendererStartPos = flashlightVisualRenderer.transform.localPosition;
    }

    private void Update()
    {
        if (flashlightVisual)
        {
            if (!playerData.equipmentState.flashlightIsOn)
            {
                flashlightVisual.transform.position = handPosition.transform.position;
                flashlightVisual.transform.rotation = handPosition.transform.rotation;
                if (characterController.IsFacingLeft())
                {
                    flashlightVisualRenderer.transform.localPosition = flashlightRendererStartPos + (Vector3) leftPosOffset;
                    flashlightVisualRenderer.flipY = true;
                    flashlightVisualRenderer.flipX = true;
                }
                else
                {
                    flashlightVisualRenderer.transform.localPosition = flashlightRendererStartPos;
                    flashlightVisualRenderer.flipY = false;
                    flashlightVisualRenderer.flipX = false;
                }
            }
            else
            {
                flashlightVisualRenderer.transform.localPosition = flashlightRendererStartPos;
                flashlightVisual.transform.position = transform.position;
                flashlightVisual.transform.rotation = transform.rotation;
                flashlightVisualRenderer.flipY = false;
                flashlightVisualRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerData.flashlightAvailable && playerData.CurrentBattery > 0)
        {
            var mouseRaw = Mouse.current.position.ReadValue();
            var mouse = new Vector3(mouseRaw.x, mouseRaw.y, Vector3.Distance(player.transform.position, Camera.main.transform.position));
            var mousePos = Camera.main.ScreenToWorldPoint(mouse);
            // var flip = characterController.IsFacingLeft() ? -1 : 1;
            // mousePos.x = pivot.transform.position.x + (5f * flip);

            // invert mouse position
            if (characterController.IsFacingLeft() && mousePos.x > characterController.transform.position.x)
            {
                var position = characterController.transform.position;
                mousePos.x = position.x - Mathf.Abs(mousePos.x - position.x);
            }
            else if (!characterController.IsFacingLeft() && mousePos.x < characterController.transform.position.x)
            {
                var position = characterController.transform.position;
                mousePos.x = position.x + Mathf.Abs(mousePos.x - position.x);
            }
        
        
            Vector3 difference = mousePos - pivot.position;
        
            // Normalize mouse position
            difference.Normalize();

            // Get angle of mouse
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        
            // Flashlight clamp when player is facing right
            if (rotationZ < -45 && !characterController.IsFacingLeft())
            {
                rotationZ = -45;
            }
            else if (rotationZ > 45 && !characterController.IsFacingLeft())
            {
                rotationZ = 45;
            }
        
            // Flashlight clamp when player is facing left
            if (rotationZ > -135 && rotationZ < 0 && characterController.IsFacingLeft())
            {
                rotationZ = -135;
            }
            else if (rotationZ < 135 && rotationZ >= 0 && characterController.IsFacingLeft())
            {
                rotationZ = 135;
            }

            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
    }
}
