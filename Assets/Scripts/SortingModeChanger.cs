/*******************************************************************************************
*
*    File: SortingModeChanger.cs
*    Purpose: Allows offsetting the sorting mode - particularly for cutscenes.
*    Author: Josh Taylor
*    Date: 19/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortingModeChanger : MonoBehaviour
{
    [SerializeField] private List<SortingModeData> renderers;

    public int offset = 500;
    private int oldOffset = 0;
    
    void Awake()
    {
        var srs = GetComponentsInChildren<SpriteRenderer>(true).ToList();
        foreach (var renderer in srs)
        {
            renderer.sortingOrder += offset;
            renderers.Add(new SortingModeData
            {
                renderer = renderer, 
                initialValue = renderer.sortingOrder,
                initialLayer = renderer.sortingLayerName
            });
        }
    }

    public void ResetLayer() {}

    public void SetSortingValue(int targetValue)
    {
        oldOffset = offset;
        offset = targetValue;
        foreach (var obj in renderers)
        {
            var diff = obj.renderer.sortingOrder - oldOffset;

            obj.renderer.sortingOrder = offset + diff;
            obj.newValue = obj.renderer.sortingOrder;
        }
    }

    public void Reset()
    {
        foreach (var obj in renderers)
        {
            obj.renderer.sortingOrder = obj.initialValue;
            obj.renderer.sortingLayerID = SortingLayer.NameToID(obj.initialLayer);
            obj.newValue = 0;
            offset = oldOffset;
        }
    }
    
    public void SetLayer(string layer)
    {
        
    }
    
    
}

[Serializable]
public class SortingModeData
{
    public SpriteRenderer renderer;
    public int initialValue;
    public int newValue;
    public string initialLayer;
    public string newLayer;
}