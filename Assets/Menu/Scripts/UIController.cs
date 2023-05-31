using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIController : MonoBehaviour
{
    [FormerlySerializedAs("windowAnimator")]
    [Header("Animators")]
    [SerializeField] protected Animator dialogueAnimator;
    [SerializeField] protected Animator pauseAnimator;
    [SerializeField] protected Animator healthBarAnimator;
    [SerializeField] protected Animator transitionWindowLR;
    [SerializeField] protected Animator transitionWindowRL;
    
    private bool _isPaused;
    private float _pauseDelay;
    private bool CanPause => _pauseDelay >= 0.3f;

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
    }
    
    public void OpenLevel()
    {
        transitionWindowLR.SetBool("Open", false);
    }
    
    public void CloseLevel()
    {
        transitionWindowRL.SetBool("Open", true);
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
            dialogueAnimator.SetBool("StartOpen", false);
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
        dialogueAnimator.SetBool("StartOpen", true);
        healthBarAnimator.SetBool("Show", true);
    }
}
