using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIContainerGrabby : MonoBehaviour
{
    [SerializeField] private RectTransform activationArea;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Canvas parentCanvas;
    private bool active;
    private bool hasTarget;

    private Vector2 targetPosition;
    private Vector2 slotTargetPosition;
    private Tween moveTween;

    private float lerpPos = 0;
    [SerializeField] private float lerpSpeed = 5f;
    private Vector2 velocity;
    public bool ReachedTarget;
    private UnityAction ReachedTargetCallback;
    private float startY;
    private bool resetPosition; 
    
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (!activationArea)
        {
            Debug.LogWarningFormat("{0}: Activation area not set", name);
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        
    }

    public void Show()
    {
        
        startY = rect.anchoredPosition.y;
        // animate in
        StartCoroutine(FollowSelector());
    }

    private void Update()
    {
        
    }

    public void SetSlotTarget(Vector2 position, UnityAction completeCallback = null)
    {
        hasTarget = true;
        ReachedTarget = false;
        resetPosition = false;
        targetPosition = position;
        ReachedTargetCallback = completeCallback;
    }

    public void ResetSlotTarget()
    {
        hasTarget = false;
    }

    private IEnumerator FollowSelector()
    {
        while (true)
        {
            var mousePos = Mouse.current.position.ReadValue();
            active = RectTransformUtility.RectangleContainsScreenPoint(activationArea, mousePos);

            if (!hasTarget)
            {
                if (active)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(activationArea,
                        mousePos, null, out var pos);
                    pos.y = rect.anchoredPosition.y;

                    targetPosition = pos;
                }
                else
                {
                    var pos = activationArea.rect.center;
                    pos.y = rect.anchoredPosition.y;

                    targetPosition = pos;
                }
            }
            else
            {
                //Debug.Log(Vector2.Distance(targetPosition, rect.anchoredPosition));
                if (Vector2.Distance(targetPosition, rect.anchoredPosition) < 0.2f)
                {
                    if (!resetPosition)
                    {
                        targetPosition = new Vector2(rect.anchoredPosition.x, startY);
                        resetPosition = true;
                        ReachedTarget = true;
                        ReachedTargetCallback?.Invoke();
                    }
                    else
                    {
                        hasTarget = false;
                    }
                }
            }


            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, targetPosition, ref velocity, lerpSpeed);
            
            yield return null;
        }
    }

    public void Hide()
    {
        
    }
}
