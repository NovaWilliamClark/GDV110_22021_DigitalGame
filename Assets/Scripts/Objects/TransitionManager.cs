/*******************************************************************************************
*
*    File: TransitionManager.cs
*    Purpose: Manages the process of transitioning from one room to another
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects
{
    public static TransitionManager Instance { get; private set; }
    public int GetSpawnIndex => indexToSpawnAt;
    private int indexToSpawnAt;
    private LevelData data;
    
    private void Awake()
    {
        if (Instance == null)
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
            AudioManager.Instance.Cleanup();
            SceneManager.LoadScene(sceneToLoad);
        }
    
        public void SetSpawnIndex(int index)
        {
            indexToSpawnAt = index;
        }
    }
}