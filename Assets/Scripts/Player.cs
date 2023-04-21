using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;

    private bool _onFoot = true;
    private string _currentAnimation;

    public float speed = 10;
    public float JumpForce = 1200;

    private static readonly Vector3 RightLocalScale = new(1, 1);
    private static readonly Vector3 LeftLocalScale = new(-1, 1);

    private static float MovementAxis => Input.GetAxis("Horizontal");
    private Side _faceOrientation;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeAnimation("Attack_anim");
        }
        else if (MovementAxis != 0)
        {
            _faceOrientation = MovementAxis > 0 ? Side.Right : Side.Left;
            Move();
        }
        else if (_currentAnimation == "2B_Fall_End_anim" && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
        }
        else if (_currentAnimation == "Attack_anim" && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
        }
        else if (_currentAnimation == "Fall_anim" && _onFoot)
        {
            ChangeAnimation("2B_Fall_End_anim");
        }
        else if (_onFoot)
        {
            Stay();
        }

        Flip();

        if (!_onFoot && _rb.velocity.y < -5)
        {
            ChangeAnimation("Fall_anim");
        }
            

        if (_onFoot && Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Move()
    {
        if (_onFoot)
            ChangeAnimation("Move_anim");
        _rb.velocity = new Vector2(MovementAxis * speed, _rb.velocity.y);
    }

    private void Stay()
    {
        ChangeAnimation("Idle_anim");
    }

    private void Flip()
    {
        transform.localScale = _faceOrientation == Side.Right ? RightLocalScale : LeftLocalScale;
    }

    private void Jump()
    {
        ChangeAnimation("Jump_anim");
        _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        _onFoot = false;
    }

    private void ChangeAnimation(string anim)
    {
        if (_currentAnimation == anim)
            return;

        _currentAnimation = anim;
        _animator.Play(anim);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onFoot = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _onFoot = true;
        }
    }
}