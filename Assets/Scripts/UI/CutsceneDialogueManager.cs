using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneDialogueManager : MonoBehaviour
{
    // Public variables
    public CutsceneDialogue cutsceneDialogue;
    public Pointer DialoguePointer;

    // Private variables
    private string currentSentence;
    private string currentText;
    private int sentenceCounter = 0;
    private UnityAction onCompleteCallback;
    private CanvasGroup dialogueBox;

    void Start()
    {
        // Set reference to dialogue box
        dialogueBox = this.GetComponent<CanvasGroup>();
    }

    public void ShowDialogue(CutsceneDialogue dialogue, UnityAction callback)
    {
        cutsceneDialogue = dialogue;
        onCompleteCallback = callback;
        DialogueResume();
    }
    
    IEnumerator ShowText()
    {
        // Iterate through each letter
        for (int i = 0; i < currentSentence.Length; i++)
        {
            // Create a substring for each letter and add it to the text, playing a sound and then wait
            currentText = currentSentence.Substring(0, i);
            this.GetComponent<TextMeshProUGUI>().text = currentText;
            AudioManager.Instance.PlaySound(cutsceneDialogue.speaker.SpeakerAudioClip);
            yield return new WaitForSeconds(cutsceneDialogue.textSpeed);
        }

        // Set the dialogue box to be invisible, will trigger again when the dialogue resumes
        dialogueBox.alpha = 0;
        onCompleteCallback.Invoke();
    }
    
    // Not entirely sure how the EventHandler will link to the DialogueResume function, will need clarification
    public void DialogueResume()
    {
        // Reset the text (might need ot do something else?)
        this.GetComponent<TextMeshProUGUI>().text = "";

        // If there are still sentences
        if (cutsceneDialogue.sentences.Length > sentenceCounter)
        {
            // Load the next sentence and show the dialogue
            currentSentence = cutsceneDialogue.sentences[sentenceCounter];
            dialogueBox.alpha = 1;
            sentenceCounter += 1;
            StartCoroutine(ShowText());
        }
    }
}
