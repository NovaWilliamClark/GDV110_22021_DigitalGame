/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using Audio;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private PlayerSpawnPoint[] playerSpawnPoints;
    [SerializeField] private GameObject playerPrefab;

    public AudioClip LevelBGM;
    public float BGMVolume;

    private void Awake()
    {
        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
    }

    private void Start()
    {
        InitPlayer();
    }

    private void InitPlayer()
    {
        var sanityMeter = FindObjectOfType<SanityMeter>();
        sanityMeter.decreaseSlider = false;
        
        AudioManager.Instance.PlayMusic(LevelBGM, BGMVolume);
        
        int spawnIndex = TransitionManager.Instance.GetSpawnIndex;
        Vector2 pos = new Vector2();
        
        PlayerSpawnPoint.FacingDirection direction = PlayerSpawnPoint.FacingDirection.Left;
        
        foreach (PlayerSpawnPoint spawn in playerSpawnPoints)
        {
            if (spawn.GetSpawnIndex == spawnIndex)
            {
                pos = spawn.GetPosition;
                direction = spawn.GetFacingDirection;
                break;
            }
        }

        var existingPlayer = FindObjectOfType<CharacterController>();

        if (!existingPlayer)
        {
            var player = Instantiate(playerPrefab);
            var playerController = player.GetComponent<CharacterController>();
            playerController.SetIsFlipped(direction == PlayerSpawnPoint.FacingDirection.Left);
            player.transform.position = pos;
        }
        
        sanityMeter.Init();
        //playerObj.FetchPersistentData();
    }
}