using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelEventSystem : MonoBehaviour
{
    public DialogueTrigger[] dialogueTriggers;
    protected Queue<DialogueTrigger> _dialogues;

    protected int CurrentEvent;
    protected bool EventIsHappening;
    
    private void Start()
    {
        _dialogues = new Queue<DialogueTrigger>();
        foreach (var dialogueTrigger in dialogueTriggers)
            _dialogues.Enqueue(dialogueTrigger);
        
        StartNextDialogue();
    }

    protected void StartNextDialogue()
    {
        EventIsHappening = true;
        var dialogue = _dialogues.Dequeue();
        dialogue.TriggerDialogue();
    }

    public void NextEvent()
    {
        EventIsHappening = false;
        CurrentEvent++;
    }
}
