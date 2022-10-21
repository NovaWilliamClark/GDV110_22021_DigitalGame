using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class PooledAudioSource : MonoBehaviour
{
    public AudioSource audioSource;
    private bool hasStarted;
    private bool returned;
    private bool completed;

    public event EventHandler Played;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {

    }

    public void Reset()
    {
        hasStarted = false;
    }

    private void Update()
    {
        if (!hasStarted && audioSource.isPlaying)
        {
            hasStarted = true;
        }

        if (hasStarted && !audioSource.isPlaying)
        {
            OnPlayed();
        }
    }

    protected virtual void OnPlayed()
    {
        Played?.Invoke(this, EventArgs.Empty);
    }
}
