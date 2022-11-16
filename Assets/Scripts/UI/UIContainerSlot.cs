using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIContainerSlot : MonoBehaviour
{
    private ItemData itemData;
    public ItemData Item => itemData;
    private RectTransform rect;
    

    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    
    public UnityEvent<UIContainerSlot> Clicked;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Init(ItemData slotItemData)
    {
        itemData = slotItemData;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemData.slotSize.x);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemData.slotSize.y);

        icon.sprite = itemData.itemSprite;

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Clicked?.Invoke(this);
    }
}
