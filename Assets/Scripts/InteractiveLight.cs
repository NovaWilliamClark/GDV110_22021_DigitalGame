/*******************************************************************************************
*
*    File: InteractiveLight.cs
*    Purpose: Provides a light that the player can turn on
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/



using TMPro;
using UnityEngine;

public class InteractiveLight : Light
{
    [SerializeField] private GameObject lights;
    [SerializeField] private Canvas prompt;
    private bool isLit = false;


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isLit)
        {
            base.OnTriggerEnter2D(other);
        }

        prompt.GetComponent<TMP_Text>().text = "Interact - " + Input.GetButton("Submit");
        prompt.gameObject.SetActive(true);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (isLit)
        {
            base.OnTriggerExit2D(other);
        }
        else
        {
            prompt.gameObject.SetActive(false);
        }
    }
}