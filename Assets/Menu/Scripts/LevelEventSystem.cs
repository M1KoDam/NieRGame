using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelEventSystem : MonoBehaviour
{
    [Header("Triggers")]
    [SerializeField] private DialogueTrigger[] dialogueTriggers;
    
    [Header("Spots")]
    [SerializeField] protected Transform[] moveSpots;
    [SerializeField] protected Transform[] spawnSpots;

    [Header("UIController")]
    [SerializeField] private UIController uiController;

    private Queue<DialogueTrigger> _dialogues;

    protected int CurrentEvent;
    public bool dialogueEventIsHappening;
    protected bool AttackEventIsHappening;
    protected bool EventCompleted => !dialogueEventIsHappening && !AttackEventIsHappening;
    private List<SmallFlyer> _smallFlyers;
    
    private void Start()
    {
        Invoke(nameof(OpenLevel), 1);
        CurrentEvent = -1;
        _smallFlyers = new List<SmallFlyer>();
        _dialogues = new Queue<DialogueTrigger>();
        foreach (var dialogueTrigger in dialogueTriggers)
            _dialogues.Enqueue(dialogueTrigger);
    }

    private void OpenLevel()
    {
        uiController.OpenLevel();
    }

    private void FixedUpdate()
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
    
    protected void SpawnFlyerTop(SmallFlyerTop flyerType, Transform spawnPosition, IEnumerable<Transform> moveSpots)
    {
        var enemy = Instantiate(flyerType, spawnPosition.position, transform.rotation);
        foreach (var moveSpot in moveSpots)
        {
            enemy.GiveMoveSpot(moveSpot);
        }
        _smallFlyers.Add(enemy);
    }
    
    protected void SpawnFlyerSide(SmallFlyerSide flyerType, Transform spawnPosition, IEnumerable<Transform> moveSpots)
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
    
    protected void NextLevel()
    {
        uiController.CloseLevel();
        Invoke(nameof(OpenNextLevel), 1);
    }

    private void OpenNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
