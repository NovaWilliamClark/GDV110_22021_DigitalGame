/*******************************************************************************************
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
using AI;
using Audio;
using Character;
using Cinemachine;
using Core;
using Core.SceneManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelController : MonoBehaviour
{
    private PlayerSpawnPoint[] playerSpawnPoints;

    //private List<ItemPickup> levelItems = new List<ItemPickup>();
    private List<ItemUse> levelItemInteractions = new List<ItemUse>();
    private List<GameObject> levelEnemies = new List<GameObject>();
    [SerializeField] private GameObject playerPrefab;
    [FormerlySerializedAs("levelData")] [SerializeField] private LevelData_SO levelDataSo;
    private CharacterController instancedPlayer;
    public bool safeZone;

    public LevelCutscene onLoadCutscene;

    public AudioClip LevelBGM;
    //public float BGMVolume;

    private Dictionary<string, Nightlight> nightlights = new();
    private Dictionary<int, ItemPickup> levelItems = new();

    [SerializeField] private PlayerData_SO playerDataRef;

    public UnityEvent<CharacterController> PlayerSpawned;
    public UnityEvent LevelInitialized;

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
        if (onLoadCutscene && !levelDataSo.levelCutscenePlayed)
        {
            UIHelpers.Instance.Fader.Fade(0f, 0.1f);
        }
        LevelInit();
    }

    private void OnCutsceneCompleted()
    {
        levelDataSo.levelCutscenePlayed = true;
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
        var persistentObjects = FindObjectsOfType<PersistentObject>();

        // Interates over all the objects in scene with a Persistent Object component
        // Will either:
        //      a - set the state based on the type of component if the level has already been visited
        //      b - initialize the object state to defaults on first level load
        foreach (var po in persistentObjects)
        {
            var interaction = po.GetComponent<InteractionPoint>();
            var genericObject = po.GetComponent<GenericObject>();
            var container = po.GetComponent<ContainerInventory>();

            // TODO: Make SM01 and other mobs inherit from same base class
            var mob = po.GetComponent<SM01>();

            #region Set States
            // Level is already initialized: get the object state depending on the MonoBehaviour type
            // Tell the persistent object to set it's state
            if (levelDataSo.Initialized)
            {
                if (interaction)
                {
                    if (levelDataSo.levelInteractions.GetObjectState(po).State is InteractionState data)
                    {
                        interaction.SetInteractedState(data);
                    }
                }
                if (genericObject)
                {
                    if (levelDataSo.levelGenericObjects.GetObjectState(po).State is GenericState data)
                    {
                        genericObject.SetPersistentState(data);
                    }
                }
                if (container)
                {
                    if (levelDataSo.levelContainers.GetObjectState(po).State is ItemContainerState data)
                        container.SetContainerState(data);
                }
                if (mob)
                {
                    if (levelDataSo.levelEnemies.GetObjectState(po).State is EnemyLevelState data)
                    {
                        mob.SetEnemyState(data);
                    }
                }
            }
            // Initialize object state in Level Data SO
            else
            {
                if (interaction) levelDataSo.levelInteractions.AddObjectState(po);
                if (genericObject) levelDataSo.levelGenericObjects.AddObjectState(po);
                if (container) levelDataSo.levelContainers.AddObjectState(po);
                if (mob) levelDataSo.levelEnemies.AddObjectState(po);
            }
            #endregion

            #region State listeners
            if (interaction)
            {
                interaction.Interacted.AddListener(OnInteractionPointInteracted);
                if (interaction is Nightlight nightlight)
                {
                    nightlights.Add(po.Id, nightlight);
                }
            }
            
            if (genericObject)
            {
                genericObject.ObjectStateChanged.AddListener(OnGenericObjectStateChanged);
            }

            if (container)
            {
                // container listen for item taken
                container.ContainerStateChanged.AddListener(OnContainerStateChanged);
            }

            if (mob)
            {
                mob.EnemyStateChanged.AddListener(OnEnemyStateChanged);
            }
            #endregion

        }

        // Initialize level data to default state
        if (!levelDataSo.Initialized)
        {
            levelDataSo.Setup();
        }
        
        // Inform objects that want to do all the things before player is spawned
        LevelInitialized?.Invoke();
    }

    private void OnEnemyStateChanged(PersistentObject po, EnemyLevelState state)
    {
        levelDataSo.levelEnemies.SetObjectState(po, state);
    }

    private void OnContainerStateChanged(ContainerInventory container, ItemContainerState state)
    {
        var po = container.GetComponent<PersistentObject>();
        levelDataSo.levelContainers.SetObjectState(po, state);
    }

    private void OnGenericObjectStateChanged(GenericObject ge, GenericState state)
    {
        var po = ge.GetComponent<PersistentObject>();
        levelDataSo.levelGenericObjects.SetObjectState(po, state);
    }
 
    private void OnInteractionPointInteracted(InteractionPoint interactionPoint, InteractionState state)
    {
        var po = interactionPoint.GetComponent<PersistentObject>();
        if (interactionPoint is Nightlight)
        {
            Debug.LogFormat("Player spawn set from {0}: {1}", po.name, po.Id);
            playerDataRef.spawnPoint.Set(po.Id, SceneManager.GetActiveScene().name);
        }
        levelDataSo.levelInteractions.SetObjectState(po,state);
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
        else if (existingPlayer && !levelDataSo.Initialized)
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

        if (onLoadCutscene && !levelDataSo.levelCutscenePlayed)
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