using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformerLES : GameLES
{
    private bool _dialogueIsHappening;
    private bool _endIsComing;
    private bool _gameEndComing;
    
    public void GetTriggerSignal(int signal, bool blockPlayer)
    {
        if (blockPlayer)
            player.UnActivePlayer();

        if (signal is 0)
        {
            StartDialogue(1);
            _endIsComing = true;
        }
        
        if (signal is 1)
        {
            StartDialogue(0);
        }

        if (signal is -1)
        {
            StartDialogue(0);
            _gameEndComing = true;
        }
    }

    protected override void UpdateLES()
    {
        if (!_dialogueIsHappening && _gameEndComing)
        {
            uiController.CloseLevel();
            Invoke(nameof(ToMenu), 1);
        }
        if (!_dialogueIsHappening && _endIsComing)
            NextLevel();
    }

    private void ToMenu()
    {
        uiController.BackToMenu();
    }
    
    public override void Respawn()
    {
        if (Checkpoint.IsUnityNull())
        {
            base.Respawn();
            return;
        }
        Time.timeScale = 1;
        player.SetHealth(PlayerHealth);
        player.ActivePlayer();
        player.transform.position = Checkpoint.transform.position;
        uiController.SetPaused();
    }

    private void StartDialogue(int index)
    {
        _dialogueIsHappening = true;
        dialogueTriggers[index].TriggerDialogue();
    }
    
    public override void EndDialogue()
    {
        _dialogueIsHappening = false;
        player.ActivePlayer();
    }
}
