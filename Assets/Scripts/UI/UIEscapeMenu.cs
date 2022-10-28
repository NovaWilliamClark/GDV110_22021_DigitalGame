using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ShowHide(!Container.activeInHierarchy);
        }
    }

    public void ShowHide(bool show = true)
    {
        Container.SetActive(show);
        Time.timeScale = show ? 0f : 1f;
    }

    public void MainMenu()
    {
        ShowHide(false);
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}
