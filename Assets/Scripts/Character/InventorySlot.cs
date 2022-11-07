/*******************************************************************************************
*
*    File: InventorySlot.cs
*    Purpose: Represents a slot in the inventory to hold an item.
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 
*
**********************************************************************************************/

using System;
using DG.Tweening;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Character
{
    [Serializable]
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent<InventorySlot> SlotClicked;
        [SerializeField] private TextMeshProUGUI slotText;
        public Button buttonObj { get; private set; }
        public Image itemImage;
        public Image background;
        public Item GetItem => item;
        private Item item;
        private Tween bgTween;

        public Color ActiveColour;
        public Color InactiveColour;
        private bool active = false;

        private void Awake()
        {
            buttonObj = GetComponent<Button>();
            background.GetComponent<RectTransform>().DOScale(0f, 0f);
            bgTween = background.GetComponent<RectTransform>().DOScale(1f, 1f).SetAutoKill(false).Pause();
        }

        private void OnEnable()
        {
            buttonObj.onClick.AddListener(SlotClick);
        }

        private void OnDisable()
        {
            buttonObj.onClick.RemoveListener(SlotClick);
        }

        public void SetItem(Item item)
        {
            this.item = item;
            slotText.text = GetItem.itemName;
        }

        public void SlotClick()
        {
            Debug.Log("CLicked");
            if (!active)
            {
                bgTween.Restart();
                active = true;
            }
            else
            {
                bgTween.PlayBackwards();
                active = false;
            }
            SlotClicked.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            slotText.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            slotText.gameObject.SetActive(false);
        }
    }
}