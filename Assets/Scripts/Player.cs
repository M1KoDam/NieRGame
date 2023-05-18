using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private string _currentAnimation;

    [Header("Swords")] 
    [SerializeField] private LightSword lightSword;
    [SerializeField] private HeavySword heavySword;
    [SerializeField] private SpinningSword spinningSword;

    [FormerlySerializedAs("SwingCollider")]
    [FormerlySerializedAs("attack1Collider")]
    [Header("References")] 
    [SerializeField] private Transform SwingSwordCollider;

    [FormerlySerializedAs("FallCollider")] [FormerlySerializedAs("attack2Collider")] [SerializeField] private Transform FallSwordCollider;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask enemies;
    [SerializeField] private LayerMask enemyBullet;
    [SerializeField] private LayerMask checkpoints;

    [Header("Step Climb Settings")] 
    [SerializeField] private GameObject stayRayUpper;
    [SerializeField] private GameObject stayRayLower;
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepLenght;

    private bool _onFoot = true;
    private bool _attack = true;
    private bool _doubleJump = true;
    private bool _attackInAir;
    private bool _fallAttack;
    private int _onCollision;
    private bool _climb;
    private bool _canMove = true;

    [Header("Health Settings")] 
    [SerializeField] private int health;

    [SerializeField] private int maxHealth;

    [Header("Delay")] 
    [SerializeField] private float attackDelay1;
    [SerializeField] private float attackDelay2;
    [SerializeField] private float attackDelay3;

    [Header("Other Settings")] 
    [SerializeField] private float speed = 12;

    [SerializeField] private float jumpForce = 1200;
    [SerializeField] private float lightAttackRange;
    [SerializeField] private float heavyAttackRange;
    [SerializeField] private Vector2 fallAttackRange;
    [SerializeField] private int lightAttackDamage = 20;
    [SerializeField] private int heavyAttackDamage = 60;
    [SerializeField] private Side faceOrientation;

    [SerializeField] private Checkpoint checkpoint;
    private PlayerState _playerState = PlayerState.Default;
    private List<Collider2D> _hitEnemies;
    // A variable used to give the animator a frame to update animations called outside the Update method
    private bool _changeAnimFrameWait;

    private static readonly Vector3 RightLocalScale = new(1, 1);
    private static readonly Vector3 LeftLocalScale = new(-1, 1);

    private readonly string[] _getDamagedAnimations = 
        {"GetDamaged1", "GetDamaged2", "GetDamagedInAir1", "GetDamagedInAir2"};

    private static float MovementAxis => Input.GetAxis("Horizontal");

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _hitEnemies = new List<Collider2D>();
    }

    private void Update()
    {
        if (Dead()) return;
        if (Preprocessing()) return;
        StepClimb();
        if (Climb()) return;
        ChangeCheckpoint();
        if (Attack()) return;
        if (Jump()) return;
        if (Move()) return;
        if (Fall()) return;
        Idle();
    }

    #region Death

    private bool Dead()
    {
        if (_playerState is PlayerState.Dead)
            return true;

        if (health > 0) return false;
        Die();
        return true;
    }
    
    private void Die()
    {
        _playerState = PlayerState.Dead;
        Invoke(nameof(Respawn), 5);
    }

    private void Respawn()
    {
        _playerState = PlayerState.Default;
        health = maxHealth;
        transform.position = checkpoint.transform.position;
    }

    #endregion

    #region Preprocessing
    
    private bool Preprocessing()
    {
        if (_changeAnimFrameWait || _getDamagedAnimations.Contains(_currentAnimation) && AnimPlaying())
        {
            if (_changeAnimFrameWait) _changeAnimFrameWait = false;
            return true;
        }

        return false;
    }
    
    #endregion

    #region StepClimb

    private void StepClimb()
    {
        var hitLower = Physics2D.Raycast(stayRayLower.transform.position,
            Vector2.right * (int)faceOrientation, 0.8f, ground);
        if (hitLower.collider && _rb.velocity.y >= 0 && _rb.velocity.x * (int)faceOrientation > 0.1f)
        {
            var hitUpper = Physics2D.Raycast(stayRayUpper.transform.position,
                Vector2.right * (int)faceOrientation, 0.8f, ground);
            if (!hitUpper.collider)
            {
                _rb.position -= new Vector2(-stepLenght * (int)faceOrientation, -stepHeight);
            }
        }
    }

    #endregion

    #region Climb

    private bool Climb()
    {
        if (_climb)
        {
            if (_currentAnimation is "GetDamagedClimb" && AnimPlaying())
                ChangeAnimation("GetDamagedClimb");
            else
            {
                ChangeAnimation("Climb");
            }
            var velocity = _rb.velocity;
            _rb.velocity = new Vector2(velocity.x, Math.Max(0.2f, velocity.y));
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _rb.velocity = new Vector2(velocity.x, -10);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _rb.velocity = new Vector2(-(int)faceOrientation * 5, 0);
                faceOrientation = (Side)((int)faceOrientation * -1);
                Flip();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _rb.velocity = new Vector2(-(int)faceOrientation * 12, 20);
                faceOrientation = (Side)((int)faceOrientation * -1);
                Flip();
            }

            return true;
        }

        return false;
    }

    #endregion

    #region ChangeCheckpoint

    private void ChangeCheckpoint()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var hitCheckpoint = Physics2D.OverlapCircleAll(transform.position, 5, checkpoints);
            if (hitCheckpoint.Length > 0)
            {
                checkpoint = hitCheckpoint[0].GetComponent<Checkpoint>();
            }
        }
    }

    #endregion

    #region Attack

    private bool Attack()
    {
        if (heavySword.inHands && _currentAnimation is not "FallAttackEnd") heavySword.ReturnSword();
        if (lightSword.inHands) lightSword.ReturnSword();

        if (HeavyAttack())
        {
            spinningSword.Destroy();
            heavySword.DrawSword();
            return true;
        }

        _hitEnemies.Clear();
        if (_currentAnimation is "GetDamagedInAir" && AnimPlaying())
        {
            return true;
        }

        if (LightAttack())
        {
            lightSword.DrawSword();
            return true;
        }
        
        return false;
    }

    #endregion

    #region HeavyAttack

    private bool HeavyAttack()
    {
        if (Input.GetMouseButtonDown(1) && _fallAttack && !_onFoot)
        {
            Damage(heavyAttackRange, heavyAttackDamage);
            ChangeAnimation("FallAttack");
            _rb.velocity = new Vector2(0, 0.25f);
            _fallAttack = false;
            return true;
        }

        if (_currentAnimation == "FallAttack")
        {
            if (AnimPlaying())
            {
                _rb.velocity = new Vector2(0, 0.25f);
            }
            else if (_rb.velocity.y < 0)
            {
                ChangeAnimation("FallAttackFalling");
                _rb.velocity = new Vector2(0, -15);
            }

            return true;
        }

        if (_currentAnimation == "FallAttackFalling" && _onFoot)
        {
            ChangeAnimation("FallAttackEnd");
            return true;
        }

        if (_currentAnimation is "FallAttackFalling" or "FallAttackEnd")
        {
            if (_currentAnimation is "FallAttackFalling")
                HeavyDamage(heavyAttackDamage);
            return _currentAnimation is not "FallAttackEnd" || !CheckAnimTime(0.5f);
        }

        return false;
    }

    #endregion

    #region LightAttack

    private bool LightAttack()
    {
        if (Input.GetMouseButtonDown(0) && _onFoot)
        {
            if (_currentAnimation == "Attack" && CheckAnimTime(0.5f))
            {
                Damage(lightAttackRange, lightAttackDamage);
                ChangeAttack(attackDelay2);
                ChangeAnimation("Attack2");
                _rb.velocity = new Vector2(7 * (int)faceOrientation, _rb.velocity.y);
                return true;
            }

            if (_currentAnimation == "Attack2" && CheckAnimTime(0.5f))
            {
                ChangeAttack(attackDelay3);
                ChangeAnimation("Attack3");
                _rb.velocity = new Vector2(7 * (int)faceOrientation, _rb.velocity.y);
                spinningSword.Create();
                return true;
            }

            if (_attack)
            {
                Damage(lightAttackRange, lightAttackDamage);
                _attack = false;
                ChangeAttack(attackDelay1);
                if (_rb.velocity.x > 20)
                    _rb.velocity = new Vector2(10 * (int)faceOrientation, _rb.velocity.y);
                ChangeAnimation("Attack");
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0) && !_onFoot)
        {
            if (_attackInAir && _attack)
            {
                Damage(lightAttackRange, lightAttackDamage);
                ChangeAnimation("AttackInAir");
                _rb.velocity = new Vector2(0, 0.5f);
                _attackInAir = false;
                return true;
            }

            if (Input.GetMouseButtonDown(0) && _currentAnimation is "AttackInAir" && CheckAnimTime(0.5f))
            {
                Damage(lightAttackRange, lightAttackDamage);
                ChangeAnimation("AttackInAir2");
                _rb.velocity = new Vector2(4 * (int)faceOrientation, 0.5f);
                return true;
            }

            if (Input.GetMouseButtonDown(0) && _currentAnimation == "AttackInAir2" && CheckAnimTime(0.5f))
            {
                ChangeAnimation("AttackInAir3");
                _rb.velocity = new Vector2(4 * (int)faceOrientation, 0.5f);
                spinningSword.Create();
                return true;
            }
        }

        if (_currentAnimation is "Attack3" or "AttackInAir3" && AnimPlaying() && !spinningSword.isActiveAndEnabled)
        {
            ChangeAnimation(_currentAnimation is "Attack3" ? "Attack3End" : "AttackInAir3End");
            return true;
        }
        
        if (_currentAnimation is "Attack3" or "AttackInAir3" && AnimCompleted() && spinningSword.isActiveAndEnabled)
        {
            ChangeAnimation(_currentAnimation is "Attack3" ? "Attack3End" : "AttackInAir3End");
            spinningSword.Destroy();
            return true;
        }

        if (_currentAnimation is "Attack" or "Attack2" or "Attack3" or "Attack3End" && AnimPlaying())
        {
            return true;
        }

        if (_currentAnimation is "AttackInAir" or "AttackInAir2" or "AttackInAir3" or "AttackInAir3End" &&
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
        if (MovementAxis != 0 && _canMove)
        {
            faceOrientation = MovementAxis > 0 ? Side.Right : Side.Left;
            _rb.velocity = new Vector2(MovementAxis * speed, _rb.velocity.y);
            Flip();
            if (_onFoot)
            {
                ChangeAnimation("Move");
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Jump

    private bool Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _onFoot)
        {
            ChangeAnimation("Jump");
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _onFoot = false;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _doubleJump && _currentAnimation is "Jump" && CheckAnimTime(0.5f))
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
        if (!_onFoot && !_climb && _rb.velocity.y < -5)
        {
            ChangeAnimation("Fall");
            return false;
        }

        if (_currentAnimation == "Fall" && _onFoot)
        {
            ChangeAnimation("FallEnd");
            return true;
        }

        if (_currentAnimation == "FallEnd" && AnimPlaying())
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
            if (_currentAnimation is "FallAttackEnd" && !AnimCompleted())
                return true;
            ChangeAnimation("Idle");
            return true;
        }

        return false;
    }

    #endregion
    
    private void Damage(float attackRadius, int damage)
    {
        var hitEnemies = Physics2D.OverlapCircleAll(SwingSwordCollider.position, attackRadius, enemies);
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().GetDamage(damage);
        }

        var hitBullets = Physics2D.OverlapCircleAll(SwingSwordCollider.position, attackRadius, enemyBullet);
        foreach (var enemy in hitBullets)
        {
            enemy.GetComponent<EnemyBullet>().Destroy();
        }
    }

    private void HeavyDamage(int damage)
    {
        var hitEnemies = Physics2D.OverlapBoxAll(FallSwordCollider.position, fallAttackRange, 0, enemies);
        foreach (var enemy in hitEnemies)
        {
            if (_hitEnemies.Contains(enemy))
                continue;
            enemy.GetComponent<Enemy>().GetDamage(damage);
            _hitEnemies.Add(enemy);
        }

        var hitBullets = Physics2D.OverlapBoxAll(FallSwordCollider.position, fallAttackRange, 0, enemyBullet);
        foreach (var enemy in hitBullets)
        {
            enemy.GetComponent<EnemyBullet>().Destroy();
        }
    }

    public void GetDamage(int inputDamage, Transform attackVector)
    {
        health -= inputDamage;
        var damageVector = (transform.position - attackVector.position).x >= 0 ? -1 : 1;
        spinningSword.Destroy();
        
        if (_climb)
            ChangeAnimation("GetDamagedClimb");
        else if (damageVector == 1 && faceOrientation is Side.Right || damageVector == -1 && faceOrientation is Side.Left)
            ChangeAnimation(_onFoot ? "GetDamaged1" : "GetDamagedInAir1");
        else
            ChangeAnimation(_onFoot ? "GetDamaged2" : "GetDamagedInAir2");
        
        _changeAnimFrameWait = true;
        _rb.velocity -= new Vector2(Math.Min(inputDamage / 5, 10) * damageVector, 0);
    }

    private void Flip()
    {
        transform.localScale = faceOrientation == Side.Right ? RightLocalScale : LeftLocalScale;
    }

    public Side GetFaceOrientation()
    {
        return faceOrientation;
    }

    private void ChangeAttack(float delayTime)
    {
        CancelInvoke(nameof(WaitForAttack));
        Invoke(nameof(WaitForAttack), delayTime);
    }

    private void ChangeMove()
    {
        _canMove = true;
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
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
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
        if (collision.gameObject.CompareTag("Platform") 
            || collision.gameObject.CompareTag("Stairs") 
            || collision.gameObject.CompareTag("Enemy"))
        {
            _onFoot = true;
            _attackInAir = true;
            _fallAttack = true;
            _doubleJump = true;
            _climb = false;
            _canMove = true;
        }

        if (collision.gameObject.CompareTag("Platform") 
            || collision.gameObject.CompareTag("Stairs")
            || collision.gameObject.CompareTag("Enemy"))
        {
            _onCollision++;
        }

        if (collision.gameObject.CompareTag("Wall") && !_onFoot)
        {
            CancelInvoke(nameof(ChangeMove));
            _climb = true;
            _canMove = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") 
            || collision.gameObject.CompareTag("Stairs")
            || collision.gameObject.CompareTag("Enemy"))
        {
            _onCollision--;
        }

        if (_onCollision == 0)
        {
            _onFoot = false;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            _climb = false;
            Invoke(nameof(ChangeMove), 0.5f);
            if (!_onFoot)
                ChangeAnimation("Fall");
        }
    }

    #endregion

    #region Draw

    void OnDrawGizmosSelected()
    {
        var position = SwingSwordCollider.position;
        Gizmos.DrawWireSphere(position, lightAttackRange);
        Gizmos.DrawWireSphere(position, heavyAttackRange);
        Gizmos.DrawWireCube(FallSwordCollider.position, fallAttackRange);
    }

    #endregion
}