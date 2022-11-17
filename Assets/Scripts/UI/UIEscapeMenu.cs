using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIEscapeMenu : MonoBehaviour
{
    public GameObject Container;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ShowHide(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ShowHide(!Container.activeInHierarchy);
        }
    }

    public void ShowHide(bool show = true)
    {
        Container.SetActive(show);
        Time.timeScale = show ? 0f : 1f;
        Cursor.visible = true ? true : false;
        // fade audio down
    }

    public void MainMenu()
    {
        ShowHide(false);
        UIHelpers.Instance.SanityMeter.UnsetPlayer();
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}
