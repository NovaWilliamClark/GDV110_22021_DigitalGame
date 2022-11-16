using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Objects;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class UIContainer : MonoBehaviour
{
    private ItemContainer_SO data;

    [SerializeField] private RectTransform slotPrefab;
    private List<UIContainerSlot> itemsContained = new();
    [SerializeField] private UIContainerGrabby grabber;

    [SerializeField] private RectTransform activeArea; 
    private CanvasGroup canvasGroup;

    public UnityEvent Closed;

    public UnityEvent<ItemData> ItemClicked;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(ItemContainer_SO containerData, UnityAction<ItemData> takeItem = null, UnityAction takeAllItems = null)
    {
        var activeRect = activeArea.rect;
        foreach (var item in containerData.initialItems)
        {
            if (containerData.ItemsTaken.Contains(item))
                continue;
            var slot = Instantiate(slotPrefab, activeArea.transform);
            var rect = slot.rect;
            var uiSlot = slot.GetComponent<UIContainerSlot>();
            uiSlot.Init(item);
            uiSlot.Clicked.AddListener(OnSlotClicked);
            
            itemsContained.Add(uiSlot);
            slot.anchoredPosition = GetRandomPosition(activeRect, rect);
            var containerSlot = slot.GetComponent<UIContainerSlot>();
            // for (var index = 0; index < itemsContained.Count; index++)
            // {
            //     var other = itemsContained[index];
            //     if (other != containerSlot)
            //     {
            //         var otherRect = other.GetComponent<RectTransform>();
            //     
            //         if (otherRect.rect.Overlaps(rect))
            //         {
            //             slot.anchoredPosition = GetRandomPosition(activeRect, rect); 
            //             index -= 1;
            //         }
            //         
            //     }
            // }
        }
    }

    private void OnSlotClicked(UIContainerSlot slot)
    {
        Debug.Log("Slot clicked");
        grabber.SetSlotTarget(((RectTransform) slot.transform).anchoredPosition, () =>
        {
            ItemClicked?.Invoke(slot.Item);
            slot.gameObject.SetActive(false);
        });
    }

    public void Open()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        canvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            grabber.Show();
        });
    }

    private Vector2 GetRandomPosition(Rect targetRect, Rect rectToPlace)
    {
        return new Vector2(
            Random.Range(targetRect.min.x + rectToPlace.width / 2f, targetRect.max.x - rectToPlace.width / 2f),
            Random.Range(targetRect.min.y + rectToPlace.height / 2, targetRect.max.y + rectToPlace.height / 2));
    }

    public void Close()
    {
        Cursor.visible = false;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            Closed?.Invoke();
        });
    }
}
