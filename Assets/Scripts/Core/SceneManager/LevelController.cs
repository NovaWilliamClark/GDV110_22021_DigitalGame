/*******************************************************************************************
*
*    File: LevelData.cs
*    Purpose: Hold data and functionality for each level
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using Audio;
using Character;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
        var sanityMeter = UIHelpers.Instance.SanityMeter;
        // sanityMeter.decreaseSlider = false;
        
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

        GameObject player;
        CharacterController cc;
        var existingPlayer = FindObjectOfType<CharacterController>();

        if (!existingPlayer)
        {
            player = Instantiate(playerPrefab);
            player.transform.position = pos;
        }
        else
        {
            player = existingPlayer.gameObject;
        }

        cc = player.GetComponent<CharacterController>();
        cc.SetIsFlipped(direction == PlayerSpawnPoint.FacingDirection.Left);

        sanityMeter.SetPlayer(cc);
        //playerObj.FetchPersistentData();
        UIHelpers.Instance.Fader.Fade(0f, 2f);
    }
}