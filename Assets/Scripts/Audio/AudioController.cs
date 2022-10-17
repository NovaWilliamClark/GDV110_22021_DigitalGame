/*******************************************************************************************
*
*    File: AudioController.cs
*    Purpose: Controls the audio
*    Author: Joshua Stephens
*    Date: 16/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        // members
        public static AudioController instance;

        public bool debug;
        public AudioTrack[] tracks;

        private Hashtable audioTable; // relationship between audio types (key) and audio tracks (value)
        private Hashtable jobTable; // relationship between audio types (key) and jobs (value) (Coroutine, IEnumerator)

        [System.Serializable]
        public class AudioObject
        {
            public AudioType type;
            public AudioClip clip;
        }

        [System.Serializable]
        public class AudioTrack
        {
            public AudioSource source;
            public AudioObject[] audio;
        }

        private class AudioJob
        {
            public AudioAction action;
            public AudioType type;
            public bool fade;
            public float delay;

            public AudioJob(AudioAction newAction, AudioType newType, bool newFade, float newDelay)
            {
                action = newAction;
                type = newType;
                fade = newFade;
                delay = newDelay;
            }
        }

        private enum AudioAction
        {
            START,
            STOP,
            RESTART
        }

#region Unity Functions

        private void Awake()
        {
            if (!instance)
            {
                Configure();
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

#endregion

#region Public Functions

        public void PlayAudio(AudioType type, bool fade=false, float delay=0.0f)
        {
            AddJob(new AudioJob(AudioAction.START, type, fade, delay));
        }
        
        public void StopAudio(AudioType type, bool fade=false, float delay=0.0f)
        {
            AddJob(new AudioJob(AudioAction.STOP, type, fade, delay));
        }
        
        public void RestartAudio(AudioType type, bool fade=false, float delay=0.0f)
        {
            AddJob(new AudioJob(AudioAction.RESTART, type, fade, delay));
        }

#endregion

#region Private Functions

        private void Configure()
        {
            instance = this;
            audioTable = new Hashtable();
            jobTable = new Hashtable();
            GenerateAudioTable();
        }

        private void Dispose()
        {
            foreach (DictionaryEntry entry in jobTable)
            {
                IEnumerator job = (IEnumerator)entry.Value;
                StopCoroutine(job);
            }
        }

        private void GenerateAudioTable()
        {
            foreach (AudioTrack track in tracks)
            {
                foreach (AudioObject obj in track.audio)
                {
                    //do not duplicate keys
                    if (audioTable.ContainsKey(obj.type))
                    {
                        LogWarning("You are trying to register audio ["+obj.type+"] that has already been registered.");
                    }
                    else
                    {
                        audioTable.Add(obj.type, track);
                        Log("Registering audio ["+obj.type+"].");
                    }
                }
            }
        }

        private IEnumerator RunAudioJob(AudioJob job)
        {
            yield return new WaitForSeconds(job.delay);
            
            AudioTrack track = (AudioTrack)audioTable[job.type];
            track.source.clip = GetAudioClipFromAudioTrack(job.type, track);

            switch (job.action)
            {
                case AudioAction.START:
                    track.source.Play(); 
                    break;

                case AudioAction.STOP:
                    if (!job.fade)
                    {
                        track.source.Stop();
                    }
                    break;
                
                case AudioAction.RESTART:
                    track.source.Stop();
                    track.source.Play();
                    break;
            }

            if (job.fade)
            {
                float initial = job.action == AudioAction.START || job.action == AudioAction.RESTART ? 0.0f : 1.0f;
                float target = initial == 0 ? 1 : 0;
                float duration = 1.0f;
                float timer = 0.0f;

                while (timer <= duration)
                {
                    track.source.volume = Mathf.Lerp(initial, target, timer / duration);
                    timer += Time.deltaTime;
                    yield return null;
                }

                if (job.action == AudioAction.STOP)
                {
                    track.source.Stop();
                }
            }
            
            jobTable.Remove(job.type);
            Log("Job count: " + jobTable.Count);

            yield return null;
        }

        private void AddJob(AudioJob job)
        {
            // remove conflicting jobs
            RemoveConflictingJobs(job.type);

            // start job
            IEnumerator jobRunner = RunAudioJob(job);
            jobTable.Add(job.type, jobRunner);
            StartCoroutine(jobRunner);
            Log("Starting job on ["+job.type+"] with operation: " + job.action);
        }

        private void RemoveJob(AudioType type)
        {
            if (!jobTable.ContainsKey(type))
            {
                LogWarning("Trying to stop a job ["+type+"] that is not running.");
                return;
            }

            IEnumerator runningJob = (IEnumerator)jobTable[type];
            StopCoroutine(runningJob);
            jobTable.Remove(type);
        }

        private void RemoveConflictingJobs(AudioType type)
        {
            if (jobTable.ContainsKey(type))
            {
                RemoveJob(type);
            }

            AudioType conflictAudio = AudioType.None;
            
            foreach (DictionaryEntry entry in jobTable)
            {
                AudioType audioType = (AudioType)entry.Key;
                AudioTrack audioTrackInUse = (AudioTrack)audioTable[audioType];
                AudioTrack audioTrackNeeded = (AudioTrack)audioTable[type];

                if (audioTrackNeeded.source == audioTrackInUse.source)
                {
                    // conflict
                    conflictAudio = audioType;
                }
            }

            if (conflictAudio != AudioType.None)
            {
                RemoveJob(conflictAudio);
            }
        }

        public AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track)
        {
            foreach (AudioObject obj in track.audio)
            {
                if (obj.type == type)
                {
                    return obj.clip;
                }
            }

            return null;
        }

        private void Log(string message)
        {
            if (!debug) return;
            Debug.Log("[Audio Controller]: "+ message);
        }
        
        private void LogWarning(string message)
        {
            if (!debug) return;
            Debug.LogWarning("[Audio Controller]: "+ message);
        }
            
#endregion
    }
}
