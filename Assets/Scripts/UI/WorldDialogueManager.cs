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
using UnityEngine;
using UnityEngine.Pool;
using UI;
using Unity.VisualScripting;

namespace UI
{
    public class WorldDialogueManager : MonoBehaviour
    {
        public static WorldDialogueManager Instance { get; private set; }

        private float fadeDuration;
        private float holdDuration;
        
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

        private Queue<Dialogue> queuedDialogue = new();
        private List<Dialogue> activeDialogue = new();

        private bool prefab1Active = false;

        public GameObject dialogueBoxPrefab;

        private ObjectPool<WorldDialogue> pool;
        public ObjectPool<WorldDialogue> Pool => pool;

        private bool displaying = false;

        void Start()
        {
            pool = new ObjectPool<WorldDialogue>(OnCreateDialogue, OnDialogueBoxFromPool);
        }

        private void OnDialogueBoxFromPool(WorldDialogue obj)
        {
            // enable etc
            //obj.gameObject.SetActive(true);
            obj.Completed.AddListener(OnWorldDialogueCompleted);
            
        }

        private WorldDialogue OnCreateDialogue()
        {
            var obj = Instantiate(dialogueBoxPrefab);
            var dialogue = obj.GetComponent<WorldDialogue>();
            obj.name = "Pooled Dialogue Box";
            return dialogue;
        }

        public WorldDialogue CreateDialogueBox(Dialogue dialogue, int startIndex = 0)
        {
            // TODO: Trigger will pass in which dialogue it's up to
            var obj = pool.Get();
            obj.Init(dialogue, startIndex);
            obj.gameObject.SetActive(false);
            return obj;
        }

        private void OnWorldDialogueCompleted(WorldDialogue wd)
        {
            wd.Completed.RemoveListener(OnWorldDialogueCompleted);
            wd.gameObject.SetActive(false);
        }
    }
}