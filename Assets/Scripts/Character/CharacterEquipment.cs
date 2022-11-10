using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
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

    [SerializeField] private AudioClip flashlightActivateSfx;
    [SerializeField] private float sfxVolume;

    [SerializeField] private ItemUseEvent flashlightUseEvent;

    private Coroutine degenCoroutine;
    private bool degenStarted;
    private bool turnedOff = false;

    private bool initialized = false;

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
        data.BatteryValueChanged.AddListener(OnBatteryValueChanged);
        useFlashlightInput.Enable();

        initialized = true;
        if (data.equipmentState.flashlightIsOn)
        {
            ToggleFlashlight(true);
        }
    }

    private void Update()
    {
        if (!initialized) return;
        if (data.equipmentState.flashlightIsOn)
        {
            data.CurrentBattery -= data.equipmentState.flashlightDecreaseRate;
            if (data.CurrentBattery <= 0f)
            {
                if (!turnedOff)
                {
                    turnedOff = true;
                    ToggleFlashlight(false);
                }
            }
        }
    }

    private void OnBatteryValueChanged(float arg0)
    {
        Debug.Log("Battery recovered " + arg0);

        if (arg0 <= 0)
        {
            flashlightObject.SetActive(false);
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
        if (data.CurrentBattery <= 0 && data.flashlightAvailable)
        {
            var seq = DOTween.Sequence();
            seq.Append(tween);
            seq.AppendInterval(.5f);
            seq.Append(DOVirtual.DelayedCall(0f, () =>
            {
                AudioManager.Instance.PlaySound(flashlightActivateSfx,sfxVolume);
            }));
            seq.AppendInterval(.5f);
            seq.Append(DOVirtual.Float(1f, 0f, .2f, val =>
            {
                trackingScript.solver.weight = val;
            }));
            seq.OnComplete(() =>
            {
                data.equipmentState.flashlightIsOn = false;
                flashlightCooldownComplete = false;
                StartCoroutine(FlashlightCooldown());
            });
            seq.Play();
            return;
        }
        if (on && data.flashlightAvailable)
        {
            tween.OnComplete(() =>
            {
                UIHelpers.Instance.BatteryIndicator.Toggle(true);
                flashlightObject.SetActive(true);
                data.equipmentState.flashlightIsOn = true;
                flashlightCooldownComplete = false;
                StartCoroutine(FlashlightCooldown());
            });
        }
        else if (!on && data.flashlightAvailable)
        {
            tween.OnComplete(() =>
            {
                UIHelpers.Instance.BatteryIndicator.Toggle(false);
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
            UIHelpers.Instance.BatteryIndicator.Show();
        }
    }
    public void ReloadFlashlight(float amount = 500f)
    {
        data.CurrentBattery = data.MaxBattery;
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
    public float flashlightDecreaseRate = 0.1f;

    public EquipmentState Copy()
    {
        var copy = new EquipmentState
        {
            hasSockey = hasSockey,
            flashlightEquipped = flashlightEquipped,
            flashlightIsOn = flashlightIsOn,
            flashlightDecreaseRate = flashlightDecreaseRate
        };
        return copy;
    }
}