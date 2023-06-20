using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEventSystem : MonoBehaviour
{
    [SerializeField] private Musics musics;
    [SerializeField] private Sounds sounds;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    private static float _soundVolume = 1;
    private static float _musicVolume = 1;
    
    private static GameDifficulty _gameDifficulty;

    protected static float PlayerHealth => _gameDifficulty switch
    {
        GameDifficulty.Easy => 1000,
        GameDifficulty.Medium => 500,
        _ => 100
    };

    protected void Start()
    {
        sounds.ChangedVolume(_soundVolume);
        musics.ChangedVolume(_musicVolume);
        soundSlider.value = _soundVolume;
        musicSlider.value = _musicVolume;
    }

    private void Update()
    {
        if (Math.Abs(_soundVolume - soundSlider.value) > 0.01f)
        {
            _soundVolume = soundSlider.value;
            sounds.ChangedVolume(_soundVolume);
        }
        if (Math.Abs(_musicVolume - musicSlider.value) > 0.01f)
        {
            _musicVolume = musicSlider.value;
            musics.ChangedVolume(_musicVolume);
        }
    }

    public virtual void NextLevel()
    {
        OpenNextLevel();
    }

    protected void OpenNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetEasyDifficulty()
    {
        _gameDifficulty = GameDifficulty.Easy;
    }

    public void SetMediumDifficulty()
    {
        _gameDifficulty = GameDifficulty.Medium;
    }
    
    public void SetHardDifficulty()
    {
        _gameDifficulty = GameDifficulty.Hard;
    }
}
