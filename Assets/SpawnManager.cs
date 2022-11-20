using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private SpawnData currentSpawnData = null;

    public bool HasSpawnPoint => currentSpawnData != null;
    public SpawnData CurrentSpawnPoint => currentSpawnData; 
    public bool PlayerDied { get; private set; }

    private List<LevelData_SO> currentLevelData;

    // if respawning we need to grab the level data and player data at time of spawn


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentSpawnData = null;
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
        var lvl = levelData.CreateCopy();
        var player = playerReference.PlayerData.CreateCopy();
        player.ResetSanity(); // don't respawn with low sanity
        //AddCurrentLevelData(sceneName, lvl);
        currentSpawnData.Initialize(nightlightPoId, sceneName, lvl,player);
    }

    public void DoSpawn()
    {
        currentPlayerData = currentSpawnData.PlayerDataAtSpawn;
        
        // loop over level states- remove any that are timestamped after this spawn point was set
        foreach (var dataSo in currentLevelData.ToList())
        {
            if (dataSo.sceneName == currentSpawnData.LevelDataAtSpawn.sceneName) continue;
            if (dataSo.createdAt > currentSpawnData.LevelDataAtSpawn.createdAt)
            {
                currentLevelData.Remove(dataSo);
            }
        }
    }

    public PlayerData_SO GetPlayerData()
    {
        if (PlayerDied)
        {
            currentPlayerData.BatteryValueChanged.RemoveAllListeners();
            currentPlayerData = currentSpawnData.PlayerDataAtSpawn;
            return currentSpawnData.PlayerDataAtSpawn;
        }

        return currentPlayerData;
    }

    public void AddCurrentLevelData(string sceneName, LevelData_SO data)
    {
        if (HasLevelData(sceneName))
        {
            var pos = currentLevelData.FindIndex(i => i.sceneName == sceneName);
            currentLevelData.RemoveAt(pos);
        }
        currentLevelData.Add(data);
    }

    public LevelData_SO GetCurrentLevelData(string sceneName)
    {
        return currentLevelData.FirstOrDefault(dataSo => dataSo.sceneName == sceneName);
    }

    public bool HasLevelData(string sceneName)
    {
        if (currentLevelData.Any(dataSo => dataSo.sceneName == sceneName))
        {
            return true;
        }

        return false;
    }

    private void SetupPlayerData()
    {
        
    }

    public void SetCurrentLevelData(string sceneName, LevelData_SO data)
    {
        if (HasLevelData(sceneName))
        {
            //currentLevelData[sceneName] = data;
        }
        else
        {
            AddCurrentLevelData(sceneName, data);
        }
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

    public void ResetState()
    {
        currentLevelData.Clear();
        playerReference = null;
        currentSpawnData = null;
        PlayerDied = false;
        
        initialSpawnData = new SpawnData();
        currentPlayerData = Instantiate(originalPlayerData);
        currentPlayerData.Init();
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