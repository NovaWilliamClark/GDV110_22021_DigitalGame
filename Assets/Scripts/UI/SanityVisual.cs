using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Character;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
[ExecuteInEditMode]
public class SanityVisual : MonoBehaviour
{
    public bool RunInEditor = false;
    public RectTransform MaskedImage;
    [FormerlySerializedAs("LargeParticles")] public ParticleSystem ScratchyParticles;
    public ParticleSystem SoftParticles;

    private Canvas canvasObj;

    public float Value = 1f;
    public Vector3 MaskScale = new Vector3(150f, 150f, 1f);
    public float ParticleRadius = 75f;

    public float PercentageToShow = 60.0f;
    public float MinValue = 0.4f;
    public float ParticleMinSize = 10f;
    public float ParticleMaxSize = 20f;

    public float ValueToPlayBgm = 0.6f;

    private float bgmPerc = 0f;
    
    private ParticleSystem.MinMaxCurve tempCurve = new ParticleSystem.MinMaxCurve(25, 50);

    private CharacterSanity player;

    public bool ShowSanity = false;

    private void Awake()
    {
        player = FindObjectOfType<CharacterSanity>();
        canvasObj = GetComponentInChildren<Canvas>(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float LogLerp(float a, float b, float t)
    {
        return a * Mathf.Pow(b / a, t);
    }

    // Update is called once per frame
    void Update()
    {

        if (!Application.isPlaying && !RunInEditor) return;
        var newScale = new Vector3(
            LogLerp(MaskScale.x * MinValue, MaskScale.x, Value),
            LogLerp(MaskScale.y * MinValue, MaskScale.y, Value),
            1f);
        MaskedImage.localScale = newScale;
        var ps = ScratchyParticles.shape;
        var psSize = ScratchyParticles.main;
        var softps = SoftParticles.shape;
        ps.radius = LogLerp(ParticleRadius * MinValue, ParticleRadius, Value);
        softps.radius = LogLerp(ParticleRadius * MinValue, ParticleRadius, Value);

        tempCurve.constantMin = ParticleMinSize * (1 - Value);
        tempCurve.constantMax = ParticleMaxSize * (1 - Value);
        psSize.startSize = tempCurve;

        if (!player) return;
        ShowSanity = player.Enabled;
        canvasObj.gameObject.SetActive(ShowSanity);
        transform.position = player.transform.position;
        
    }

    public void SetPlayer(CharacterSanity cs)
    {
        Value = 1f;
        player = cs;
        player.SanityValueChanged.AddListener(OnSanityChanged);
        canvasObj.gameObject.SetActive(true);
    }

    public void UnsetPlayer()
    {
        player.SanityValueChanged.RemoveListener(OnSanityChanged);
        player = null;
        AudioManager.Instance.StopSanityBgm();
        canvasObj.gameObject.SetActive(false);
        
    }

    public void ToggleVisibility(bool hide = true, float fadeDuration = 0f)
    {
        var endVal = hide ? 0f : 1f;
        var canvasGroup = GetComponentInChildren<CanvasGroup>();
        canvasGroup.DOFade(endVal, fadeDuration).OnPlay(() =>
        {
            if (hide)
            {
                ScratchyParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                SoftParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                ScratchyParticles.Play();
                SoftParticles.Play();
            }
        });
        
    }

    public void Disable()
    {
        player = null;
        canvasObj.gameObject.SetActive(false);

    }

    private void OnSanityChanged(float value, float max)
    {
        Value = value / max;
        Value = Mathf.Clamp(Value, 0f, 1.0f);

        if (Value <= ValueToPlayBgm)
        {
            //Debug.Log((ValueToPlayBgm-Value)/ValueToPlayBgm);
            var bgmvol = (ValueToPlayBgm - Value) / ValueToPlayBgm;
            AudioManager.Instance.PlaySanityBgm(bgmvol);
        }
    }
}