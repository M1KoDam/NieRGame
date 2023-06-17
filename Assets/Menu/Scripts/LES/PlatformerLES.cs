using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformerLES : GameLES
{
    [SerializeField] private PlayerCamera Camera;

    [Header("Control")] [SerializeField] private Animator[] controlAnimators;
    [Header("Control")] [SerializeField] private Animator[] hintAnimators;
    private int _cAIndex;
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

        if (signal is 3)
        {
            Camera.SetSize(12.5f);
        }

        if (signal is 2)
        {
            Camera.SetSize(7.5f);
        }

        if (signal is >= 10 and < 20)
        {
            Invoke(nameof(ShowControll), 1);
        }

        if (signal is >= 20 and < 25)
        {
            ShowHints(signal%5);
        }
        
        if (signal is >= 25 and < 30)
        {
            HideHints(signal%5);
        }
    }

    private void ShowHints(int signal)
    {
        if (signal > hintAnimators.Length - 1)
            return;
        hintAnimators[signal].SetBool("IsHide", false);
    }
    
    private void HideHints(int signal)
    {
        Debug.Log(true);
        if (signal > hintAnimators.Length - 1)
            return;
        hintAnimators[signal].SetBool("IsHide", true);
    }

    private void ShowControll()
    {
        if (_cAIndex > controlAnimators.Length - 1)
            return;
        controlAnimators[_cAIndex].SetBool("IsHide", false);
        Invoke(nameof(HideControll), 3f);
    }
    private void HideControll()
    {
        controlAnimators[_cAIndex].SetBool("IsHide", true);
        _cAIndex++;
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
        Debug.Log("Respawn");
        if (Checkpoint.IsUnityNull())
        {
            base.Respawn();
            return;
        }
        Time.timeScale = 1;
        Camera.RollBack();
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
