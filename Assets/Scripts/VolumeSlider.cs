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
        if (sliderValue < 0.01f) {
            sliderValue = 0.01f;
        }
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
}
