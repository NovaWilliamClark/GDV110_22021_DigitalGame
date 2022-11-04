using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIGammaSlider : MonoBehaviour
{
    public VolumeProfile profile;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        AudioManager.Instance.StopMusic();
        slider.onValueChanged.AddListener(OnSliderChanged);
    }
    
    public void OnSliderChanged(float value)
    {
        // TODO: Store gamma value in a settings file
        
        LiftGammaGain lgg;
        if (profile.TryGet(out lgg))
        {
            lgg.gamma.overrideState = true;
            Debug.Log(lgg.gamma.value);
            var gammaValue = lgg.gamma.value;
            gammaValue.w = value;

            lgg.gamma.value = gammaValue;
            //lgg.gamma.value = value;
        }
    }
}
