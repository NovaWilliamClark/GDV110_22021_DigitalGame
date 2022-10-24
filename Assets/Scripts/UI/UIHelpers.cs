/*******************************************************************************************
*
*    File: UIHelpers.cs
*    Purpose: Singleton of UI Helpers - particularly Fading
*    Author: Joshua Taylor
*    Date: 21/10/2022
*
**********************************************************************************************/

using UnityEngine;

public class UIHelpers : MonoBehaviour
{
    public static UIHelpers Instance { get; private set; }

    public UIFader Fader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Fader.gameObject.SetActive(true);
    }
}
