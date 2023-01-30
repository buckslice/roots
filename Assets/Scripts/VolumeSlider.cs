using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

    public AudioMixer mixer;
    public Slider mySlider;

    void Start() {
        if (mixer.GetFloat("MasterVolume", out float v)) {
            mySlider.value = Mathf.Pow(10, v / 20.0f);
        } else {
            mySlider.value = 1.0f;
        }
    }

    public void SetLevel(float sliderValue) {
        float volume = 0.0f;
        if (sliderValue > 0.001f) {
            volume = Mathf.Log10(sliderValue) * 20;
        }
        mixer.SetFloat("MasterVolume", volume);
    }

}
