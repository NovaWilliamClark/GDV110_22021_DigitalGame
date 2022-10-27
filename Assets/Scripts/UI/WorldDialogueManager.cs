/*******************************************************************************************
*
*    File: WorldDialogueManager.cs
*    Purpose: Controls the audio
*    Author: Joshua Stephens
*    Date: 25/10/2022
*
**********************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class WorldDialogueManager : MonoBehaviour
    {
        private float holdDuration;
        private float fadeDuration;

        [SerializeField] public GameObject DialogueBox;

        private DialogueBoxManager dialoguePrefab1;
        private DialogueBoxManager dialoguePrefab2;

        private Queue<string> sentences = new Queue<string>();
        private Queue<Vector3> positions = new Queue<Vector3>();

        private bool prefab1Active = false;

        void Start()
        {
            dialoguePrefab1 = Instantiate(DialogueBox).GetComponent<DialogueBoxManager>();
            dialoguePrefab2 = Instantiate(DialogueBox).GetComponent<DialogueBoxManager>();
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            dialoguePrefab1 = Instantiate(DialogueBox).GetComponent<DialogueBoxManager>();
            dialoguePrefab2 = Instantiate(DialogueBox).GetComponent<DialogueBoxManager>();
        }

        public void StartDialogue(Dialogue dialogue)
        {
            holdDuration = dialogue.holdDuration;
            fadeDuration = dialogue.fadeDuration;

            sentences.Clear();

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            foreach (Vector3 position in dialogue.positions)
            {
                positions.Enqueue(position);
            }

            DisplayNextSentence();
            prefab1Active = true;
        }

        private void DisplayNextSentence()
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            string sentence = sentences.Dequeue();

            Vector3 newPosition = positions.Dequeue();

            if (!prefab1Active)
            {
                dialoguePrefab1.PrintText(sentence, newPosition, holdDuration, fadeDuration);
                prefab1Active = true;
            }
            else
            {
                dialoguePrefab2.PrintText(sentence, newPosition, holdDuration, fadeDuration);
                prefab1Active = false;
            }

            StartCoroutine(NextSentence(holdDuration, fadeDuration));
        }
        
        private IEnumerator NextSentence(float currentHoldDuration, float currentFadeDuration)
        {
            yield return new WaitForSeconds(currentHoldDuration + (currentFadeDuration * 2));

            DisplayNextSentence();
        }
        
        public void EndDialogue()
        {
            sentences.Clear();
            
            if (!prefab1Active)
            {
                prefab1Active = true;
            }
            else
            {
                prefab1Active = false;
            }
        }
    }
}