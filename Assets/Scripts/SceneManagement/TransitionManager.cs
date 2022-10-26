/*******************************************************************************************
*
*    File: TransitionManager.cs
*    Purpose: Manages the process of transitioning from one room to another
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using SceneManagement;
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

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void SetSpawnIndex(int index)
    {
        indexToSpawnAt = index;
    }
}