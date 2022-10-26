/*******************************************************************************************
*
*    File: AudioManager.cs
*    Purpose: Audio Management
*    Author: Joshua Taylor
*    Date: 21/10/2022
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        public AudioMixer MainAudioMixer;
        private AudioSource previousBGMSource;
        private AudioSource BGMSource;
        private AudioSource sanityBgmSource;

        public AudioClip sanityBgm;

        [SerializeField] private int poolSize = 20;
        private ObjectPool<AudioSource> pool;
        public ObjectPool<AudioSource> Pool => pool;

        private List<AudioSource> loopedSounds = new();

        private void Awake()
        {
            if (Instance == null)
            {
                var mixerGroup = MainAudioMixer.FindMatchingGroups("Master/BGM").First();
                
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                BGMSource = new GameObject("BGM").AddComponent<AudioSource>();
                BGMSource.loop = true;
                BGMSource.outputAudioMixerGroup = mixerGroup;
                
                sanityBgmSource = BGMSource.gameObject.AddComponent<AudioSource>();
                sanityBgmSource.loop = true;
                sanityBgmSource.outputAudioMixerGroup = mixerGroup;
                sanityBgmSource.clip = sanityBgm; 
                
                previousBGMSource = BGMSource.gameObject.AddComponent<AudioSource>();
                previousBGMSource.loop = true;
                previousBGMSource.outputAudioMixerGroup = mixerGroup;
                DontDestroyOnLoad(BGMSource.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            pool = new ObjectPool<AudioSource>(CreatePoolAudioSource, OnAudioSourceFromPool, OnAudioSourceReleased, OnAudioSourceDestroyed, true, poolSize, 100);
        }

        /// <summary>
        /// Delete pool objects from scene, cleaning up existing references
        /// </summary>
        public void Cleanup()
        {
            if (pool.CountAll > 0)
            {
                pool.Clear();
            }
            //pool = new ObjectPool<AudioSource>(CreatePoolAudioSource, OnAudioSourceFromPool, OnAudioSourceReleased, OnAudioSourceDestroyed, true, 20, 100);
        }

        public void PlaySound(AudioClip clip, float volume = 1f, bool loop = false)
        {
            var src = pool.Get();
            src.clip = clip;
            src.volume = volume;
            src.loop = loop;
            if (loop)
            {
                loopedSounds.Add(src);
            }
            src.Play();
        }

        public void StopLooping(AudioClip clip)
        {
            foreach (var src in loopedSounds)
            {
                if (src.loop && src.clip == clip)
                {
                    src.Stop();
                    loopedSounds.Remove(src); 
                }
            }
        }

        public void PlayMusic(AudioClip clip, float volume = 1f, float fadeInDuration = 1f, float crossfadeDuration = 1f)
        {
            // cross fade current clip
            if (BGMSource.isPlaying)
            {
                previousBGMSource.clip = BGMSource.clip;
                previousBGMSource.time = BGMSource.time;
                previousBGMSource.volume = BGMSource.volume;
                BGMSource.Stop();
                previousBGMSource.Play();
                BGMSource.clip = clip;
                BGMSource.volume = 0f;
                
                previousBGMSource.DOFade(0f, crossfadeDuration);
                BGMSource.Play();
                BGMSource.DOFade(volume, fadeInDuration);
            }
            else
            {
                BGMSource.clip = clip;
                BGMSource.volume = 0f;
                BGMSource.Play();
                BGMSource.DOFade(volume, fadeInDuration);
            }
        }

        public void PlayPreviousMusic()
        {
            PlayMusic(previousBGMSource.clip, previousBGMSource.volume);
        }

        public void FadeMusic(float endValue, float duration, UnityAction onComplete = null)
        {
            BGMSource.DOFade(endValue, duration).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void StopMusic(UnityAction onComplete = null)
        {
            BGMSource.Stop();
            onComplete?.Invoke();
        }

        public void StopMusic(float fadeDuration, UnityAction onComplete = null)
        {
            FadeMusic(0f, fadeDuration, () =>
            {
                StopMusic(onComplete);
            });
        }

        public void PlaySanityBgm(float volume)
        {
            sanityBgmSource.volume = volume;
            BGMSource.volume = 1f - volume;
            if (!sanityBgmSource.isPlaying) sanityBgmSource.Play();
        }

        public void ChangeSanityVolume(float volume)
        {
            sanityBgmSource.volume = volume;
        }

        private void OnAudioSourceDestroyed(AudioSource obj)
        {
            var pas = obj.GetComponent<PooledAudioSource>();
            pas.Played -= OnPoolSourcePlayed;
            Destroy(obj.gameObject);
        }

        private void OnAudioSourceReleased(AudioSource obj)
        {
            var pas = obj.GetComponent<PooledAudioSource>();
            pas.Reset();
            obj.gameObject.SetActive(false);
        }

        private void OnAudioSourceFromPool(AudioSource obj)
        {
            obj.gameObject.SetActive(true);
        }

        private AudioSource CreatePoolAudioSource()
        {
            var go = new GameObject("Pooled Sound Effect");
            var audiosrc = go.AddComponent<AudioSource>();
            audiosrc.outputAudioMixerGroup = MainAudioMixer.FindMatchingGroups("Master/SFX").First();
            var pas = go.AddComponent<PooledAudioSource>();
            pas.Played += OnPoolSourcePlayed;
            return audiosrc;
        }

        private void OnPoolSourcePlayed(object sender, EventArgs e)
        {
            PooledAudioSource src = (PooledAudioSource) sender;
            pool.Release(src.audioSource);
        }
    }
}