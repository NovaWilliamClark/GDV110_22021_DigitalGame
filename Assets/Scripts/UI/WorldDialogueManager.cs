/*******************************************************************************************
*
*    File: WorldDialogueManager.cs
*    Purpose: Controls the audio
*    Author: Joshua Stephens
*    Date: 25/10/2022
*
**********************************************************************************************/


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WorldDialogueManager : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;
        
        private Queue<string> sentences = new Queue<string>();

        public void StartDialogue(Dialogue dialogue)
        {
            sentences.Clear();

            nameText.text = dialogue.name;
            
            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }
    
        private void DisplayNextSentence()
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            string sentence = sentences.Dequeue();
            dialogueText.text = sentence;
        }

        public void EndDialogue()
        {
            nameText.text = "";
            dialogueText.text = "";
            sentences.Clear();
        }
    }
}