using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class UIWorldDialogue : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text textBox;

        private Sequence sequence;

        public void Init()
        {
            canvasGroup.alpha = 0f;
            textBox.text = "";
        }
        
        public void PrintText(string newText, Vector3 newPosition, 
            float holdDuration, float fadeDuration, UnityAction onComplete = null)
        {
            sequence = DOTween.Sequence();
            gameObject.transform.position = newPosition;
            textBox.text = newText;

            sequence
                .Append(canvasGroup.DOFade(1f, fadeDuration))
                .AppendInterval(holdDuration)
                .Append(canvasGroup.DOFade(0f, fadeDuration))
                .OnComplete(() =>
                {
                    canvasGroup.alpha = 0;
                    onComplete?.Invoke();
                });
            sequence.Play();
        }
    }
}