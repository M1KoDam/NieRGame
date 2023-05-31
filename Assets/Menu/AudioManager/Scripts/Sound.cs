using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public float volume;
    public GameObject audioObject;
    private float _length;
    public float maxVolume;
    public AudioSource audioSource;
    // Start is called before the first frame update
    public Sound InitializationSounds(string soundName, float volume, AudioClip explosionClip)
    {
        audioObject = new GameObject(soundName);
        audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = explosionClip;
        maxVolume = volume;
        audioSource.volume = maxVolume;
        _length = explosionClip.length;
        return this;
    }

    public void PlaySound()
    {
        audioSource.Play();
    }

    public void PlaySoundLoop()
    {
        audioSource.loop = true;
        audioSource.Play();
    }
    
    public void OnDestroy()
    {
        Destroy(audioObject, _length+1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
