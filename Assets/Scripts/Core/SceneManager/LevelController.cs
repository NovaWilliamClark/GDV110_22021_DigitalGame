﻿/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Character;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LevelController : MonoBehaviour
{
    private PlayerSpawnPoint[] playerSpawnPoints;
    private List<ItemPickup> levelItems = new List<ItemPickup>();
    private List<GameObject> levelEnemies = new List<GameObject>();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelData levelData;

    public AudioClip LevelBGM;
    //public float BGMVolume;

    private void Awake()
    {
        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        levelItems = FindObjectsOfType<ItemPickup>().ToList();
    }

    private void Start()
    {
        InitLevelData();
        InitPlayer();
    }

    private void OnDestroy()
    {
        //levelData.items = FindObjectsOfType<Item>().ToList();
    }

    private void InitLevelData()
    {
        /*if (levelData.items.Count == 0)
        {
            return;
        }*/

        foreach (var obj in levelItems)
        {
            if (obj.GetItem.hasBeenPickedUp)
            {
                Destroy(obj.gameObject);
            }
            /*if (!levelData.items.Contains(obj.gameObject))
            {
                obj.gameObject.SetActive(false);   
            }*/
        }
    }

    private void InitPlayer()
    {
        var sanityMeter = UIHelpers.Instance.SanityMeter;
        // sanityMeter.decreaseSlider = false;
        
        AudioManager.Instance.PlayMusic(LevelBGM);
        
        int spawnIndex = TransitionManager.Instance.GetSpawnIndex;
        Vector2 pos = new Vector2();
        
        PlayerSpawnPoint.FacingDirection direction = PlayerSpawnPoint.FacingDirection.Right;
        
        foreach (PlayerSpawnPoint spawn in playerSpawnPoints)
        {
            if (spawn.GetSpawnIndex == spawnIndex)
            {
                pos = spawn.GetPosition;
                direction = spawn.GetFacingDirection;
                break;
            }
        }

        GameObject player;
        CharacterController cc;
        var existingPlayer = FindObjectOfType<CharacterController>();

        if (!existingPlayer)
        {
            Debug.Log("No player in scene");
            player = Instantiate(playerPrefab);
            player.transform.position = pos;
            //player.transform.position = pos;
        }
        else
        {
            Debug.Log("Player in scene");
            player = existingPlayer.gameObject;
        }

        cc = player.GetComponent<CharacterController>();
        cc.SetIsFlipped(direction == PlayerSpawnPoint.FacingDirection.Left);

        sanityMeter.SetPlayer(cc);
        //playerObj.FetchPersistentData();
        UIHelpers.Instance.Fader.Fade(0f, 2f);
    }
}