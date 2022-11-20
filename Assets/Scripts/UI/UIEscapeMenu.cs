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
    private LevelController currentLevelController;
    private CharacterController playerRef;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ShowHide(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;
        currentLevelController = FindObjectOfType<LevelController>();
        currentLevelController.PlayerSpawned.AddListener(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(CharacterController cc)
    {
        playerRef = cc;
        currentLevelController.PlayerSpawned.RemoveListener(OnPlayerSpawned);
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
        if (playerRef)
        {
            playerRef.ToggleActive(!show);
            var sanity = playerRef.GetComponent<CharacterSanity>();
            sanity.AdjustDecreaseRate(0f, !show);
        }
        // fade audio down
    }

    public void MainMenu()
    {
        ShowHide(false);
        
        SpawnManager.Instance.ResetState();

        if (playerRef.PlayerData.equipmentState.flashlightEquipped)
        {
            var equipment = playerRef.Equipment;
            playerRef.Equipment.FlashlightVisual.SetActive(false);
            UIHelpers.Instance.BatteryIndicator.Hide();
        }
        
        // This code resets your equipment state when you go to the main menu,
        // need to clarify if we want to start anew or continue game from menu
        // data.equipmentState = EquipmentState.Reset();
        // inventory.items.Clear();
        // inventory.slots.Clear();
        
        UIHelpers.Instance.SanityMeter.UnsetPlayer();
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}
