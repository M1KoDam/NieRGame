using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private float _moveInput;
    public bool faceOrientationRight = true;
    private bool _isGround = true;
    
    private Animator _animator;
    private string _currentAnimation;

    void Move()
    {
        _moveInput = Input.GetAxis("Horizontal");
        ChangeAnimation(_moveInput != 0 ? "Move_anim" : "Idle_anim");
        _rb.velocity = new Vector2(_moveInput * speed, _rb.velocity.y);
    }

    void Jump()
    {
        if (!_isGround)
            _rb.AddForce(new Vector2(0, jumpForce*100), ForceMode2D.Impulse);
    }

    void Flip()
    {
        if (_moveInput > 0 && faceOrientation == Side.Left || _moveInput < 0 && faceOrientation == Side.Right)
        {
            transform.localScale *= new Vector2(-1, 1);
            faceOrientation = faceOrientation == Side.Right ? Side.Left : Side.Right;
        }
    }

    void ChangeAnimation(string animation)
    {
        if (_currentAnimation == animation) return;
        _animator.Play(animation);
        _currentAnimation = animation;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var hit = Physics2D.Raycast(_rb.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        _isGround = !hit.collider.IsUnityNull();
        
        Move();
        Flip();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
}