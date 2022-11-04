/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private PlayerSpawnPoint[] playerSpawnPoints;
    private List<ItemPickup> levelItems = new List<ItemPickup>();
    private List<ItemUse> levelItemInteractions = new List<ItemUse>();
    private List<GameObject> levelEnemies = new List<GameObject>();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelData levelData;
    private CharacterController instancedPlayer;

    public LevelCutscene onLoadCutscene;

    public AudioClip LevelBGM;
    //public float BGMVolume;

    private void Awake()
    {
        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        levelItems = FindObjectsOfType<ItemPickup>().ToList();
        levelItemInteractions = FindObjectsOfType<ItemUse>().ToList();
        List<ItemUseRecipe> itemUseRecipes = FindObjectsOfType<ItemUseRecipe>().ToList();
        foreach (var recipe in itemUseRecipes)
        {
            levelItemInteractions.Add(recipe);
        }
    }

    private void Start()
    {
        if (onLoadCutscene)
        {
            onLoadCutscene.Completed.AddListener(OnCutsceneCompleted);
            onLoadCutscene.Play();
        }
        else
        {
            LevelInit();
        }
    }

    private void OnCutsceneCompleted()
    {
        LevelInit();
    }

    private void LevelInit()
    {
        InitLevelData();
        InitPlayer();
    }

    private void InitLevelData()
    {
        foreach (var obj in levelItems)
        {
            if (obj.GetItem != null)
            {
                if (obj.GetItem.hasBeenPickedUp)
                {
                    Destroy(obj.gameObject);
                }
            }
        }

        foreach (var interaction in levelItemInteractions)
        {
            if (interaction.GetData != null)
            {
                if (interaction.GetData.state == InteractiveData.InteractionState.INACTIVE)
                {
                    Destroy(interaction.gameObject);
                }
            }
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

        instancedPlayer = player.GetComponent<CharacterController>();
        instancedPlayer.SetIsFlipped(direction == PlayerSpawnPoint.FacingDirection.Left);
        instancedPlayer.onDeath.AddListener(OnPlayerDeath);

        sanityMeter.SetPlayer(instancedPlayer);
        //playerObj.FetchPersistentData();
        UIHelpers.Instance.Fader.Fade(0f, 2f);
    }

    private void OnPlayerDeath()
    {
        UIHelpers.Instance.SanityMeter.UnsetPlayer();
        instancedPlayer = null;
    }
}