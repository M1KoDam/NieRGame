using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerLES : GameLES
{
    public void GetTriggerSignal(int signal)
    {
        if (signal is 1)
        {
            StartDialogue(0);
            player.UnActivePlayer();
        }
    }
    
    public override void Respawn()
    {
        base.Respawn();
        transform.position = Checkpoint.transform.position;
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
