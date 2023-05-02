using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HeavySword : MonoBehaviour
{
    private Animator _animator;
    public bool inHands;

    private string _currentAnimation;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _currentAnimation = "Heavy_Sword_anim";
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_currentAnimation);
        _animator.Play(_currentAnimation);
        if (_currentAnimation == "Return_Heavy_Sword_anim" && AnimCompleted())
            _currentAnimation = "Heavy_Sword_anim";
    }

    public void ReturnSword()
    {
        gameObject.SetActive(true);
        _currentAnimation = "Return_Heavy_Sword_anim";
        inHands = false;
    }
    
    public void DrawSword()
    {
        gameObject.SetActive(false);
        inHands = true;
    }

    private bool AnimCompleted()
    {
        return Math.Abs(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1) < 0.01f;
    }
}
