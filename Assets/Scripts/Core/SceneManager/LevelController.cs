﻿/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Audio;
using Character;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private PlayerSpawnPoint[] playerSpawnPoints;

    //private List<ItemPickup> levelItems = new List<ItemPickup>();
    private List<ItemUse> levelItemInteractions = new List<ItemUse>();
    private List<GameObject> levelEnemies = new List<GameObject>();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelData levelData;
    private CharacterController instancedPlayer;
    public bool safeZone;

    public LevelCutscene onLoadCutscene;

    public AudioClip LevelBGM;
    //public float BGMVolume;

    private Dictionary<string, Nightlight> nightlights = new();
    private Dictionary<int, ItemPickup> levelItems = new();

    [SerializeField] private PlayerData_SO playerDataRef;

    public UnityEvent<CharacterController> PlayerSpawned;

    private void Awake()
    {
        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        levelItemInteractions = FindObjectsOfType<ItemUse>().ToList();
        List<ItemUseRecipe> itemUseRecipes = FindObjectsOfType<ItemUseRecipe>().ToList();
        foreach (var recipe in itemUseRecipes)
        {
            levelItemInteractions.Add(recipe);
        }
    }

    private void Start()
    {
        if (onLoadCutscene && !levelData.levelCutscenePlayed)
        {
            UIHelpers.Instance.Fader.Fade(0f, 0.1f);
        }
        LevelInit();
    }

    private void OnCutsceneCompleted()
    {
        levelData.levelCutscenePlayed = true;
        AudioManager.Instance.PlayMusic(LevelBGM);
    }

    private void LevelInit()
    {
        InitLevelData();
        InitPlayer();
        FinaliseInit();
    }

    private void FinaliseInit()
    {
        if (UIHelpers.Instance.Fader.IsOpaque())
            UIHelpers.Instance.Fader.Fade(0f, 2f);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void InitLevelData()
    {
        var interactions = FindObjectsOfType<InteractionPoint>();
        // check that level isn't already init'd

        foreach (var interact in interactions)
        {
            var po = interact.GetComponent<PersistentObject>();
            if (levelData.Initialized)
            {
                if (levelData.HasInteracted(po.Id))
                {
                    //interact
                    interact.SetInteractedState();
                }
            }
            else
            {
                levelData.AddInteraction(po);
            }

            if (interact is Nightlight nightlight)
            {
                nightlights.Add(po.Id, nightlight);
            }
            interact.Interacted.AddListener(OnInteractionPointInteracted);
        }

        var genericObjects = FindObjectsOfType<GenericObject>();
        foreach (var ge in genericObjects)
        {
            var po = ge.GetComponent<PersistentObject>();
            if (levelData.Initialized)
            {
                if (levelData.PersistentObjectActive(po.Id))
                {
                    ge.SetPersistentState();
                }
            }
            else
            {
                levelData.AddPersistentObject(po);
            }
            ge.ObjectStateChanged.AddListener(OnGenericObjectStateChanged);
        }
        
        if (!levelData.Initialized)
        {
            levelData.Setup();
        }
    }

    private void OnGenericObjectStateChanged(GenericObject ge)
    {
        var po = ge.GetComponent<PersistentObject>();
        levelData.SetPersistentObject(po.Id);
    }
 
    private void OnInteractionPointInteracted(InteractionPoint interactionPoint)
    {
        var po = interactionPoint.GetComponent<PersistentObject>();
        if (interactionPoint is Nightlight)
        {
            Debug.Log("Player spawn created");
            playerDataRef.spawnPoint.Set(po.Id, SceneManager.GetActiveScene().name);
        }
        levelData.SetInteraction(po.Id);
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
        var existingPlayer = FindObjectOfType<CharacterController>();

        var spawnPointData = playerDataRef.spawnPoint;
        if (spawnPointData.isSet && spawnPointData.SceneName == SceneManager.GetActiveScene().name)
        {
            Debug.Log("Spawn point is set, spawning there");
            var spawnNl = nightlights[spawnPointData.Id];
            pos = spawnNl.transform.position;
        }
        else if (existingPlayer && !levelData.Initialized)
        {
            pos = existingPlayer.transform.position;
        }

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
            player.transform.position = pos;
        }

        instancedPlayer = player.GetComponent<CharacterController>();
        instancedPlayer.SetIsFlipped(direction == PlayerSpawnPoint.FacingDirection.Left);
        instancedPlayer.onDeath.AddListener(OnPlayerDeath);
        if (!playerDataRef)
        {
            playerDataRef = instancedPlayer.PlayerData;
        }

        var sanity = player.GetComponent<CharacterSanity>();
        sanityMeter.SetPlayer(sanity);
        if (!safeZone)
        {
            sanity.Enable();
        }
        else
        {
            sanity.Disable();
        }
        
        PlayerSpawned?.Invoke(instancedPlayer);

        if (onLoadCutscene && !levelData.levelCutscenePlayed)
        {
            AudioManager.Instance.StopMusic();
            onLoadCutscene.Completed.AddListener(OnCutsceneCompleted);
            onLoadCutscene.Play();
        }
        //playerObj.FetchPersistentData();
    }

    public void SetSpawnPoint(int id)
    {
    }

    private void OnPlayerDeath()
    {
        UIHelpers.Instance.SanityMeter.UnsetPlayer();
        UIHelpers.Instance.SanityMeter.Disable();
        instancedPlayer = null;
        StartCoroutine(WaitALittleBit());
    }

    public IEnumerator WaitALittleBit()
    {
        yield return new WaitForSeconds(2f);
        if (!playerDataRef.spawnPoint.isSet)
        {
            TransitionManager.Instance.LoadScene("Level01_BensBedroom");
            playerDataRef.Sanity = playerDataRef.initialSanity; 
        }
        else
        {
            playerDataRef.SetFromCopy(playerDataRef.spawnPoint.DataAsAtSpawn);
            TransitionManager.Instance.LoadScene(playerDataRef.spawnPoint.SceneName);
        }
    }
}