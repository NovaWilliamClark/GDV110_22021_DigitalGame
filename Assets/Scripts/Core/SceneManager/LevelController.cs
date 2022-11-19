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
using AI;
using Audio;
using Character;
using Core;
using Core.SceneManager;
using DG.Tweening;
using Objects;
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
    [FormerlySerializedAs("levelDataSo")] [FormerlySerializedAs("levelData")] [SerializeField] private LevelData_SO levelDataOriginal;
    private LevelData_SO currentLevelData;
    private CharacterController instancedPlayer;
    public bool safeZone;

    public LevelData_SO LevelDataOriginal => levelDataOriginal;
    
    public LevelCutscene onLoadCutscene;

    public AudioClip LevelBGM;
    //public float BGMVolume;

    private Dictionary<string, Nightlight> nightlights = new();
    private Dictionary<int, ItemPickup> levelItems = new();

    public UnityEvent<CharacterController> PlayerSpawned;
    public UnityEvent LevelInitialized;

    private SceneTransition sceneTransitionTarget;
    

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
        if (SpawnManager.Instance.PlayerDied)
        {
            SpawnManager.Instance.DoSpawn();
            currentLevelData = SpawnManager.Instance.CurrentSpawnPoint.LevelDataAtSpawn;
        }
        else
        {
            var sceneName = SceneManager.GetActiveScene().name;
            // does spawn manager have initial level data??
            if (!SpawnManager.Instance.HasLevelData(sceneName))
            {
                currentLevelData = Instantiate(levelDataOriginal);
                currentLevelData.Init();
                SpawnManager.Instance.AddCurrentLevelData(sceneName, currentLevelData);
            } else
            {
                currentLevelData = SpawnManager.Instance.GetCurrentLevelData(sceneName);
            }

        }
        
        LevelInit();
    }

    private void OnCutsceneCompleted()
    {
        currentLevelData.levelCutscenePlayed = true;
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
        if (onLoadCutscene && !currentLevelData.levelCutscenePlayed)
        {
            UIHelpers.Instance.Fader.Fade(0, 0f, () =>
            {
                onLoadCutscene.gameObject.SetActive(true);
                AudioManager.Instance.StopMusic();
                onLoadCutscene.Completed.AddListener(OnCutsceneCompleted);
                onLoadCutscene.Play();
            });
        }
        else
        {
            if (onLoadCutscene)
                onLoadCutscene.gameObject.SetActive(false);
            if (UIHelpers.Instance.Fader.IsOpaque())
                UIHelpers.Instance.Fader.Fade(0f, 2f);
        }

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
            var mob = po.GetComponent<Enemy>();

            #region Set States
            // Level is already initialized: get the object state depending on the MonoBehaviour type
            // Tell the persistent object to set it's state
            if (currentLevelData.Initialized)
            {
                if (interaction)
                {
                    var state = currentLevelData.persistentObjectData.GetObjectState(po).State;
                    if (state is InteractionState data)
                    {
                        if (interaction is BreakerObject)
                        {
                            data = state as BreakerState;
                        }
                        // we only want to set persistent data if the existing data matches our desired type
                        interaction.SetInteractedState(data);
                    }
                    else
                    {
                        DebugPersistence(interaction);
                    }
                }
                if (genericObject)
                {
                    if (currentLevelData.persistentObjectData.GetObjectState(po)?.State is GenericState data)
                    {
                        genericObject.SetPersistentState(data);
                    }
                    else
                    {
                        DebugPersistence(genericObject);
                    }
                }
                if (container)
                {
                    if (currentLevelData.persistentObjectData.GetObjectState(po)?.State is ItemContainerState data)
                    {
                        container.SetContainerState(data);
                    }
                    else
                    {
                        DebugPersistence(container);
                    }
                }
                if (mob)
                {
                    if (currentLevelData.persistentObjectData.GetObjectState(po)?.State is EnemyLevelState data)
                    {
                        mob.SetEnemyState(data);
                    }
                    else
                    {
                        DebugPersistence(mob);
                    }
                }
            }
            // Initialize object state in Level Data SO
            else
            {
                if (interaction) currentLevelData.persistentObjectData.AddObjectState(po);
                if (genericObject) currentLevelData.persistentObjectData.AddObjectState(po);
                if (container) currentLevelData.persistentObjectData.AddObjectState(po);
                if (mob) currentLevelData.persistentObjectData.AddObjectState(po);
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
        if (!currentLevelData.Initialized)
        {
            currentLevelData.CompleteInitialization();
        }
        
        // Inform objects that want to do all the things before player is spawned
        LevelInitialized?.Invoke();
    }

    private void DebugPersistence(Component component)
    {
        Debug.LogWarningFormat("Trying to set persistent state for {0} but LevelData {1} returning Null ref.\b Has state been set? Will be null when default PersistentObjectState", component.gameObject.name, levelDataOriginal.name);
    }

    private void OnEnemyStateChanged(PersistentObject po, EnemyLevelState state)
    {
        currentLevelData.persistentObjectData.SetObjectState(po, state);
    }

    private void OnContainerStateChanged(ContainerInventory container, ItemContainerState state)
    {
        var po = container.GetComponent<PersistentObject>();
        currentLevelData.persistentObjectData.SetObjectState(po, state);
    }

    private void OnGenericObjectStateChanged(GenericObject ge, GenericState state)
    {
        var po = ge.GetComponent<PersistentObject>();
        currentLevelData.persistentObjectData.SetObjectState(po, state);
    }
 
    private void OnInteractionPointInteracted(InteractionPoint interactionPoint, InteractionState state)
    {
        var po = interactionPoint.GetComponent<PersistentObject>();
        if (interactionPoint is Nightlight)
        {
            Vector2 ipTrans = interactionPoint.transform.position;
            Debug.LogFormat("Player spawn set in Scene: {0} at (x:{2} y:{3}): {1} - {4}", SceneManager.GetActiveScene().name, po.name, ipTrans.x, ipTrans.y, po.Id);
            //var lvlData = Instantiate(currentLevelData);
            SpawnManager.Instance.SetSpawnPoint(po.Id, SceneManager.GetActiveScene().name, currentLevelData);
                //playerDataRef.spawnPoint.Set(po.Id, SceneManager.GetActiveScene().name);
        }
        currentLevelData.persistentObjectData.SetObjectState(po,state);
        //SpawnManager.Instance.SetCurrentLevelData(SceneManager.GetActiveScene().name, currentLevelData);
    }

    private void InitPlayer()
    {
        var sanityMeter = UIHelpers.Instance.SanityMeter;
        // sanityMeter.decreaseSlider = false;
        
        AudioManager.Instance.PlayMusic(LevelBGM);

        Vector2 pos = new Vector2();
        PlayerSpawnPoint.FacingDirection direction = PlayerSpawnPoint.FacingDirection.Right;

        var existingPlayer = FindObjectOfType<CharacterController>();
        if (existingPlayer)
            Destroy(existingPlayer.gameObject);
        
        // set up spawn
        var tmi = TransitionManager.Instance;
        if (tmi.isChangingScenes && tmi.previousScene != "MainMenu")
        {
            tmi.isChangingScenes = false;
            foreach (var sceneTransition in FindObjectsOfType<SceneTransition>())
            {
                if (sceneTransition.TargetScene == tmi.previousScene)
                {
                    sceneTransitionTarget = sceneTransition;
                    break;
                }
            }

            if (sceneTransitionTarget)
            {
                pos = sceneTransitionTarget.SpawnPosition;
                direction = sceneTransitionTarget.SpawnFacingDirection;
            }
            else
            {
                Debug.LogWarningFormat("Changing scene set but cannot find spawn location");
            }
        }

        var spawnMan = SpawnManager.Instance;

        instancedPlayer = Instantiate(playerPrefab).GetComponent<CharacterController>();
        instancedPlayer.Init(spawnMan.GetPlayerData());
        
        if (spawnMan.PlayerDied)
        {
            if (SceneManager.GetActiveScene().name == spawnMan.CurrentSpawnPoint.SceneName)
            {
                Debug.Log("Player has died, respawning");
                var spawnNl = nightlights[spawnMan.CurrentSpawnPoint.Id];
                pos = spawnNl.transform.position;
            }
        }
        
        spawnMan.SetCurrentLevel(this, instancedPlayer);
        
        instancedPlayer.transform.position = pos;
        instancedPlayer.SetIsFlipped(direction == PlayerSpawnPoint.FacingDirection.Left);
        instancedPlayer.onDeath.AddListener(OnPlayerDeath);

        var sanity = instancedPlayer.GetComponent<CharacterSanity>();
        sanity.Init(instancedPlayer.PlayerData);
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
        //playerObj.FetchPersistentData();
    }

    private void OnPlayerDeath()
    {
        UIHelpers.Instance.Fader.Fade(1f, 3f, () =>
        {
            UIHelpers.Instance.SanityMeter.UnsetPlayer();
            UIHelpers.Instance.SanityMeter.Disable();
            SpawnManager.Instance.SetPlayerDied(true);
            instancedPlayer = null;
            StartCoroutine(WaitALittleBit());
        });
    }

    public IEnumerator WaitALittleBit()
    {
        yield return new WaitForSeconds(2f);
        TransitionManager.Instance.LoadScene(SpawnManager.Instance.CurrentSpawnPoint.SceneName);
    }
}