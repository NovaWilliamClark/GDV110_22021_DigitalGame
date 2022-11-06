using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollow : MonoBehaviour
{
    public GameObject player;
    private CharacterController characterController;

    private void Start()
    {
        characterController = player.GetComponent<CharacterController>();
    }
    
    private void FixedUpdate()
    {
        var mouseRaw = Mouse.current.position.ReadValue();
        var mouse = new Vector3(mouseRaw.x, mouseRaw.y, Vector3.Distance(player.transform.position, Camera.main.transform.position));
        var mousePos = Camera.main.ScreenToWorldPoint(mouse);
        Vector3 difference = mousePos - transform.position;
        
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
