/*******************************************************************************************
*
*    File: UIMainMenu.cs
*    Purpose: Main Menu
*    Author: Joshua Taylor
*    Date: 21/10/2022
*
**********************************************************************************************/

using Audio;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    private void Start()
    {
        AudioManager.Instance.PlayMusic(menuMusic, .5f);
        UIHelpers.Instance.Fader.Fade(0f, 2f, OnFaded);
    }

    private void OnFaded()
    {
        // enable interaction
        // fade in stuff
        Debug.Log("Faded in");
    }

    public void QuitGame()
    {
        AudioManager.Instance.StopMusic(true, 1f);
        UIHelpers.Instance.Fader.Fade(1f, 2f, () =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        });

    }
}
