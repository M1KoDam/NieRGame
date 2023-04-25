using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;

    private GameObject _lightSword;
    private GameObject _heavySword;

    private bool _onFoot = true;
    private bool _attackInAir = false;
    private string _currentAnimation;

    public float speed = 10;
    public float jumpForce = 1200;

    private static readonly Vector3 RightLocalScale = new(1, 1);
    private static readonly Vector3 LeftLocalScale = new(-1, 1);

    private static float MovementAxis => Input.GetAxis("Horizontal");
    private Side _faceOrientation;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _lightSword = transform.Find("Light_Sword").GameObject();
        _heavySword = transform.Find("Heavy_Sword").GameObject();
    }

    private void Update()
    {
        if (Attack()) return;
        if (Jump()) return;
        if (Move()) return;
        if (Fall()) return;
        Idle();
    }

    #region Attack

    private bool Attack()
    {
        if (Input.GetMouseButtonDown(0) && _onFoot)
        {
            if (_currentAnimation == "Attack_anim" && CheckAnimTime(0.8f))
            {
                ChangeAnimation("Attack_anim2");
            }
            ChangeAnimation("Attack_anim");
            return true;
        }
        if (Input.GetMouseButtonDown(0) && _attackInAir && !_onFoot)
        {
            ChangeAnimation("Attack_in_air_anim");
            _rb.velocity = new Vector2(0, 0.25f);
            _attackInAir = false;
            _lightSword.SetActive(false);
            return true;
        }
        if (_currentAnimation is "Attack_anim" or "Attack_anim2" or "Attack_in_air_anim" && CheckForAnimComplete())
        {
            _lightSword.SetActive(false);
            if (_currentAnimation == "Attack_in_air_anim")
                _rb.velocity = new Vector2(0, 0.25f);
            return true;
        }

        _lightSword.SetActive(true);
        _heavySword.SetActive(true);
        return false;
    }

    #endregion

    #region Move

    private bool Move()
    {
        if (MovementAxis != 0)
        {
            _faceOrientation = MovementAxis > 0 ? Side.Right : Side.Left;
            if (_onFoot)
                ChangeAnimation("Move_anim");
            _rb.velocity = new Vector2(MovementAxis * speed, _rb.velocity.y);
            Flip();
            return true;
        }

        return false;
    }

    #endregion

    #region Jump

    private bool Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _onFoot)
        {
            ChangeAnimation("Jump_anim");
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _onFoot = false;
            return true;
        }

        return false;
    }

    #endregion

    #region Fall

    private bool Fall()
    {
        if (!_onFoot && _rb.velocity.y < -5)
        {
            ChangeAnimation("Fall_anim");
            return false;
        }
        if (_currentAnimation == "Fall_anim" && _onFoot)
        {
            ChangeAnimation("2B_Fall_End_anim");
            return true;
        }
        if (_currentAnimation == "2B_Fall_End_anim" && CheckForAnimComplete())
        {
            return true;
        }

        return false;
    }

    #endregion
    
    #region Idle

    private bool Idle()
    {
        if (_onFoot)
        {
            ChangeAnimation("Idle_anim");
            return true;
        }

        return false;
    }

    #endregion
    
    
    private bool CheckForAnimComplete()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }
    
    private bool CheckAnimTime(float time)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > time;
    }

    private void Flip()
    {
        transform.localScale = _faceOrientation == Side.Right ? RightLocalScale : LeftLocalScale;
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
            _attackInAir = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onFoot = false;
        }
    }
}