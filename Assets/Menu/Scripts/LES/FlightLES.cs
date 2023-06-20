using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class FlightLES : GameLES
{
    [Header("Spots")]
    [SerializeField] protected Transform[] moveSpots;
    [SerializeField] protected Transform[] spawnSpots;

    protected int CurrentEvent;
    public bool dialogueEventIsHappening;
    protected bool AttackEventIsHappening;
    protected List<SmallFlyer> SmallFlyers;
    private Queue<DialogueTrigger> _dialogues;
    protected bool EventCompleted => !dialogueEventIsHappening && !AttackEventIsHappening;
    
    protected override void StartLES()
    {
        base.StartLES();
        _dialogues = new Queue<DialogueTrigger>();
        foreach (var dialogueTrigger in dialogueTriggers)
            _dialogues.Enqueue(dialogueTrigger);
        CurrentEvent = -1;
        SmallFlyers = new List<SmallFlyer>();
    }

    protected override void UpdateLES()
    {
        UpdateEnemies();
        if (EventCompleted)
            NextEvent();
        CreateEvent();
    }
    
    protected virtual void CreateEvent()
    {
    }
    
    private void UpdateEnemies()
    {
        SmallFlyers = SmallFlyers.Where(c => !c.IsUnityNull()).ToList();
        if (SmallFlyers.Count == 0)
            AttackEventIsHappening = false;
    }
    
    protected void SpawnFlyer(SmallFlyerFlightScene flyerType, Transform spawnPosition, IEnumerable<Transform> moveSpots)
    {
        var enemy = Instantiate(flyerType, spawnPosition.position, transform.rotation);
        foreach (var moveSpot in moveSpots)
        {
            enemy.GiveMoveSpot(moveSpot);
        }
        SmallFlyers.Add(enemy);
    }
    
    protected void StartNextDialogue()
    {
        var dialogue = _dialogues.Dequeue();
        dialogue.TriggerDialogue();
    }

    private void NextEvent()
    {
        AttackEventIsHappening = false;
        dialogueEventIsHappening = false;
        CurrentEvent++;
    }

    public override void EndDialogue()
    {
        dialogueEventIsHappening = false;
    }
}
