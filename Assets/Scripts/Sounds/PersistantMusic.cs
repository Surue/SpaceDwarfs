using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantMusic : MonoBehaviour {

    static PersistantMusic instance;
    public static PersistantMusic Instance {
        get {
            return instance;
        }
    }

    private AudioSource audioSource;
    AudioLowPassFilter lowPassFilter;

    public List<AudioClip> clip;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    private void Update() {
        if(audioSource.clip == null || !audioSource.isPlaying) {
            audioSource.clip = clip[Random.Range(0, clip.Count)];
            audioSource.Play();
        }
    }

    public void PlayMusic() {
        if(!audioSource.isPlaying) {
            audioSource.Play();
        }
    }

    public void StopMusic() {
        audioSource.Stop();
    }

    public void UpVolume() {
        audioSource.volume += 0.025f;
    }

    public void ResetLowPass() {
        lowPassFilter.cutoffFrequency = 600;
    }

    public void AddLowPass(int f) {
        lowPassFilter.cutoffFrequency += f;
    }
}
