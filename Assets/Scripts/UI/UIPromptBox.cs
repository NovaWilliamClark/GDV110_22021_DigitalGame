using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIPromptBox : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Sequence animationSequence;
    [SerializeField] private TMP_Text promptText;

    [Header("Animations")]
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeDelay;
    [SerializeField] private Ease easeType;
    
    [SerializeField] private float moveDuration;
    [SerializeField] private float moveDelay;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private Vector2 startPosition;

    public bool isAnimating;
    
    private void Awake()
    {
        rect = canvasGroup.GetComponent<RectTransform>();
        startPosition = rect.anchoredPosition;
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        rect.anchoredPosition = endPosition;
        canvasGroup.alpha = 0f;
        animationSequence = DOTween.Sequence();
        animationSequence.SetAutoKill(false);
        animationSequence
            .Insert(moveDelay, rect.DOAnchorPos(startPosition, moveDuration))
            .Insert(fadeDelay, canvasGroup.DOFade(1f, fadeDuration))
            .SetEase(easeType);
        animationSequence.Pause();
    }

    public void Show(string textToShow)
    {
        promptText.text = textToShow;
        isAnimating = true;
        animationSequence.OnComplete(() => isAnimating = false)
            .Restart();
    }

    public void Hide()
    {
        var wait = 0f;
        if (animationSequence.active)
        {
            wait = animationSequence.Duration() - animationSequence.position;
        }

        StartCoroutine(WaitThen(wait, () =>
        {
            isAnimating = true;
            animationSequence
                .OnRewind(() =>
                {
                    gameObject.SetActive(false);
                    isAnimating = false;
                })
                .PlayBackwards();
        }));

    }

    public IEnumerator WaitThen(float delay, UnityAction callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
