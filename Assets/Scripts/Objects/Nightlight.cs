/*******************************************************************************************
*
*    File: Nightlight.cs
*    Purpose: Prototype Nightlight - Just visual
*    Author: Joshua Taylor
*    Date: 14/10/2022
*
**********************************************************************************************/

using Audio;
using System.Collections.Generic;
using System.Collections;
using System;
using Core.LitArea;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Nightlight : InteractionPoint
{
    [Header("Nightlight")]
    private Animator _animator;
    private Light2D _light;
    public AudioClip ActivateSFX;
    public float sfxVolume;
    private bool _activated = false;
    private float lightIntensity;

    private LitArea litArea;
    public bool requiresBattery = true;
    public bool turnedOn = false;
    [FormerlySerializedAs("batteryItem")] [SerializeField] private ItemData batteryItemData;
    [SerializeField] private string missingBatteryMessage;
    private bool inRadius;
    public float saveCooldown = 5f;


    //public UnityEvent<int> Interacted;

    private int id;
    private LevelController controller;

    protected override void Awake()
    {
        base.Awake();
        litArea = GetComponentInChildren<LitArea>();
        _light = GetComponentInChildren<Light2D>();
        controller = FindObjectOfType<LevelController>();
        controller.PlayerSpawned.AddListener(OnPlayerSpawned);
        canInteract = false;
    }

    protected override void Start()
    {
        base.Start();
        
    }
    
    private void OnPlayerSpawned(CharacterController cc)
    {
        canInteract = true;
        lightIntensity = _light.intensity;
        if (requiresBattery)
        {
            litArea.isEnabled = false;
            _light.intensity = 0f;
        }
        else
        {
            litArea.isEnabled = true;
        }

    }

    public void Init(int idInScene, LevelController lvlcontroller)
    {
        id = idInScene;
        controller = lvlcontroller;
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        base.OnTriggerStay2D(other);
        inRadius = true;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        var spawnMan = SpawnManager.Instance;
        if (!other.CompareTag("Player")) return;
        if (!other.GetComponent<CharacterController>()) return;
        if (!canInteract) return;
        if (!promptBox) return;

        playerRef = other.GetComponent<CharacterController>();
        string msg = "";
        if (!automaticInteraction)
        {
            hasItem = playerRef.GetInventory.HasItem(batteryItemData);
            msg = !hasItem ? missingBatteryMessage : promptMessage;
        }
        else
        {
            var sceneName = SceneManager.GetActiveScene().name;
            msg = "Saved";
            
            var newerSpawnPoint = false;
            var nullSpawnPoint = spawnMan.CurrentSpawnPoint == null;
            if (!nullSpawnPoint)
            {
                var spLvlData = spawnMan.CurrentSpawnPoint.LevelDataAtSpawn;
                // is this level newer
                newerSpawnPoint = spLvlData.createdAt < spawnMan.GetCurrentLevelData(sceneName).createdAt || 
                                  spLvlData.sceneName == sceneName && spLvlData.createdAt < Time.time;
            }

            if (newerSpawnPoint || nullSpawnPoint)
            {
                ShowPrompt(msg);
                StartCoroutine(WaitThenInteract());
                return;
            }
        }
        
        promptBox.gameObject.SetActive(true);
        promptBox.Show(msg);
    }

    private void ShowPrompt(string msg)
    {
        promptBox.gameObject.SetActive(true);
        promptBox.Show(msg);
    }

    private IEnumerator WaitThenInteract()
    {
        yield return new WaitForSeconds(1f);
        Interact(playerRef);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if(!other.GetComponent<CharacterController>()) return;
        playerRef = null;
        if (automaticInteraction)
        {
            StartCoroutine(SaveCooldown());
        }

        DisablePrompt();
    }

    private IEnumerator SaveCooldown()
    {
        yield return new WaitForSeconds(saveCooldown);
        canInteract = true;
        hasInteracted = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetInteractedState(object state)
    {
        base.SetInteractedState(state);
        if (state is not InteractionState interactionState) return;
        
        litArea.isEnabled = interactionState.interacted;
        _light.intensity = interactionState.interacted ? _light.intensity : 0f;
        hasInteracted = interactionState.interacted;
        canInteract = !interactionState.interacted;

        if (hasInteracted)
        {
            automaticInteraction = true;
            requiresBattery = false;
        }
    }

    protected override void Interact(CharacterController cc)
    {
        if (requiresBattery)
        {
            hasItem = playerRef.GetInventory.HasItem(batteryItemData);
            if (!hasItem) return;
            playerRef.GetInventory.UseItem(batteryItemData);
            AudioManager.Instance.PlaySound(ActivateSFX, sfxVolume);
        }

        if (!automaticInteraction)
        {
            _light.intensity = lightIntensity;
            litArea.isEnabled = true;
        }

        requiresBattery = false;
        automaticInteraction = true;
        hasInteracted = true;
        canInteract = false;
        
        DisablePrompt();
        Debug.Log("Nightlight Interacted");
        Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
    }

    protected override void DisablePrompt()
    {
        if (promptBox)
        {
            promptBox.Hide();
        }
    }
}