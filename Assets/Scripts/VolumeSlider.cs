using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

    public AudioMixer mixer;
    public Slider mySlider;

    float logScale = 30.0f;

    void Start() {
        if (mixer.GetFloat("MasterVolume", out float v)) {
            mySlider.value = Mathf.Pow(10, v / logScale);
        } else {
            mySlider.value = 1.0f;
        }
    }

    public void SetLevel(float sliderValue) {
        float volume = -100.0f;
        if (sliderValue > 0.001f) {
            volume = Mathf.Log10(sliderValue) * logScale;
        }
        mixer.SetFloat("MasterVolume", volume);
    }

}
