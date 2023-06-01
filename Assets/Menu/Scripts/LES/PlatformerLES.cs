using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformerLES : GameLES
{
    public void GetTriggerSignal(int signal, bool blockPlayer)
    {
        if (blockPlayer)
            player.UnActivePlayer();
        
        if (signal is 1)
        {
            StartDialogue(0);
        }
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
        dialogueTriggers[index].TriggerDialogue();
    }
    
    public override void EndDialogue()
    {
        player.ActivePlayer();
    }
}
