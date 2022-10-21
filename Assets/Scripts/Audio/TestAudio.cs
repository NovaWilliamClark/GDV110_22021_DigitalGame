/*******************************************************************************************
*
*    File: TestAudio.cs
*    Purpose: For testing the audio
*    Author: Joshua Stephens
*    Date: 16/10/2022
*
**********************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class TestAudio : MonoBehaviour
    {
        public AudioController audioController;
        
#region Unity Functions
#if UNITY_EDITOR        
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                audioController.PlayAudio(AudioType.BedroomSoundtrack, true, 1.0f);
            }
            
            if (Input.GetKeyUp(KeyCode.G))
            {
                audioController.StopAudio(AudioType.BedroomSoundtrack, true);
            }
            
            if (Input.GetKeyUp(KeyCode.B))
            {
                audioController.RestartAudio(AudioType.BedroomSoundtrack, true);
            }
            
            if (Input.GetKeyUp(KeyCode.Y))
            {
                audioController.PlayAudio(AudioType.BenDeathSound);
            }
            
            if (Input.GetKeyUp(KeyCode.H))
            {
                audioController.StopAudio(AudioType.BenDeathSound);
            }
            
            if (Input.GetKeyUp(KeyCode.N))
            {
                audioController.RestartAudio(AudioType.BenDeathSound);
            }
        }
        
#endif
#endregion
    }
}
