using System;
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
    [SerializeField] private Transform attack1Collider;
    [SerializeField] private Transform attack2Collider;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask enemies;
    [SerializeField] private LayerMask enemyBullet;

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

    [Header("Health Settings")]
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    
    [Header("Other Settings")]
    [SerializeField] private float speed = 12;
    [SerializeField] private float jumpForce = 1200;
    [SerializeField] private float lightAttackRange;
    [SerializeField] private float heavyAttackRange;
    [SerializeField] private Vector2 fallAttackRange;
    [SerializeField] private int lightAttackDamage = 20;
    [SerializeField] private int heavyAttackDamage = 60;
    [SerializeField] private Side faceOrientation;

    private static readonly Vector3 RightLocalScale = new(1, 1);
    private static readonly Vector3 LeftLocalScale = new(-1, 1);

    private static float MovementAxis => Input.GetAxis("Horizontal");

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        StepClimb();

        if (_currentAnimation is "Get_Damaged_in_air_anim" && AnimPlaying())
        {
            return;
        }
        
        if (health <= 0)
            Die();
        
        if (heavySword.inHands && _currentAnimation is not "Fall_Attack_End_anim") heavySword.ReturnSword();
        if (lightSword.inHands) lightSword.ReturnSword();
        
        if (HeavyAttack())
        {
            spinningSword.Destroy();
            heavySword.DrawSword();
            return;
        }
        CancelInvoke(nameof(HeavyDamage));
        
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
            Damage(heavyAttackRange, heavyAttackDamage);
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
            else if (_rb.velocity.y < 0)
            {
                InvokeRepeating(nameof(HeavyDamage), 0, 0.2f);
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
                Damage(lightAttackRange, lightAttackDamage);
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
                Damage(lightAttackRange, lightAttackDamage);
                _attack = false;
                ChangeAttack(1);
                if (_rb.velocity.x > 20)
                    _rb.velocity = new Vector2(10 * (int)faceOrientation, _rb.velocity.y);
                ChangeAnimation("Attack_anim");
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0) && !_onFoot)
        {
            if (_attackInAir && _attack)
            {
                Damage(lightAttackRange, lightAttackDamage);
                ChangeAnimation("Attack_in_air_anim");
                _rb.velocity = new Vector2(0, 0.5f);
                _attackInAir = false;
                return true;
            }

            if (Input.GetMouseButtonDown(0) && _currentAnimation is "Attack_in_air_anim" && CheckAnimTime(0.5f))
            {
                Damage(lightAttackRange, lightAttackDamage);
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

    private void Die()
    {
        gameObject.SetActive(false);
    }

    private void StepClimb()
    {
        var hitLower = Physics2D.Raycast(stayRayLower.transform.position,
            Vector2.right*(int)faceOrientation, 0.8f, ground);
        if (hitLower.collider is not null && _rb.velocity.y >= 0 &&  _rb.velocity.x*(int)faceOrientation > 0.1f)
        {
            var hitUpper = Physics2D.Raycast(stayRayUpper.transform.position,
                Vector2.right*(int)faceOrientation, 0.8f, ground);
            if (hitUpper.collider is null)
            {
                _rb.position -= new Vector2(-stepLenght*(int)faceOrientation, -stepHeight);
            }
        }
    }
    
    private void Damage(float attackRadius, int damage)
    {
        var hitEnemies = Physics2D.OverlapCircleAll(attack1Collider.position, attackRadius, enemies);
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<SmallFlyer>().GetDamage(damage);
        }
        var hitBullets = Physics2D.OverlapCircleAll(attack1Collider.position, attackRadius, enemyBullet);
        foreach (var enemy in hitBullets)
        {
            enemy.GetComponent<EnemyBullet>().Destroy();
        }
    }
    
    private void HeavyDamage()
    {
        var hitEnemies = Physics2D.OverlapBoxAll(attack2Collider.position, fallAttackRange, enemies);
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<SmallFlyer>().GetDamage(heavyAttackDamage);
        }
        var hitBullets = Physics2D.OverlapBoxAll(attack2Collider.position, fallAttackRange, enemies);
        foreach (var enemy in hitBullets)
        {
            enemy.GetComponent<EnemyBullet>().Destroy();
        }
    }

    public void GetDamage(int inputDamage, Transform attackVector)
    {
        health -= inputDamage;
        var damageVector = (transform.position - attackVector.position).x >= 0 ? -1 : 1;
        if (!_onFoot)
        {
            ChangeAnimation("Get_Damaged_in_air_anim");
        }
        _rb.velocity -= new Vector2(Math.Min(inputDamage / 20, 5) * damageVector, 0);
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
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Stairs"))
        {
            _onFoot = true;
            _attackInAir = true;
            _fallAttack = true;
            _doubleJump = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Stairs"))
        {
            _onFoot = false;
        }
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attack1Collider.position, lightAttackRange);
        Gizmos.DrawWireCube(attack2Collider.position, fallAttackRange);
    }
}