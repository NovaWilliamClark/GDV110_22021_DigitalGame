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
using Audio;
using DG.Tweening;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Character
{
    [ExecuteInEditMode]
    [Serializable]
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent<InventorySlot> SlotClicked;
        [SerializeField] private TextMeshProUGUI slotText;
        public Button buttonObj { get; private set; }
        public Image itemImage;
        public Image background;
        public ItemData GetItemData => itemReference;
        public ItemData itemReference;
        private Tween bgTween;

        private Sequence outlineFlash;


        [Header("Visuals")] [SerializeField] private Image opaqueImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private Vector2 outlineScale;
        [SerializeField] private bool showOutline = false;
        [SerializeField] private Color outlineColour;

        [FormerlySerializedAs("clickSfx")] [Header("Sound")] [SerializeField] private AudioClip activateSfx;
        [SerializeField] private AudioClip deactivateSfx;
        [SerializeField] private float sfxVolume;

        private bool mouseOver = false;

        private bool active = false;

        private void Awake()
        {
            if (!Application.isPlaying) return;
            buttonObj = GetComponent<Button>();
            outlineFlash = DOTween.Sequence().SetAutoKill(false);
            outlineFlash
                .Append(outlineImage.DOFade(1f, .2f))
                .Append(outlineImage.DOFade(0f, .2f))
                .SetLoops(-1);
            outlineFlash.Restart();
            showOutline = false;
            
            //bgTween = background.GetComponent<RectTransform>().DOScale(1f, 1f).SetAutoKill(false).Pause();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying) return;
            ResetSlot();
            buttonObj.onClick.AddListener(SlotClick);
        }

        private void OnDisable()
        {   
            if (!Application.isPlaying) return;
            ResetSlot();
            buttonObj.onClick.RemoveListener(SlotClick);
        }

        private void ResetSlot()
        {
            active = false;
            showOutline = false;
            outlineImage.color = new Color(outlineImage.color.r, outlineImage.color.g, outlineImage.color.b, 0f);
        }

        public void SetItem(ItemData itemData)
        {
            this.itemReference = itemData;
            slotText.text = GetItemData.itemName;
            itemImage.sprite = itemData.itemSprite;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                DoEditorChanges();
            }
            else
            {
                UpdateOutline();
            }
        }

        private void DoEditorChanges()
        {
            UpdateOutline();
        }

        private void UpdateOutline()
        {
            outlineImage.enabled = showOutline;
            var scale = opaqueImage.GetComponent<RectTransform>();
            scale.sizeDelta = outlineImage.GetComponent<RectTransform>().sizeDelta;
            opaqueImage.GetComponent<RectTransform>().localScale =
                new Vector3(1f - outlineScale.x, 1f - outlineScale.y, 0f);
            if (opaqueImage.sprite != outlineImage.sprite)
            {
                opaqueImage.sprite = outlineImage.sprite;
            }
        }

        public void SlotClick()
        {
            if (!buttonObj.interactable) return;
            if (!active)
            {
                active = true;
                outlineFlash.Pause();
                AudioManager.Instance.PlaySound(activateSfx,sfxVolume);
                outlineImage.DOFade(1f, .2f).OnComplete(() =>
                {
                    var seq = DOTween.Sequence();
                    var rect = GetComponent<RectTransform>();
                    var pos = rect.rotation.eulerAngles;
                    seq
                        .Append(rect.DORotate(pos + new Vector3(0f, 0f, 10f), .25f).SetEase(Ease.OutElastic))
                        .Append(rect.DORotate(pos + new Vector3(0f, 0f, -10f), .25f).SetEase(Ease.OutElastic))
                        .Append(rect.DORotate(pos, .25f).SetEase(Ease.InBounce));
                    seq.Play();
                });
            }
            else
            {
                AudioManager.Instance.PlaySound(deactivateSfx,sfxVolume);
                Deactivate();
            }
            SlotClicked.Invoke(this);
        }

        public void Deactivate()
        {
            if (mouseOver)
            {
                outlineFlash.Restart();
            }
            else
            {
                outlineImage.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    showOutline = false;
                });
            }

            active = false;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!buttonObj.interactable) return;
            if (active) return;
            showOutline = true;
            //slotText.gameObject.SetActive(true);
            mouseOver = true;
            outlineFlash.Restart();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!buttonObj.interactable) return;
            outlineFlash.Pause();
            mouseOver = false;
            if (!active)
            {
                outlineImage.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    showOutline = false;
                });
            }
            //slotText.gameObject.SetActive(false);
        }
    }
}