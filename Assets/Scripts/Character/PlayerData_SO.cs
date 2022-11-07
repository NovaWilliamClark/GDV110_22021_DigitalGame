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

    [Header("--Flashlight--")]
    public float initialBattery = 0f;

    [SerializeField] private float maxBattery = 100f;
    public float MaxBattery => maxBattery;

    private float currentBattery = 0f;

    [HideInInspector]
    public UnityEvent<float> BatteryValueChanged;
    [HideInInspector]
    public UnityEvent<float> SanityValueChanged;

    [Header("Equipment")] public EquipmentState initialState;
    [FormerlySerializedAs("currentState")] public EquipmentState equipmentState;
    
    public float CurrentBattery
    {
        get => currentBattery;
        set
        {
            currentBattery = Mathf.Min(value,maxBattery);
            BatteryValueChanged.Invoke(currentBattery);
        }
    }


    public List<Item> inventoryItems;

    private void OnEnable()
    {
        Debug.Log("PlayerDataSO Fired");
        CurrentBattery = initialBattery;
        sanity = initialSanity;
        sanityGainRate = initialGainRate;
        sanityLossRate = initialLossRate;
        equipmentState = initialState;
        inventoryItems.Clear();
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
}