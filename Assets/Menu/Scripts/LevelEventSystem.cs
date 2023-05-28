using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class LevelEventSystem : MonoBehaviour
{
    [SerializeField] private DialogueTrigger[] dialogueTriggers;
    [SerializeField] private Transform[] moveSpots;
    [SerializeField] private Transform[] spawnSpots;
    private Queue<DialogueTrigger> _dialogues;

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
