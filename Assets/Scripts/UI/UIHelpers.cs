/*******************************************************************************************
*
*    File: UIHelpers.cs
*    Purpose: Singleton of UI Helpers - particularly Fading
*    Author: Joshua Taylor
*    Date: 21/10/2022
*
**********************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHelpers : MonoBehaviour
{
    public static UIHelpers Instance { get; private set; }

    public UIFader Fader;
    public SanityVisual SanityMeter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Fader.gameObject.SetActive(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;
    }
}
