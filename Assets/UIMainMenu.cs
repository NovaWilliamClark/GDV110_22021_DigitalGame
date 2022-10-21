using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using AudioType = Audio.AudioType;

public class UIMainMenu : MonoBehaviour
{
    public void QuitGame()
    {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
    }
}
