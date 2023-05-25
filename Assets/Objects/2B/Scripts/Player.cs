using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private string _currentAnimation;

    [Header("Swords")] 
    [SerializeField] private LightSword lightSword;
    [SerializeField] private HeavySword heavySword;
    [SerializeField] private SpinningSword spinningSword;
    
    [Header("References")]
    [SerializeField] private Transform swingSwordCollider; 
    [SerializeField] private Transform fallSwordCollider;
    [SerializeField] private Transform fallEndSwordCollider;
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
    private bool _canAttack = true;
    private bool _doubleJump = true;
    private bool _canAttackInAir;
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
        if (_changeAnimFrameWait || PlayerAnim.GetDamagedAnimations.Contains(_currentAnimation) && AnimPlaying())
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
            if (_currentAnimation is PlayerAnim.GetDamagedClimb && AnimPlaying())
                ChangeAnimation(PlayerAnim.GetDamagedClimb);
            else
            {
                ChangeAnimation(PlayerAnim.Climb);
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
        if (heavySword.inHands && _currentAnimation is not PlayerAnim.FallAttackEnd) heavySword.ReturnSword();
        if (lightSword.inHands) lightSword.ReturnSword();

        if (FallAttack())
        {
            spinningSword.Destroy();
            heavySword.DrawSword();
            return true;
        }

        _hitEnemies.Clear();

        if (LightAttack())
        {
            lightSword.DrawSword();
            return true;
        }
        
        return false;
    }

    #endregion

    #region FallAttack

    private bool FallAttack()
    {
        if (Input.GetMouseButtonDown(1) && _fallAttack && !_onFoot)
        {
            Damage(heavyAttackRange, heavyAttackDamage);
            ChangeAnimation(PlayerAnim.FallAttackStart);
            _rb.velocity = new Vector2(0, 0.25f);
            _fallAttack = false;
            return true;
        }

        if (_currentAnimation is PlayerAnim.FallAttackStart)
        {
            if (AnimPlaying())
            {
                _rb.velocity = new Vector2(0, 0.25f);
            }
            else if (_rb.velocity.y < 0)
            {
                ChangeAnimation(PlayerAnim.FallAttack);
                _rb.velocity = new Vector2(0, -15);
            }

            return true;
        }

        if (_currentAnimation == PlayerAnim.FallAttack && _onFoot)
        {
            ChangeAnimation(PlayerAnim.FallAttackEnd);
            HeavyDamage(heavyAttackDamage, fallSwordCollider);
            return true;
        }

        if (_currentAnimation is PlayerAnim.FallAttack or PlayerAnim.FallAttackEnd)
        {
            if (_currentAnimation is PlayerAnim.FallAttack)
                HeavyDamage(heavyAttackDamage, fallEndSwordCollider);
            return _currentAnimation is not PlayerAnim.FallAttackEnd || !CheckAnimTime(0.5f);
        }

        return false;
    }

    #endregion

    #region LightAttack

    private bool LightAttack()
    {
        if (Input.GetMouseButtonDown(0) && _onFoot)
        {
            if (_currentAnimation == PlayerAnim.Attack1 && CheckAnimTime(0.5f))
            {
                Damage(lightAttackRange, lightAttackDamage);
                ChangeAttack(attackDelay2);
                ChangeAnimation(PlayerAnim.Attack2);
                _rb.velocity = new Vector2(7 * (int)faceOrientation, _rb.velocity.y);
                return true;
            }

            if (_currentAnimation == PlayerAnim.Attack2 && CheckAnimTime(0.5f))
            {
                ChangeAttack(attackDelay3);
                ChangeAnimation(PlayerAnim.Attack3);
                _rb.velocity = new Vector2(7 * (int)faceOrientation, _rb.velocity.y);
                spinningSword.Create();
                return true;
            }

            if (_canAttack)
            {
                Damage(lightAttackRange, lightAttackDamage);
                _canAttack = false;
                ChangeAttack(attackDelay1);
                if (_rb.velocity.x > 20)
                    _rb.velocity = new Vector2(10 * (int)faceOrientation, _rb.velocity.y);
                ChangeAnimation(PlayerAnim.Attack1);
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0) && !_onFoot)
        {
            if (_canAttackInAir)
            {
                Damage(lightAttackRange, lightAttackDamage);
                ChangeAnimation(PlayerAnim.AttackInAir1);
                _rb.velocity = new Vector2(0, 0.5f);
                _canAttackInAir = false;
                return true;
            }

            if (Input.GetMouseButtonDown(0) && _currentAnimation is PlayerAnim.AttackInAir1 && CheckAnimTime(0.5f))
            {
                Damage(lightAttackRange, lightAttackDamage);
                ChangeAnimation(PlayerAnim.AttackInAir2);
                _rb.velocity = new Vector2(4 * (int)faceOrientation, 0.5f);
                return true;
            }

            if (Input.GetMouseButtonDown(0) && _currentAnimation == PlayerAnim.AttackInAir2 && CheckAnimTime(0.5f))
            {
                ChangeAnimation(PlayerAnim.AttackInAir3);
                _rb.velocity = new Vector2(4 * (int)faceOrientation, 0.5f);
                spinningSword.Create();
                return true;
            }
        }

        if (_currentAnimation is PlayerAnim.Attack3 or PlayerAnim.AttackInAir3 && AnimPlaying() && !spinningSword.isActiveAndEnabled)
        {
            ChangeAnimation(_currentAnimation is PlayerAnim.Attack3 ? PlayerAnim.Attack3End : PlayerAnim.AttackInAir3End);
            return true;
        }
        
        if (_currentAnimation is PlayerAnim.Attack3 or PlayerAnim.AttackInAir3 && AnimCompleted() && spinningSword.isActiveAndEnabled)
        {
            ChangeAnimation(_currentAnimation is PlayerAnim.Attack3 ? PlayerAnim.Attack3End : PlayerAnim.AttackInAir3End);
            spinningSword.Destroy();
            return true;
        }

        if (_currentAnimation is PlayerAnim.Attack1 or PlayerAnim.Attack2 or PlayerAnim.Attack3 or PlayerAnim.Attack3End && AnimPlaying())
        {
            return true;
        }

        if (_currentAnimation is PlayerAnim.AttackInAir1 or PlayerAnim.AttackInAir2 or PlayerAnim.AttackInAir3 or PlayerAnim.AttackInAir3End &&
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
                ChangeAnimation(PlayerAnim.Move);
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
            ChangeAnimation(PlayerAnim.Jump);
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _onFoot = false;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _doubleJump && _currentAnimation is PlayerAnim.Jump && CheckAnimTime(0.5f))
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
            ChangeAnimation(PlayerAnim.Fall);
            return false;
        }

        if (_currentAnimation is PlayerAnim.Fall && _onFoot)
        {
            ChangeAnimation(PlayerAnim.FallEnd);
            return true;
        }

        if (_currentAnimation == PlayerAnim.FallEnd && AnimPlaying())
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
            if (_currentAnimation is PlayerAnim.FallAttackEnd && !AnimCompleted())
                return true;
            ChangeAnimation(PlayerAnim.Idle);
            return true;
        }

        return false;
    }

    #endregion
    
    private void Damage(float attackRadius, int damage)
    {
        var hitEnemies = Physics2D.OverlapCircleAll(swingSwordCollider.position, attackRadius, enemies);
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().GetDamage(damage);
        }

        var hitBullets = Physics2D.OverlapCircleAll(swingSwordCollider.position, attackRadius, enemyBullet);
        foreach (var enemy in hitBullets)
        {
            enemy.GetComponent<EnemyBullet>().Destroy();
        }
    }

    private void HeavyDamage(int damage, Transform damageCenter)
    {
        var hitEnemies = Physics2D.OverlapBoxAll(damageCenter.position, fallAttackRange, 0, enemies);
        foreach (var enemy in hitEnemies)
        {
            if (_hitEnemies.Contains(enemy))
                continue;
            enemy.GetComponent<Enemy>().GetDamage(damage);
            _hitEnemies.Add(enemy);
        }

        var hitBullets = Physics2D.OverlapBoxAll(fallSwordCollider.position, fallAttackRange, 0, enemyBullet);
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
            ChangeAnimation(PlayerAnim.GetDamagedClimb);
        else if (damageVector == 1 && faceOrientation is Side.Right || damageVector == -1 && faceOrientation is Side.Left)
            ChangeAnimation(_onFoot ? PlayerAnim.GetDamagedFromFront : PlayerAnim.GetDamagedInAirFromFront);
        else
            ChangeAnimation(_onFoot ? PlayerAnim.GetDamagedFromBehind : PlayerAnim.GetDamagedInAirFromBehind);
        
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
        _canAttack = true;
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
            _canAttackInAir = true;
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
                ChangeAnimation(PlayerAnim.Fall);
        }
    }

    #endregion

    #region Draw

    void OnDrawGizmosSelected()
    {
        var position = swingSwordCollider.position;
        Gizmos.DrawWireSphere(position, lightAttackRange);
        Gizmos.DrawWireSphere(position, heavyAttackRange);
        Gizmos.DrawWireCube(fallSwordCollider.position, fallAttackRange);
        Gizmos.DrawWireCube(fallEndSwordCollider.position, fallAttackRange);
    }

    #endregion
}