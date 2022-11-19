/*******************************************************************************************
*
*    File: UIHelpers.cs
*    Purpose: Singleton of UI Helpers - particularly Fading
*    Author: Joshua Taylor
*    Date: 21/10/2022
*
**********************************************************************************************/

using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFader : MonoBehaviour
{
    private Image image;

    private Tween tween;
    public bool IsComplete => tween.IsComplete();

    public bool IsOpaque()
    {
        if (image)
            return image.color.a > 0;

        return false;
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        if (!image)
        {
            image = gameObject.AddComponent<Image>();
        }
    }

    public void Fade(float endValue, float duration, UnityAction onComplete = null)
    {
        image.DOFade(endValue, duration).OnComplete(() =>
        {
            onComplete?.Invoke();

            image.raycastTarget = !(endValue <= 0f);
        });
    }
}
