using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private string _currentAnimation;
    
    [SerializeField] private LightSword lightSword;
    [SerializeField] private HeavySword heavySword;
    [SerializeField] private SpinningSword spinningSword;

    private bool _onFoot = true;
    private bool _attack = true;
    private bool _doubleJump = true;
    private bool _attackInAir;
    private bool _fallAttack;

    [SerializeField] private float health = 10;
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpForce = 1200;

    private static readonly Vector3 RightLocalScale = new(1, 1);
    private static readonly Vector3 LeftLocalScale = new(-1, 1);

    private static float MovementAxis => Input.GetAxis("Horizontal");
    public Side faceOrientation;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        faceOrientation = Side.Right;
    }

    private void Update()
    {
        if (heavySword.inHands && _currentAnimation is not "Fall_Attack_End_anim") heavySword.ReturnSword();
        if (lightSword.inHands) lightSword.ReturnSword();
        
        if (HeavyAttack())
        {
            spinningSword.Destroy();
            heavySword.DrawSword();
            return;
        }
        
        if (LightAttack())
        {
            lightSword.DrawSword();
            return;
        }
        spinningSword.Destroy();

        if (Jump()) return;
        if (Move()) return;
        if (Fall()) return;
        Idle();
    }
    
    #region HeavyAttack

    private bool HeavyAttack()
    {
        if (Input.GetMouseButtonDown(1) && _fallAttack && !_onFoot)
        {
            ChangeAnimation("Fall_Attack_anim");
            _rb.velocity = new Vector2(0, 0.25f);
            _fallAttack = false;
            return true;
        }
        if (_currentAnimation == "Fall_Attack_anim")
        {
            if (AnimPlaying())
            {
                _rb.velocity = new Vector2(0, 0.25f);
            }
            if (_rb.velocity.y < 0)
            {
                ChangeAnimation("Fall_Attack_Falling_anim");
                _rb.velocity = new Vector2(0, -15);
            }
            return true;
        }
        if (_currentAnimation == "Fall_Attack_Falling_anim" && _onFoot)
        {
            ChangeAnimation("Fall_Attack_End_anim");
            return true;
        }
        
        if (_currentAnimation is "Fall_Attack_Falling_anim" or "Fall_Attack_End_anim")
        {
            return _currentAnimation is not "Fall_Attack_End_anim" || !CheckAnimTime(0.5f);
        }

        return false;
    }
    
    #endregion

    #region LightAttack

    private bool LightAttack()
    {
        if (Input.GetMouseButtonDown(0) && _onFoot)
        {
            if (_currentAnimation == "Attack_anim" && CheckAnimTime(0.5f))
            {
                ChangeAttack(1);
                ChangeAnimation("Attack_anim2");
                _rb.velocity = new Vector2(7 * (int)faceOrientation, _rb.velocity.y);
                return true;
            }
            if (_currentAnimation == "Attack_anim2" && CheckAnimTime(0.5f))
            {
                ChangeAttack(2);
                ChangeAnimation("Attack_anim3");
                _rb.velocity = new Vector2(7 * (int)faceOrientation, _rb.velocity.y);
                spinningSword.Create();
                return true;
            }
            if (_attack)
            {
                _attack = false;
                ChangeAttack(1);
                if (_rb.velocity.x > 20)
                    _rb.velocity = new Vector2(10 * (int)faceOrientation, _rb.velocity.y);
                ChangeAnimation("Attack_anim");
                return true;
            }
        }
        if (Input.GetMouseButtonDown(0) && _attackInAir && !_onFoot && _attack)
        {
            ChangeAnimation("Attack_in_air_anim");
            _rb.velocity = new Vector2(0, 0.5f);
            _attackInAir = false;
            return true;
        }
        if (Input.GetMouseButtonDown(0) && _currentAnimation == "Attack_in_air_anim" && CheckAnimTime(0.5f))
        {
            ChangeAnimation("Attack_in_air_anim2");
            _rb.velocity = new Vector2(4 * (int)faceOrientation, 0.5f);
            return true;
        }
        if (Input.GetMouseButtonDown(0) && _currentAnimation == "Attack_in_air_anim2" && CheckAnimTime(0.5f))
        {
            ChangeAnimation("Attack_in_air_anim3");
            _rb.velocity = new Vector2(4 * (int)faceOrientation, 0.5f);
            spinningSword.Create();
            return true;
        }
        if (_currentAnimation is "Attack_anim" or "Attack_anim2" or "Attack_anim3" && AnimPlaying())
        {
            return true;
        }
        if (_currentAnimation is "Attack_in_air_anim" or "Attack_in_air_anim2" or "Attack_in_air_anim3" &&
            AnimPlaying())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0.5f);
            return true;
        }
        
        return false;
    }

    #endregion

    #region Move

    private bool Move()
    {
        if (MovementAxis != 0)
        {
            faceOrientation = MovementAxis > 0 ? Side.Right : Side.Left;
            _rb.velocity = new Vector2(MovementAxis * speed, _rb.velocity.y);
            Flip();
            if (_onFoot)
            {
                ChangeAnimation("Move_anim");
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Jump

    private bool Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _onFoot )
        {
            ChangeAnimation("Jump_anim");
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _onFoot = false;
            return true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && _doubleJump && _currentAnimation is "Jump_anim" && CheckAnimTime(0.5f))
        {
            _doubleJump = false;
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            return true;
        }

        return false;
    }

    #endregion

    #region Fall

    private bool Fall()
    {
        if (!_onFoot && (_rb.velocity.y < -5 || _currentAnimation == "Attack_in_air_anim"))
        {
            ChangeAnimation("Fall_anim");
            return false;
        }
        if (_currentAnimation == "Fall_anim" && _onFoot)
        {
            ChangeAnimation("2B_Fall_End_anim");
            return true;
        }
        if (_currentAnimation == "2B_Fall_End_anim" && AnimPlaying())
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
            if (_currentAnimation is "Fall_Attack_End_anim" && !AnimCompleted())
                return true;
            ChangeAnimation("Idle_anim");
            return true;
        }

        return false;
    }

    #endregion

    private void Flip()
    {
        transform.localScale = faceOrientation == Side.Right ? RightLocalScale : LeftLocalScale;
    }

    private void ChangeAttack(float delayTime)
    {
        CancelInvoke(nameof(WaitForAttack));
        Invoke(nameof(WaitForAttack), delayTime);
    }

    private void WaitForAttack()
    {
        _attack = true;
    }

    #region Animation
    
    private bool AnimPlaying(float time = 1)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < time;
    }
    
    private bool AnimCompleted()
    {
        return Math.Abs(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1) < 0.01f;
    }
    
    private bool CheckAnimTime(float time)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > time;
    }

    private void ChangeAnimation(string anim)
    {
        if (_currentAnimation == anim)
            return;

        _currentAnimation = anim;
        _animator.Play(anim);
    }
    
    #endregion

    #region Collision
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onFoot = true;
            _attackInAir = true;
            _fallAttack = true;
            _doubleJump = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onFoot = false;
        }
    }
    #endregion
}