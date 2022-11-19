﻿/*******************************************************************************************
*
*    File: TransitionManager.cs
*    Purpose: Manages the process of transitioning from one room to another
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    public int GetSpawnIndex => indexToSpawnAt;
    private int indexToSpawnAt = 0;
    private LevelController _controller;
    public UnityEvent ev;
    
    [Header("Scene Stuff")] 
    public bool isChangingScenes = false;
    public string previousScene;
    public string transitionInteractable;

    public List<LevelData_SO> AllLevels;

    [SerializeField] private PlayerData_SO originalPlayerData;


    public UnityEvent<string> SceneChanged;

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

    public void GetTargetSpawn()
    {
        
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: Move this into a GameManager for when the level is ready as things might still be loading
        
    }

    public void LoadScene(string sceneToLoad)
    {
        AudioManager.Instance.Cleanup();
        DOTween.KillAll();

        previousScene = SceneManager.GetActiveScene().name;
        isChangingScenes = true;
        
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            originalPlayerData.ResetData();
            originalPlayerData = originalPlayerData.Copy();
            foreach (var level in AllLevels)
            {
                level.MainMenu();
            }
        }
        
        SceneChanged?.Invoke(sceneToLoad);
        
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void SetSpawnIndex(int index)
    {
        indexToSpawnAt = index;
    }

    public void Respawn()
    {
        
    }
}