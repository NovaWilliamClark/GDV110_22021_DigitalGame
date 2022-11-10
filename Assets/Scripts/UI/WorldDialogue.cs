using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;

public class WorldDialogue : MonoBehaviour
{
    private Dialogue dialogueToPlay;
    private int startIndex;
    private int currentIndex;

    private bool playing = false;
    public bool isPlaying => playing;
    private bool paused;
    public bool IsCompleted => currentIndex == dialogueBoxes.Count - 1;
    
    public UnityEvent<WorldDialogue> Completed;

    [SerializeField] private List<GameObject> dialogueBoxes;

    private Queue<UIWorldDialogue> availableBoxes = new();
    public void Init(Dialogue dialogue, int _startIndex = 0)
    {
        Reset();
        dialogueToPlay = dialogue;
        startIndex = _startIndex;
    }
    
    public void StartDialogue()
    {
        currentIndex = startIndex;
        DisplayNextSentence();
    }

    public void Resume()
    {
        paused = false;
    }

    // ends the playing prematurely - to be resumed later
    public void End()
    {
        if (playing && currentIndex != dialogueBoxes.Count)
        {
            paused = true;
        }
    }

    public void DisplayNextSentence()
    {
        if (currentIndex == dialogueBoxes.Count-1)
        {
            Complete();
            return;
        }

        if (paused) return;
        playing = true;
        var db = availableBoxes.Dequeue();
        var dialogue = dialogueToPlay.entries[currentIndex];
        db.PrintText(dialogue.sentence, dialogue.position, 
            dialogue.holdDuration, dialogue.fadeDuration, () =>
            {
                currentIndex++;
                availableBoxes.Enqueue(db);
                DisplayNextSentence();
            });
    }

    private void Complete()
    {
        playing = false;
        Completed?.Invoke(this);
    }
    
    private void Reset()
    {
        // tell both objects to clean up
        foreach (var dialogueBox in dialogueBoxes)
        {
            var db = dialogueBox.GetComponent<UIWorldDialogue>();
            availableBoxes.Enqueue(db);    
        }

        currentIndex = 0;
    }
}
