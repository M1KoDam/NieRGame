using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLES : LevelEventSystem
{
    [Header("Player")] 
    [SerializeField] protected Player player;
    
    [Header("UIController")]
    [SerializeField] protected UIController uiController;
    [SerializeField] protected Animator saveWindowAnimator;
    
    [Header("Triggers")]
    [SerializeField] protected DialogueTrigger[] dialogueTriggers;

    protected Checkpoint Checkpoint;

    private void Start()
    {
        StartLES();
    }
    
    protected virtual void StartLES()
    {
        player.SetHealth(PlayerHealth);
        Invoke(nameof(StartSaving), 1.5f);
        Invoke(nameof(StopSaving), 2.5f);
        Invoke(nameof(OpenLevel), 1);
    }
    
    private void FixedUpdate()
    {
        UpdateLES();
    }
    
    protected virtual void UpdateLES()
    {
    }
    
    public virtual void Respawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveCheckpoint(Checkpoint checkpoint)
    {
        Checkpoint = checkpoint;
        StartSaving();
        Invoke(nameof(StopSaving), 1);
    }
    
    private void OpenLevel()
    {
        uiController.OpenLevel();
    }
    
    public void StartSaving()
    {
        saveWindowAnimator.SetBool("Open", true);
    }

    public void StopSaving()
    {
        saveWindowAnimator.SetBool("Open", false);
    }

    public override void NextLevel()
    {
        uiController.CloseLevel();
        Invoke(nameof(OpenNextLevel), 1);
    }

    public virtual void EndDialogue()
    {
        
    }
}
