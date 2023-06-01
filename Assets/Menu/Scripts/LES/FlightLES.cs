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
    private List<SmallFlyer> _smallFlyers;
    private Queue<DialogueTrigger> _dialogues;
    protected bool EventCompleted => !dialogueEventIsHappening && !AttackEventIsHappening;
    
    protected override void StartLES()
    {
        base.StartLES();
        _dialogues = new Queue<DialogueTrigger>();
        foreach (var dialogueTrigger in dialogueTriggers)
            _dialogues.Enqueue(dialogueTrigger);
        CurrentEvent = -1;
        _smallFlyers = new List<SmallFlyer>();
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
        _smallFlyers = _smallFlyers.Where(c => !c.IsUnityNull()).ToList();
        if (_smallFlyers.Count == 0)
            AttackEventIsHappening = false;
    }
    
    protected void SpawnFlyer(SmallFlyerFlightScene flyerType, Transform spawnPosition, IEnumerable<Transform> moveSpots)
    {
        var enemy = Instantiate(flyerType, spawnPosition.position, transform.rotation);
        foreach (var moveSpot in moveSpots)
        {
            enemy.GiveMoveSpot(moveSpot);
        }
        _smallFlyers.Add(enemy);
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
