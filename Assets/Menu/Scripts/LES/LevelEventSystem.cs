using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEventSystem : MonoBehaviour
{
    private static GameDifficulty _gameDifficulty;

    protected static float PlayerHealth => _gameDifficulty switch
    {
        GameDifficulty.Easy => 1000,
        GameDifficulty.Medium => 500,
        _ => 100
    };

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
