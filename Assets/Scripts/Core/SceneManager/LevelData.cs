﻿/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using Audio;
using Character;
using Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.SceneManager
{
    public class LevelData : MonoBehaviour
    {
        [SerializeField] private PlayerSpawnPoint[] playerSpawnPoints;
        [SerializeField] private GameObject playerPrefab;

        [FormerlySerializedAs("LevelBGM")] public AudioClip levelBGM;
        [FormerlySerializedAs("BGMVolume")] public float bgmVolume;
        private void Start()
        {
            InitPlayer();
        }

        private void InitPlayer()
        {
            var sanityMeter = FindObjectOfType<SanityMeter>();
            sanityMeter.decreaseSlider = false;
            AudioManager.Instance.PlayMusic(levelBGM, bgmVolume);
            int spawnIndex = TransitionManager.Instance.GetSpawnIndex;
            Vector2 pos = new Vector2();
        
            foreach (PlayerSpawnPoint spawn in playerSpawnPoints)
            {
                if (spawn.GetSpawnIndex == spawnIndex)
                {
                    pos = spawn.GetPosition;
                    break;
                }
            }
            GameObject player;
            var existingPlayer = FindObjectOfType<CharacterController>();
            if (TransitionManager.Instance.GetSpawnIndex == 0) {
                if (existingPlayer)
                {
                    player = existingPlayer.gameObject;
                } else
                {
                    player = Instantiate(playerPrefab);
                    player.transform.position = pos;
                } 
            }
            else
            {
                if (existingPlayer) Destroy(existingPlayer.gameObject);
                player = Instantiate(playerPrefab);
                player.transform.position = pos;
            }
        
            var playerObj = player.GetComponent<CharacterController>();
            sanityMeter.SetPlayer(playerObj);
            playerObj.FetchPersistentData();
        }
    }
}