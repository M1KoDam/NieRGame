using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Musics : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    private static float _musicVolume = 1;
    
    [SerializeField] public AudioClip flightSideMusic;
    [SerializeField] public AudioClip menuMusic;
    public Dictionary<string, Sound> AllMusics;
    // Start is called before the first frame update
    void Awake()
    {
        AllMusics = new Dictionary<string, Sound>()
        {
            {
                "FlightSideMusic",
                gameObject.AddComponent<Sound>().InitializationSounds("FlightSideMusic", 0.3f, flightSideMusic)
            },
            { "MenuMusic", gameObject.AddComponent<Sound>().InitializationSounds("MenuMusic", 0.15f, menuMusic) },
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(_musicVolume - _musicSlider.value) > 0.01f)
        {
            _musicVolume = _musicSlider.value;
            ChangedVolume(_musicVolume);
        }
    }
    public void ChangedVolume(float soundVolume)
    {
        soundVolume = Math.Max(Math.Min(1, soundVolume), 0);
        foreach (var sound in AllMusics)
        {
            sound.Value.audioSource.volume = sound.Value.maxVolume*soundVolume;
        }
    }
}
