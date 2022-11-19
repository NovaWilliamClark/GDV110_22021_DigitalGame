using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public PlayerData_SO originalPlayerData;
    private PlayerData_SO currentPlayerData;
    [SerializeField] private GameObject playerPrefab;
    public static SpawnManager Instance { get; private set; }
    private CharacterController playerReference;
    public CharacterController PlayerReference => playerReference;
    public LevelController CurrentLevelController { get; private set; }

    [SerializeField] private SpawnData initialSpawnData;
    private SpawnData currentSpawnData;

    public bool HasSpawnPoint => currentSpawnData != null;
    public SpawnData CurrentSpawnPoint => currentSpawnData; 
    public bool PlayerDied { get; private set; }

    private Dictionary<string, LevelData_SO> currentLevelData; 

    // if respawning we need to grab the level data and player data at time of spawn


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            initialSpawnData = new SpawnData();
            currentPlayerData = Instantiate(originalPlayerData);
            currentPlayerData.Init();
            currentLevelData = new();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //TransitionManager.Instance.SceneChanged.AddListener(OnSceneChanged);
    }

    public void SetPlayerDied(bool died)
    {
        PlayerDied = true;
    }

    public void SetSpawnPoint(string nightlightPoId, string sceneName, LevelData_SO levelData)
    {
        // copy of the level data at time of spawn point being set
        // copy the player data at time of spawn point
        currentSpawnData = new SpawnData();
        var lvl = levelData.CreateCopy(levelData);
        var player = Instantiate(playerReference.PlayerData);
        currentLevelData[sceneName] = lvl;
        currentSpawnData.Initialize(nightlightPoId, sceneName, lvl,player);
    }

    public PlayerData_SO GetPlayerData()
    {
        if (PlayerDied)
        {
            return currentSpawnData.PlayerDataAtSpawn;
        }

        return currentPlayerData;
    }

    public void AddCurrentLevelData(string sceneName, LevelData_SO data)
    {
        currentLevelData.Add(sceneName, data);
    }

    public LevelData_SO GetCurrentLevelData(string sceneName)
    {
        return currentLevelData[sceneName];
    }

    public bool HasLevelData(string sceneName)
    {
        return currentLevelData.ContainsKey(sceneName);
    }

    public void SetCurrentLevel(LevelController levelController, CharacterController player)
    {
        CurrentLevelController = levelController;
        playerReference = player;
        levelController.PlayerSpawned.AddListener(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(CharacterController arg0)
    {
        PlayerDied = false;
        CurrentLevelController.PlayerSpawned.RemoveListener(OnPlayerSpawned);
    }
}

[Serializable]
public class SpawnData
{
    [SerializeField] private string id = "";
    public string Id => id;
    [SerializeField] private string sceneName;
    public string SceneName => sceneName;

    
    private PlayerData_SO playerDataInstance;
    private LevelData_SO levelDataInstance;
    
    public PlayerData_SO PlayerDataAtSpawn => playerDataInstance;
    public LevelData_SO LevelDataAtSpawn => levelDataInstance;

    public void Initialize(string PoId, string spawnSceneName, LevelData_SO levelData, PlayerData_SO playerData)
    {
        playerDataInstance = playerData;
        levelDataInstance = levelData;
        sceneName = spawnSceneName;
        id = PoId;
    }
}