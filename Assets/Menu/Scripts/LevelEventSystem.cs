using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class LevelEventSystem : MonoBehaviour
{
    [SerializeField] private DialogueTrigger[] dialogueTriggers;
    [SerializeField] private Transform[] moveSpots;
    [SerializeField] private Transform[] spawnSpots;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject dialogueWindows;
    
    public Animator windowAnimator;
    public Animator pauseAnimator;
    public Animator healthBarAnimator;
     
    private Queue<DialogueTrigger> _dialogues;
    private bool _isPaused;
    private float _pauseDelay;
    private bool CanPause => _pauseDelay >= 0.3f;

    protected int CurrentEvent;
    protected bool EventIsHappening;
    
    private void Start()
    {
        _dialogues = new Queue<DialogueTrigger>();
        foreach (var dialogueTrigger in dialogueTriggers)
            _dialogues.Enqueue(dialogueTrigger);
        
        StartNextDialogue();
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
        CreateEvent();
    }

    public virtual void CreateEvent()
    {
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

    public void NextLevel()
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
}
