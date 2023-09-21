using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public AudioMixer mixer;
    public Settings settings;
    public FloatEditor musicVolume;
    public FloatEditor fxVolume;
    public Toggle stereo;
    
    private void Start()
    {
        if (!fxVolume) return;
        if (!musicVolume) return;
        
        musicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);
        fxVolume.onValueChanged.AddListener(OnFXVolumeChanged);
    }
    
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OnMusicVolumeChanged(float volume)
    {
        mixer.SetFloat("musicVol", Mathf.Log10(volume) * 20);
        settings.musicVolume = volume * 100;
    }
    
    public void OnFXVolumeChanged(float volume)
    {
        mixer.SetFloat("sfxVol", Mathf.Log10(volume) * 20);
        settings.fxVolume = volume * 100;
    }
}
