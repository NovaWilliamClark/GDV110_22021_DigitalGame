using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class UIMenu : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool shown;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float showAlpha = 0.8f;

    public virtual void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0f;
    }
    
    public virtual void Awake()
    {
        
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
        
    }

    public virtual void Start()
    {

    }

    public virtual void Show(UnityAction callback = null)
    {
        canvasGroup.DOFade(showAlpha, fadeDuration).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            callback?.Invoke();
        });
    }

    public virtual void Hide(UnityAction callback = null)
    {
        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            callback?.Invoke();
        });
    }
}
