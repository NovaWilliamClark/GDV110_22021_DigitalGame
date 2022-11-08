using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;

public class CharacterEquipment : MonoBehaviour
{
    private CharacterController charController;
    private PlayerData_SO data;

    [SerializeField] private SpriteResolver resolver;

    [SerializeField] private ItemData sockeyRef;

    [FormerlySerializedAs("flashlightItemRef")]
    [Header("Flashlight")] 
    [SerializeField] private ItemData flashlightItemDataRef;
    [SerializeField] private GameObject flashlightObject;
    [SerializeField] private ArmMouseTracking trackingScript;
    private bool flashlightCooldownComplete = true;

    private PlayerInput input;
    private InputAction useFlashlightInput;

    private bool canUseFlashlight = false;
    private bool flashlightOn = false;
    
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    public void Init(PlayerData_SO pdata)
    {
        charController.GetInventory.ItemAdded.AddListener(OnItemAddedToInventory);
        data = pdata;
        input = new PlayerInput();
        useFlashlightInput = input.Player.UseFlashlight;
        useFlashlightInput.performed += OnFlashlightInput;
        useFlashlightInput.Enable();

        if (data.equipmentState.flashlightIsOn)
        {
            ToggleFlashlight(true);
        }
    }

    private void ToggleFlashlight(bool on)
    {
        var startVal = on ? 0 :1f;
        var endVal = on ? 1f : 0f;
        var tween = DOVirtual.Float(startVal, endVal, .2f, val =>
        {
            trackingScript.solver.weight = val;
        });
        if (on)
        {
            tween.OnComplete(() =>
            {
                flashlightObject.SetActive(true);
                data.equipmentState.flashlightIsOn = true;
                flashlightCooldownComplete = false;
                StartCoroutine(FlashlightCooldown());
            });
        }
        else
        {
            tween.OnComplete(() =>
            {
                flashlightObject.SetActive(false);
                data.equipmentState.flashlightIsOn = false;
                flashlightCooldownComplete = false;
                StartCoroutine(FlashlightCooldown());
            });
        }

        tween.Play();
    }

    private void OnFlashlightInput(InputAction.CallbackContext obj)
    {
        Debug.Log("Input");
        if (!data.equipmentState.flashlightEquipped || !flashlightCooldownComplete) return;
        ToggleFlashlight(!data.equipmentState.flashlightIsOn);
    }

    private void OnDisable()
    {
        charController.GetInventory.ItemAdded.RemoveListener(OnItemAddedToInventory);
        useFlashlightInput.performed -= OnFlashlightInput;
    }

    private void OnItemAddedToInventory(ItemData original)
    {
        if (original == sockeyRef)
        {
            resolver.SetCategoryAndLabel("Hands","Sockey");
            resolver.ResolveSpriteToSpriteRenderer();
        }

        if (original == flashlightItemDataRef)
        {
            data.equipmentState.flashlightEquipped = true;
            canUseFlashlight = true;
        }
    }

    public void DisableInput()
    {
        useFlashlightInput.performed -= OnFlashlightInput;
    }

    public void EnableInput()
    {
        useFlashlightInput.performed += OnFlashlightInput;
    }
    
    IEnumerator FlashlightCooldown()
    {
        yield return new WaitForSeconds(1);

        flashlightCooldownComplete = true;
    }
}

[Serializable]
public class EquipmentState
{
    public bool hasSockey = false;
    [Header("Flashlight")]
    public bool flashlightEquipped = false;
    public bool flashlightIsOn = false;
}