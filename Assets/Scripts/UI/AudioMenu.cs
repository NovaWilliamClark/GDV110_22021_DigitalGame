using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI masterVolumeValueText;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI bgmVolumeValueText;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI fxVolumeValueText;
    [SerializeField] private Slider fxSlider;

public void MasterValueChanged() // Hooked up in editor
    {
        AudioManager.Instance.MainAudioMixer.SetFloat("MasterVolume", Mathf.Log(masterSlider.value) * 20);
        masterVolumeValueText.text = Mathf.RoundToInt(masterSlider.value * 100).ToString();
    }

    public void BackgroundVolumeChanged() // Hooked up in editor
    {
        AudioManager.Instance.MainAudioMixer.SetFloat("BGMVolume", Mathf.Log(bgmSlider.value) * 20);
        bgmVolumeValueText.text = Mathf.RoundToInt(bgmSlider.value * 100).ToString();
    }
    
    public void FXVolumeChanged() // Hooked up in editor
    {
        AudioManager.Instance.MainAudioMixer.SetFloat("FXVolume", Mathf.Log(fxSlider.value) * 20);
        fxVolumeValueText.text = Mathf.RoundToInt(fxSlider.value * 100).ToString();
    }
}