/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using Audio;
using Objects;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private PlayerSpawnPoint[] playerSpawnPoints;
    [SerializeField] private GameObject playerPrefab;

    public AudioClip LevelBGM;
    public float BGMVolume;
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
        
        foreach (PlayerSpawnPoint spawn in playerSpawnPoints)
        {
            if (spawn.GetSpawnIndex == spawnIndex)
            {
                pos = spawn.GetPosition;
                break;
            }
        }
        GameObject player;
        var existingPlayer = FindObjectOfType<CharacterController>();
        if (TransitionManager.Instance.GetSpawnIndex == 0) {
            if (existingPlayer)
            {
                player = existingPlayer.gameObject;
            } else
            {
                player = Instantiate(playerPrefab);
                player.transform.position = pos;
            } 
        }
        else
        {
            if (existingPlayer) Destroy(existingPlayer.gameObject);
            player = Instantiate(playerPrefab);
            player.transform.position = pos;
        }
        
        var playerObj = player.GetComponent<CharacterController>();
        sanityMeter.SetPlayer(playerObj);
        playerObj.FetchPersistentData();
    }
}