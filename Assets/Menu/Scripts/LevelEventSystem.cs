using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class LevelEventSystem : MonoBehaviour
{
    [SerializeField] private DialogueTrigger[] dialogueTriggers;
    [SerializeField] protected Transform[] moveSpots;
    [SerializeField] protected Transform[] spawnSpots;

    public Animator windowAnimator;
    public Animator pauseAnimator;
    public Animator healthBarAnimator;
    public Animator transitionWindowLR;
    public Animator transitionWindowRL;
     
    private Queue<DialogueTrigger> _dialogues;
    private bool _isPaused;
    private float _pauseDelay;
    private bool CanPause => _pauseDelay >= 0.3f;

    protected int CurrentEvent;
    public bool dialogueEventIsHappening;
    protected bool AttackEventIsHappening;
    protected bool EventCompleted => !dialogueEventIsHappening && !AttackEventIsHappening;
    protected List<SmallFlyer> SmallFlyers;
    
    private void Start()
    {
        Invoke(nameof(Open), 1);
        CurrentEvent = -1;
        SmallFlyers = new List<SmallFlyer>();
        _dialogues = new Queue<DialogueTrigger>();
        foreach (var dialogueTrigger in dialogueTriggers)
            _dialogues.Enqueue(dialogueTrigger);
    }

    private void Open()
    {
        transitionWindowLR.SetBool("Open", false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (CanPause || _isPaused))
        {
            _pauseDelay = 0;
            SetPaused();
        }
    }

    private void FixedUpdate()
    {
        _pauseDelay += Time.deltaTime;
        UpdateEnemies();
        if (EventCompleted)
            NextEvent();
        CreateEvent();
    }

    public virtual void CreateEvent()
    {
    }
    
    protected void UpdateEnemies()
    {
        SmallFlyers = SmallFlyers.Where(c => !c.IsUnityNull()).ToList();
        if (SmallFlyers.Count == 0)
            AttackEventIsHappening = false;
    }
    
    protected void SpawnFlyerTop(SmallFlyerTop flyerType, Transform spawnPosition, IEnumerable<Transform> moveSpots)
    {
        var enemy = Instantiate(flyerType, spawnPosition.position, transform.rotation);
        foreach (var moveSpot in moveSpots)
        {
            enemy.GiveMoveSpot(moveSpot);
        }
        SmallFlyers.Add(enemy);
    }
    
    protected void SpawnFlyerSide(SmallFlyerSide flyerType, Transform spawnPosition, IEnumerable<Transform> moveSpots)
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

    public void NextEvent()
    {
        AttackEventIsHappening = false;
        dialogueEventIsHappening = false;
        CurrentEvent++;
    }

    #region Menu

    protected void NextLevel()
    {
        transitionWindowRL.SetBool("Open", true);
        Invoke(nameof(OpenNextLevel), 1);
    }

    private void OpenNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void SetPaused()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            pauseAnimator.SetBool("Paused", true);
            windowAnimator.SetBool("StartOpen", false);
            healthBarAnimator.SetBool("Show", false);
        }
        if (_isPaused)
            Invoke(nameof(Paused), 0.3f);
        else
            Paused();
    }

    private void Paused()
    {
        if (_isPaused)
        {
            Time.timeScale = 0;
            return;
        }
        
        Time.timeScale = 1;
        pauseAnimator.SetBool("Paused", false);
        windowAnimator.SetBool("StartOpen", true);
        healthBarAnimator.SetBool("Show", true);
    }

    #endregion
}
