/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private PlayerSpawnPoint[] playerSpawnPoints;
    [SerializeField] private GameObject playerPrefab;
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
        
        foreach (PlayerSpawnPoint spawn in playerSpawnPoints)
        {
            if (spawn.GetSpawnIndex == spawnIndex)
            {
                pos = spawn.GetPosition;
                break;
            }
        }

        var existingPlayer = FindObjectOfType<CharacterController>();

        GameObject player;
        if (existingPlayer)
        {
            player = existingPlayer.gameObject;
        } else
        {
            player = Instantiate(playerPrefab);
            player.transform.position = pos;
        }
        
        var playerObj = player.GetComponent<CharacterController>();
        sanityMeter.SetPlayer(playerObj);
        playerObj.FetchPersistentData();
    }
}