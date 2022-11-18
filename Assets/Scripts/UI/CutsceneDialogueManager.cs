using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Cinemachine;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneDialogueManager : MonoBehaviour
{
    public static CutsceneDialogueManager Instance { get; private set; }
    
    // Public variables
    private CutsceneDialogueEntry cutsceneDialogue;
    public Pointer DialoguePointer;

    // Private variables
    private string currentSentence;
    private string currentText;
    private int sentenceCounter = 0;
    private UnityAction onCompleteCallback;
    public GameObject dialogueBoxPrefab;
    private CanvasGroup dialogueBox;
    private UICutsceneDialogue uiCutsceneDialogue;
    private bool shown = false;

    public CinemachineVirtualCamera vcam;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Set reference to dialogue box
        //dialogueBox = this.GetComponent<CanvasGroup>();
    }

    void CreateDialogueBox()
    {
        var box = Instantiate(dialogueBoxPrefab);
        uiCutsceneDialogue = box.GetComponent<UICutsceneDialogue>();
        dialogueBox = box.GetComponentInChildren<CanvasGroup>();
        dialogueBox.alpha = 0;
        dialogueBox.gameObject.SetActive(false);
    }


    public void ShowDialogue(CutsceneDialogueEntry dialogue, UnityAction callback)
    {
        if (!dialogueBox)
        {
            CreateDialogueBox();
        }

        dialogueBox.gameObject.SetActive(true);
        cutsceneDialogue = dialogue;
        onCompleteCallback = callback;
        DialogueResume();
    }
    
    IEnumerator ShowText()
    {
        // Iterate through each letter
        for (int i = 0; i < currentSentence.Length+1; i++)
        {
            // Create a substring for each letter and add it to the text, playing a sound and then wait
            currentText = currentSentence.Substring(0, i);
            dialogueBox.GetComponentInChildren<TextMeshProUGUI>().text = currentText;
            AudioManager.Instance.PlaySound(cutsceneDialogue.speaker.SpeakerAudioClip);
            yield return new WaitForSeconds(cutsceneDialogue.textSpeed);
        }

        yield return new WaitForSeconds(cutsceneDialogue.holdDuration);
        
        // Set the dialogue box to be invisible, will trigger again when the dialogue resumes
        dialogueBox.DOFade(0f, .25f).OnComplete(() =>
        {
            shown = false;
            onCompleteCallback?.Invoke();
        });
    }
    
    // Not entirely sure how the EventHandler will link to the DialogueResume function, will need clarification
    public void DialogueResume()
    {
        // Reset the text (might need ot do something else?)
        dialogueBox.GetComponentInChildren<TextMeshProUGUI>().text = "";

        // If there are still sentences
        if (!shown)
        {
            // Load the next sentence and show the dialogue
            currentSentence = cutsceneDialogue.sentence;
            Debug.Log(currentSentence);
            uiCutsceneDialogue.Init(cutsceneDialogue.speaker, cutsceneDialogue.iconPosition);
            
            dialogueBox.DOFade(1f, .25f).OnComplete(() =>
            {
                //sentenceCounter += 1;
                StartCoroutine(ShowText());
            });
        }
    }
}
