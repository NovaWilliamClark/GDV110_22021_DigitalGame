/*******************************************************************************************
*
*    File: InteractiveLight.cs
*    Purpose: Provides a light that the player can turn on
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/

using System.Collections;
using TMPro;
using UnityEngine;

public class InteractiveLight : Light
{
    [SerializeField] private GameObject lights;
    [SerializeField] private Canvas prompt;
    private bool isLit = false;
    private bool canInteract = false;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isLit)
        {
            base.OnTriggerEnter2D(other);
            return;
        }

        canInteract = true;
        prompt.GetComponentInChildren<TMP_Text>().text = "Interact - 'E'"; 
        prompt.gameObject.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!canInteract) return;
        if (!(Input.GetButton("Interact"))) return;
        prompt.gameObject.SetActive(false);
        lights.SetActive(true);
        isLit = true;
        canInteract = false;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x + 5, collider.size.y + 5);
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (isLit)
        {
            base.OnTriggerExit2D(other);
        }
        else
        {
            if (prompt.gameObject.activeInHierarchy)
            {
                prompt.gameObject.SetActive(false);
            }
        }
    }
}