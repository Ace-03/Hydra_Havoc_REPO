using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Setting_Script : MonoBehaviour
{
    [SerializeField] string volumeParameter = "Master_Volume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] Toggle toggle;
    private bool disableToggleEvent;

    //public GameObject Settings_Menu;

    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
        toggle.onValueChanged.AddListener(HandleToggleValueChanged);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    private void HandleSliderValueChanged(float value)
    {
        mixer.SetFloat(volumeParameter, value);
        disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }
    private void HandleToggleValueChanged(bool enable_sound)
    {
        if (disableToggleEvent)
            return;

        if (enable_sound)
            slider.value = slider.maxValue;
        else
            slider.value = slider.minValue;
    }

}
