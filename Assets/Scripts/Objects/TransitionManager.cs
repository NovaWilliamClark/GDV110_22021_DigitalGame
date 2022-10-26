/*******************************************************************************************
*
*    File: TransitionManager.cs
*    Purpose: Manages the process of transitioning from one room to another
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using Audio;
using Core.SceneManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    public int GetSpawnIndex => indexToSpawnAt;
    private int indexToSpawnAt = 0;
    private LevelController _controller;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: Move this into a GameManager for when the level is ready as things might still be loading
        UIHelpers.Instance.Fader.Fade(0f, 1f);
    }

    public void LoadScene(string sceneToLoad)
    {
        AudioManager.Instance.Cleanup();
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void SetSpawnIndex(int index)
    {
        indexToSpawnAt = index;
    }
}