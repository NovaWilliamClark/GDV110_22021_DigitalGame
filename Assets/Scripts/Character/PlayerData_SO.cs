﻿/*******************************************************************************************
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
    private float sanity = 100f;

    [SerializeField] private float maxSanity = 100f;
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

    [SerializeField] private float maxBattery = 100f;
    public float MaxBattery => maxBattery;

    private float currentBattery = 0f;

    public bool flashlightAvailable = false;

    [HideInInspector]
    public UnityEvent<float> BatteryValueChanged;
    [HideInInspector]
    public UnityEvent<float> SanityValueChanged;

    [Header("Equipment")] public EquipmentState initialState;
    [FormerlySerializedAs("currentState")] public EquipmentState equipmentState;
    private PlayerData_SO originalSo;

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
        equipmentState = initialState.Copy();
        inventoryItems.Clear();
        wasDead = false;
        breakerFixed = false;
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    public PlayerData_SO CreateInstance()
    {
        var instance = Instantiate(this);
        instance.Init();
        return instance;
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

    public void ResetData()
    {
        //sanity = 100f;
        //sanityGainRate = 0.025f;
        //sanityLossRate = 0.03f;
        //inventoryItems.Clear();
    }

    public PlayerData_SO Copy()
    {
        // Create copy of SO (exclusively for spawn points atm)
        var instance = Instantiate(this);
        instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
        instance.equipmentState = equipmentState.Copy();
        //instance.spawnPoint = spawnPoint.Copy();
        
        return instance;
    }

    // https://stackoverflow.com/questions/930433/apply-properties-values-from-one-object-to-another-of-the-same-type-automaticall
    public void SetFromCopy(PlayerData_SO copy)
    {
        Debug.Log("Overwriting OG PlayerData with Spawn Point copy");
        var ogName = name;
        foreach (PropertyInfo property in typeof(PlayerData_SO).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(copy, null), null);
        }

        name = ogName;
    }
}