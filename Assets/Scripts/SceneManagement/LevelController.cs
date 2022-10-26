/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using UnityEngine;

public class LevelController : MonoBehaviour
{
    private PlayerSpawnPoint[] playerSpawnPoints;
    [SerializeField] private GameObject playerPrefab;

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

        var player = Instantiate(playerPrefab);
        var playerObj = player.GetComponent<CharacterController>();
        playerObj.SetIsFlipped(direction == PlayerSpawnPoint.FacingDirection.Left);
        sanityMeter.Init();
        player.transform.position = pos;
        //playerObj.FetchPersistentData();
    }
}