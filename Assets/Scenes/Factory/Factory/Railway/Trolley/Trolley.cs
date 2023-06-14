using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    private Rigidbody2D _rb;
    private string _currentAnimation;
    private Animator[] _wheelsAnimators;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _wheelsAnimators = GetComponentsInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (Math.Abs(_rb.velocity.x) < 0.01f)
            ChangeAnimation("IdleWheel");
        else if (_rb.velocity.x > 0)
            ChangeAnimation("SpinRight");
        else
            ChangeAnimation("SpinLeft");
    }
    
    private void ChangeAnimation(string anim)
    {
        if (_currentAnimation == anim)
            return;

        _currentAnimation = anim;
        foreach (var animator in _wheelsAnimators)
        {
            animator.Play(anim);
        }
    }
}
