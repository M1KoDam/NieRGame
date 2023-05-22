using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SmallFlyer : Enemy
{
    private static readonly Vector2 LeftOrientationShootingPosition = new(9, 4f);
    private static readonly Vector2 RightOrientationShootingPosition = new(-9, 4f);
    
    [Header("Gun Settings")]
    [SerializeField] private EnemyBullet bullet;
    [SerializeField] private Transform gun;
    
    private float _angle;
    
    private bool _isScoping;
    private bool _swayDown;
    private int _swayCount;

    private Vector2 BulletPosition => gun.transform.position;

    private Vector2 ShootingPositionToPlayer => FaceOrientation is Side.Left
        ? EnemyToPlayer + LeftOrientationShootingPosition
        : EnemyToPlayer + RightOrientationShootingPosition;

    // Update is called once per frame
    private void Update()
    {
        if (GetState == State.Dead)
            return;
        
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;   

        Rb.MoveRotation(_angle);
    }
    
    private void FixedUpdate()
    {
        _swayCount += 1;

        var state = GetState;
        switch (state)
        {
            case State.Chase:
                Chasing();
                break;
            case State.Patrol:
                Patrolling();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Dead:
                Die();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Rb.velocity += Sway();
        ChangeFaceOrientation();
    }
    
    #region Patrol 
    
    private void Patrolling()
    {
        _isScoping = false;
        if (EnemyToSpot.magnitude < 1f)
        {
            if (CurWaitTime <= 0)
                ChangeSpotId();
            else
            {
                CurWaitTime -= Time.deltaTime;
                Wait();
                Brake();
            }
        }

        else
            GoToSpot();

        RestoreAngle();
    }
    
    private void ChangeSpotId()
    {
        CurId = ReverseGettingId ? CurId - 1 : CurId + 1;

        if (CurId >= moveSpot.Length || CurId < 0)
        {
            ReverseGettingId = !ReverseGettingId;
            CurId = ReverseGettingId ? moveSpot.Length - 1 : 0;
        }

        CurWaitTime = waitTime;
    }
    
    #endregion

    #region Chase

    private void Chasing()
    {
        _isScoping = false;
        GoToPlayer();
        RestoreAngle();
    }

    #endregion

    #region Attack

    private void Attack()
    {
        _isScoping = true;
        GoToShootingPosition();
        LookAtPlayer();
        if (CanAttack)
        {
            CanAttack = false;
            Shoot();
            Invoke(nameof(WaitForAttack), attackRate);
        }
    }
    
    private void Shoot()
    {
        var bul = Instantiate(bullet, BulletPosition, transform.rotation);
        bul.GetComponent<Rigidbody2D>().velocity = EnemyToPlayer.normalized * bul.bulletSpeed;
        Destroy(bul.gameObject, 5f); ;
    }

    #endregion

    #region FaceOrientation

    private void ChangeFaceOrientation()
    {
        FaceOrientation = _isScoping
            ? -90 <= _angle && _angle <= 90
                ? Side.Left
                : Side.Right
            : Rb.velocity.x < 0
                ? Side.Left 
                : Rb.velocity.x > 0 
                    ? Side.Right 
                    : FaceOrientation;
    }

    #endregion

    #region Move

    private void GoToPlayer()
    {
        Rb.velocity = EnemyToPlayer.normalized * chaseSpeed;
    }
    
    private void GoToSpot()
    {
        Rb.velocity = EnemyToSpot.normalized * patrolSpeed;
    }
    
    private void GoToShootingPosition()
    {
        if (ShootingPositionToPlayer.magnitude < 2f)
            Brake();
        else
            Rb.velocity = ShootingPositionToPlayer.normalized * chaseSpeed;
    }
    
    private Vector2 Sway()
    {
        if (_swayCount > 60)
        {
            _swayCount = 0;
            _swayDown = !_swayDown;
        }

        if (_swayCount < 30) return Vector2.zero;
        return _swayDown ? new Vector2(0, -0.5f) : new Vector2(0, 0.5f);
    }
    
    private void Wait()
    {
        Rb.velocity = new Vector2(0, 0.2f);
    }

    #endregion
    
    #region Angle
    
    private void RestoreAngle()
    {
        if (_angle is < 270 and > 90 or < -270 and > -90)
            _angle = 90;
        
        if (_angle is >= 270 and <= 360 or <= -270 and >= -360)
        {
            _angle = _angle >= 270 
                ? _angle + 15 
                : _angle - 15;
            if (360 - Math.Abs(_angle) < 30)
                _angle = 360;
        }
        
        if (_angle is <= 90 and >= -90)
        {  
            _angle /= 2;
            if (Math.Abs(_angle) < 5)
                _angle = 0;
        }
    }
    
    private void LookAtPlayer()
    {
        var angle = -Vector2.SignedAngle(EnemyToPlayer, Vector2.left);
        if (-90 <= angle && angle <= 90)
            _angle = angle;
        else if (angle > 90)
            _angle = angle + 180;
        else if (angle < -90)
            _angle = angle - 180;
    }
    
    #endregion

    #region GetDamage

    private void Die()
    {
        Animator.Play("FlyerDestroy");
        if (CurDestructionTime <= 0)
        {
            if (FaceOrientation is Side.Right)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }
            var tempPosition = transform.position;
            var tempRotation = transform.rotation;
            
            Destroy(gameObject);

            var smallFlyerDestroyingCopy = Instantiate(enemyDestroying, tempPosition, tempRotation);
            smallFlyerDestroyingCopy.Activate();
            Destroy(smallFlyerDestroyingCopy.gameObject, 5f);
            
            var smallFlyerExplosion = Instantiate(explosion, explosionCenter.position, tempRotation);
            smallFlyerExplosion.Explode();
        }
        else
            CurDestructionTime -= Time.deltaTime;
    }
    
    public override void GetDamage(int inputDamage)
    {
        _angle -= Math.Min(20, inputDamage/4);
        hp -= inputDamage;
    }

    #endregion
    
    void OnDrawGizmosSelected()
    {
        var position = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, maxAttackRaduis);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, maxChaseRaduis);
    }
}
