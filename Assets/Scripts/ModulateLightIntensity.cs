using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class ModulateLightIntensity : MonoBehaviour
{
    private Light2D myLight;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float wait = .2f;
    [SerializeField] private float offset = 0.1f;
    private float startIntensity = 0;
    
    private void Awake()
    {
        myLight = GetComponent<Light2D>();
        startIntensity = myLight.intensity;
    }

    private void Start()
    {
        InvokeRepeating(nameof(Flicker), startDelay, wait);
        
        
    }

    

    void Flicker()
    {
        float start = Mathf.Max(startIntensity - offset, 0f);

        DOVirtual.Float(myLight.intensity, Random.Range(start, startIntensity + offset), wait, updateVal =>
        {
            myLight.intensity = updateVal;
        });
    }
}
