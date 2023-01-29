using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages music and short menu/ui sound playback
public class SoundSingleton : MonoBehaviour {

    public AudioSource music;
    // meant for short sound effects, like for ui stuff
    public AudioSource[] soundEffects;
    static int soundIndex = 0;

    static SoundSingleton instance = null;

    // Start is called before the first frame update
    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySound(AudioClip clip, float volume, bool increment = true, float pitchVariance = 0.0f) {
        var source = instance.soundEffects[soundIndex];

        if (increment) {
            if (++soundIndex >= instance.soundEffects.Length) {
                soundIndex = 0;
            }
        }

        source.Stop();
        source.clip = clip;
        source.volume = volume;
        if (pitchVariance > 0.0f) {
            source.pitch = 1.0f + Random.Range(-pitchVariance, pitchVariance);
        } else {
            source.pitch = 1.0f;
        }
        source.Play();
    }

}