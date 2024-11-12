using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    public TextMeshProUGUI masterValue;
    public TextMeshProUGUI musicValue;
    public TextMeshProUGUI soundValue;

    
    private void Start()
    {
        LoadVolume();
        SetVolume();
        masterSlider.onValueChanged.AddListener(onSliderChanged);
        musicSlider.onValueChanged.AddListener(onSliderChanged);
        soundSlider.onValueChanged.AddListener(onSliderChanged);
    }
    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("master_volume", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("music_volume", 1f);
        soundSlider.value = PlayerPrefs.GetFloat("sound_volume", 1f);
    }
    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("master_volume", masterSlider.value);
        PlayerPrefs.SetFloat("music_volume", musicSlider.value);
        PlayerPrefs.SetFloat("sound_volume", soundSlider.value);
    }

    private void SetVolume()
    {
        audioMixer.SetFloat("Master", SoundToDB(masterSlider.value));
        audioMixer.SetFloat("Music", SoundToDB(musicSlider.value));
        audioMixer.SetFloat("Sound", SoundToDB(soundSlider.value));

        masterValue.text = ((int)(masterSlider.value * 100f)).ToString();
        musicValue.text = ((int)(musicSlider.value * 100f)).ToString();
        soundValue.text = ((int)(soundSlider.value * 100f)).ToString();
    }

    float SoundToDB(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void onSliderChanged(float value)
    {
        SetVolume();
        SaveVolume();
    }
}
