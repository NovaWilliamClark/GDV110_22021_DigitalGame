using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIEscapeMenu : MonoBehaviour
{
    public GameObject Container;
    private CanvasGroup canvasGroup;
    public CharacterEquipment characterEquipment;
    public Inventory inventory;
    public PlayerData_SO data;

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
        characterEquipment.FlashlightVisual.SetActive(false);
        UIHelpers.Instance.BatteryIndicator.Hide();
        
        // This code resets your equipment state when you go to the main menu,
        // need to clarify if we want to start anew or continue game from menu
        // data.equipmentState = EquipmentState.Reset();
        // inventory.items.Clear();
        // inventory.slots.Clear();
        
        UIHelpers.Instance.SanityMeter.UnsetPlayer();
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}
