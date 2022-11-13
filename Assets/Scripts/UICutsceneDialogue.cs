using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICutsceneDialogue : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Vector2 iconPos;
    private RectTransform iconRect;

    public Vector2 iconLeftPos;
    public Vector2 iconRightPos;

    private void Awake()
    {
        iconRect = icon.GetComponent<RectTransform>();
    }

    public void Init(SpeakerScriptableObject speaker, FaceIconPosition pos)
    {
        if (pos == FaceIconPosition.Left)
        {
            iconPos = iconLeftPos;
        } else if (pos == FaceIconPosition.Right)
        {
            iconPos = iconRightPos;
        }

        iconRect.anchoredPosition = iconPos;
        
        icon.sprite = speaker.speakerSprite;
    }
}
