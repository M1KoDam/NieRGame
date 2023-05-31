using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Sounds : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    
    private static float _musicVolume = 1;
    private static float _soundVolume = 1;

    [SerializeField] public AudioClip explosionSound;
    [SerializeField] public AudioClip flightUnitShotSound;
    [SerializeField] public AudioClip backGroundFlightSound;
    [SerializeField] public AudioClip flightSideMusic;
    [SerializeField] public AudioClip menuMusic;
    [SerializeField] public AudioClip enemyShotSound;
    [SerializeField] public AudioClip checkPoints;
    [SerializeField] public AudioClip attack2B;
    [SerializeField] public AudioClip getDamage2B;
    [SerializeField] public AudioClip podShot;
    [SerializeField] public AudioClip enemyAttack;
    public Dictionary<string, Sound> AllSounds;
    // Start is called before the first frame update
    void Awake()
    {
        AllSounds = new Dictionary<string, Sound>()
        {
            { "Explosion", new Sound("Explosion", 0.3f, explosionSound) },
            { "FlightUnitShot", new Sound("FlightUnitShot", 0.5f, flightUnitShotSound) },
            { "BackGroundFlightSound", new Sound("BackGroundFlightSound", 0.05f, backGroundFlightSound) },
            { "FlightSideMusic", new Sound("FlightSideMusic", 0.3f, flightSideMusic) },
            { "MenuMusic", new Sound("MenuMusic", 0.15f, menuMusic) },
            { "EnemyShot", new Sound("EnemyShot", 0.15f, enemyShotSound) },
            { "EnemyAttack", new Sound("EnemyAttack", 0.15f, enemyAttack) },
            { "CheckPoints", new Sound("CheckPoints", 0.2f, checkPoints) },
            { "Attack2B", new Sound("Attack2B", 0.2f, attack2B) },
            { "GetDamage2B", new Sound("GetDamage2B", 0.2f, getDamage2B) },
            { "PodShot", new Sound("PodShot", 0.2f, podShot) },
        };
    }

    private void Update()
    {
        if (Math.Abs(_soundVolume - _soundSlider.value) > 0.01f)
        {
            _soundVolume = _soundSlider.value;
            ChangedVolume(_soundVolume);
        }
        if (Math.Abs(_musicVolume - _musicSlider.value) > 0.01f)
        {
            _musicVolume = _musicSlider.value;
            ChangedVolume(_musicVolume);
        }
    }

    public void ChangedVolume(float soundVolume)
    {
        soundVolume = Math.Max(Math.Min(1, soundVolume), 0);
        foreach (var sound in AllSounds)
        {
            sound.Value.audioSource.volume = sound.Value.maxVolume*soundVolume;
        }
    }
}
