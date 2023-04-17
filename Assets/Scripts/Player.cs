using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private float _moveInput;
    public bool faceOrientationRight = true;
    private bool _onFoot = true;
    private bool _canShoot = true;

    [SerializeField] private AnimationCurve animationCurve;
    
    private Animator _animator;
    private string _currentAnimation;

    void Move()
    {
        _moveInput = Input.GetAxis("Horizontal");
        if (_moveInput != 0 && _onFoot && Math.Abs(_rb.velocity.y) < 0.1) 
            ChangeAnimation("Move_anim");
        else if (_onFoot && _moveInput == 0 && Math.Abs(_rb.velocity.y) < 0.1)
            ChangeAnimation("Idle_anim");

        _rb.velocity = new Vector2(_moveInput * speed, _rb.velocity.y);
    }

    void Jump()
    {
        ChangeAnimation("Jump_anim");
        _rb.AddForce(new Vector2(0, jumpForce*100), ForceMode2D.Impulse);
        _onFoot = false;
    }

    void Shoot()
    {
        
    }

    void Flip()
    {
        if (_moveInput > 0 && !faceOrientationRight || _moveInput < 0 && faceOrientationRight)
        {
            transform.localScale *= new Vector2(-1, 1);
            faceOrientationRight = !faceOrientationRight;
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
        GetComponent<Collider2D>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _onFoot)
        {
            Jump();
        }
        Move();
        Flip();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onFoot = true;
        }
    }
}