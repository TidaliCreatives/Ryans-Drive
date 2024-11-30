using UnityEngine;
using UnityEngine.UI;
using System;

public class VolumeSlider : MonoBehaviour
{
    public static event Action<int> OnMasterVolumeChanged;

    Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetInt("MasterVolume", 80);
        }
        slider.value = PlayerPrefs.GetInt("MasterVolume", 80);
    }

    public void SetVolume()
    {
        PlayerPrefs.SetInt("MasterVolume", (int)slider.value);
        OnMasterVolumeChanged?.Invoke((int)slider.value);
    }
}