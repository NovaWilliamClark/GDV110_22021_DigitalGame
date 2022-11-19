/*******************************************************************************************
*
*    File: UIMainMenu.cs
*    Purpose: Main Menu
*    Author: Joshua Taylor
*    Date: 21/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using Audio;
using Objects;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private GameObject controlScheme;
    public UIMenu StartMenu;
    private UIMenu currentMenu;
    private UIMenu previousMenu;

    public string StartGameScene;
    
    private void Start()
    {
        var menus = GetComponentsInChildren<UIMenu>(true);
        foreach (var menu in menus)
        {
            menu.gameObject.SetActive(true);
            menu.Init();
            if (menu != StartMenu)
            {
                menu.gameObject.SetActive(false);
            }
        }
        StartCoroutine(StartWithDelay());
    }
    
    

    private IEnumerator StartWithDelay()
    {
        // TODO: Instead of having a specific time delay this should be event based so it waits for the UIHelpers object to be ready
        yield return new WaitForSeconds(startDelay);
        
        AudioManager.Instance.PlayMusic(menuMusic, .5f);
        UIHelpers.Instance.Fader.Fade(0f, 2f, OnFaded);
    }

    private void OnFaded()
    {
        // enable interaction
        // fade in stuff
        currentMenu = StartMenu;
        currentMenu.Show();
    }

    public void PreviousMenu()
    {
        ChangeMenu(previousMenu);
    }
    
    public void ChangeMenu(UIMenu targetMenu)
    {
        currentMenu.Hide(() =>
        {
            previousMenu = currentMenu;
            previousMenu.gameObject.SetActive(false);
            currentMenu = targetMenu;
            currentMenu.gameObject.SetActive(true);
            currentMenu.Show();
            
        });
    }

    public void StartGame()
    {
        currentMenu.Hide(() =>
        {
            AudioManager.Instance.StopMusic(1f);
            UIHelpers.Instance.Fader.Fade(1f, 2f, () =>
            {
                StartCoroutine(ShowControlScheme());
                //TransitionManager.Instance.LoadScene(StartGameScene);
            });
        });
    }

    private IEnumerator ShowControlScheme()
    {
        Debug.Log("setting active");
        controlScheme.SetActive(true);
        yield return new WaitForSeconds(4f);
        TransitionManager.Instance.LoadScene(StartGameScene);
    }


    public void QuitGame()
    {
        currentMenu.Hide(() =>
        {
            AudioManager.Instance.StopMusic(1f);
            UIHelpers.Instance.Fader.Fade(1f, 2f, () =>
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                Application.Quit();
            });
        });
    }
}
