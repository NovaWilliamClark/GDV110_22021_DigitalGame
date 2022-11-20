using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollow : MonoBehaviour
{
    public GameObject player;
    private CharacterController characterController;
    [SerializeField] private Transform pivot;
    [SerializeField] private PlayerData_SO playerData;
    public GameObject flashlightVisual;
    private SpriteRenderer flashlightVisualRenderer;

    [SerializeField] private Vector2 leftPosOffset;
    [SerializeField] private Vector2 rightPosOffset;

    [SerializeField] private GameObject handPosition;

    public void Init(PlayerData_SO data, CharacterController player)
    {
        playerData = data;
        characterController = player;
    }
    
    private void Awake()
    {
        flashlightVisualRenderer = flashlightVisual.GetComponentInChildren<SpriteRenderer>();
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

            if (flashlightVisual)
            {
                if (characterController.IsFacingLeft())
                {
                    flashlightVisualRenderer.flipX = true;
                    flashlightVisualRenderer.flipY = true;
                }
                else
                {
                    flashlightVisualRenderer.flipX = false;
                    flashlightVisualRenderer.flipY = false;
                }

                flashlightVisual.transform.position = handPosition.transform.position;
                flashlightVisual.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            }
        }
    }
}
