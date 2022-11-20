/*******************************************************************************************
*
*    File: PlayerData_SO.cs
*    Purpose: Holds data for the player that can persists between scenes
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Character;
using Objects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


[CreateAssetMenu(menuName = "Create PlayerData", fileName = "PlayerData", order = 0)]
public class PlayerData_SO : ScriptableObject
{
    [Header("--Sanity--")] 
    protected float sanity = 100f;

    [SerializeField] protected float maxSanity = 100f;
    public float MaxSanity => maxSanity;
    
    public float Sanity
    {
        get => sanity;
        set
        {
            sanity = Mathf.Clamp(value, 0f, maxSanity);
            SanityValueChanged?.Invoke(sanity);
        }
    }
    public float sanityGainRate = 0.025f;
    public float sanityLossRate = 0.03f;

    public float initialSanity = 100f;
    public float initialGainRate = 0.03f;
    public float initialLossRate = 0.1f;

    public bool wasDead = false;

    [Header("--Flashlight--")]
    public float initialBattery = 0f;

    [SerializeField] protected float maxBattery = 100f;
    public float MaxBattery => maxBattery;

    protected float currentBattery = 0f;

    public bool flashlightAvailable = false;

    [HideInInspector]
    public UnityEvent<float> BatteryValueChanged;
    [HideInInspector]
    public UnityEvent<float> SanityValueChanged;

    [Header("Equipment")] public EquipmentState initialState;
    [FormerlySerializedAs("currentState")] public EquipmentState equipmentState;
    private PlayerData_SO originalSo;
    public bool playingCutscene = false;

    [Header("Objectives")] 
    public bool breakerFixed;

    public float CurrentBattery
    {
        get => currentBattery;
        set
        {
            currentBattery = Mathf.Clamp(value,0f, maxBattery);
            BatteryValueChanged.Invoke(currentBattery);
        }
    }
    
    public List<ItemData> inventoryItems;

    public void Init()
    {
        CurrentBattery = initialBattery;
        sanity = initialSanity;
        sanityGainRate = initialGainRate;
        sanityLossRate = initialLossRate;
        equipmentState = initialState.Clone() as EquipmentState;
        inventoryItems.Clear();
        wasDead = false;
        breakerFixed = false;
    }

    public void InitCopy(PlayerData_SO original)
    {
        sanity = original.sanity;
        maxSanity = original.maxSanity;
        currentBattery = original.currentBattery;
        maxBattery = original.maxBattery;
    }

    public PlayerData_SO CreateCopy()
    {
        var instance = Instantiate(this);
        instance.equipmentState = equipmentState.Clone() as EquipmentState;
        instance.inventoryItems = new List<ItemData>();
        instance.InitCopy(this);
        foreach (var item in inventoryItems)
        {
            instance.inventoryItems.Add(item);
        }
        return instance;
    }

    public void ResetSanity()
    {
        sanity = maxSanity;
    }

    public void SetItems(Inventory inventory)
    {
        inventoryItems = inventory.items;
        /*foreach (var item in inventory.items)
        {
            if (inventoryItems.Contains(item))
            {
                continue;
            }
            inventoryItems.Add(item.GetItem);
        }*/
    }
}