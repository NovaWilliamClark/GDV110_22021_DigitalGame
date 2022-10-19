using System;
using UnityEngine;
using UnityEngine.Rendering.UI;

[CreateAssetMenu(menuName = "Create PlayerData", fileName = "PlayerData", order = 0)]
public class PlayerData_SO : ScriptableObject
{
    [Header("--Sanity--")] 
    public float sanity = 100f;
    public float sanityGainRate = 0.025f;
    public float sanityLossRate = 0.03f;

    private void OnEnable()
    {
        sanity = 100f;
        sanityGainRate = 0.025f;
        sanityLossRate = 0.03f;
    }
}