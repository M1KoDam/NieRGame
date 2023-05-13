using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SmallFlyer : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    public Player player;
    public EnemyBullet bullet;
    public Transform gun;
    [SerializeField] private int hp;
    [SerializeField] private SmallFlyerDestroying smallFlyerDestroying;
    [SerializeField] private Explosion explosion;

    public Transform[] moveSpot;
    private int curId;
    private bool reverseGettingId;
    private bool _canShoot;

    public float waitTime;
    private float _curWaitTime;
    public float destructionTime;
    private float _curDestructionTime;
    private bool _isScoping;

    private const float BrakingSpeed = 2;
    private const float PatrolSpeed = 3;
    private const float ChaseSpeed = 5;
    [SerializeField] private float fireRate;
    [SerializeField] private float damage;

    private Vector2 _velocity;
    private float _angle;

    private static readonly Vector2 LeftOrientationShootingPosition = new(9, 4f);
    private static readonly Vector2 RightOrientationShootingPosition = new(-9, 4f);

    private Side FaceOrientation
        => _isScoping
            ? -90 <= _angle && _angle <= 90
                ? Side.Left
                : Side.Right
            : _velocity.x < 0
                ? Side.Left
                : Side.Right;

    private State GetState
        =>  hp <= 0 
            ? State.Dead 
            : SmallFlyerToPlayer.magnitude > 15 && SmallFlyerToPlayer.magnitude < 25
                ? State.Chase
                : SmallFlyerToPlayer.magnitude <= 15
                    ? State.Attack
                    : State.Patrol;

    private static readonly Vector2 RightLocalScale = new(-1, 1);
    private static readonly Vector2 LeftLocalScale = new(1, 1);

    private float FireDelay => 1 / fireRate;
    private Vector2 SmallFlyerToSpot => moveSpot[curId].transform.position - _rb.transform.position;
    private Vector2 SmallFlyerToPlayer => player.transform.position - _rb.transform.position;
    private Vector2 BulletPosition => gun.transform.position;

    private Vector2 ShootingPositionToPlayer => FaceOrientation is Side.Left
        ? SmallFlyerToPlayer + LeftOrientationShootingPosition
        : SmallFlyerToPlayer + RightOrientationShootingPosition;


    private bool _swayDown;
    private int _swayCount;
    private float _fireTimer;

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

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _curWaitTime = waitTime;
        _curDestructionTime = destructionTime;
    }

    private void Wait()
    {
        _velocity = new Vector2(0, 0.2f);
    }

    private void RestoreAngle()
    {
        if (_angle != 0)
            _angle /= 2;
        if ((_angle is > 0 and < 1f or < 0 and > -1f))
            _angle = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GetState == State.Dead)
            return;
        
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;

        _rb.MoveRotation(_angle);
        _rb.velocity = _velocity + Sway();
    }

    private void FixedUpdate()
    {
        _swayCount += 1;  
        HandleFireRate();
        
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
                Attacking();
                break;
            case State.Dead:
                Die();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

private void Patrolling()
    {
        _isScoping = false;
        if (SmallFlyerToSpot.magnitude < 1f)
        {
            if (_curWaitTime <= 0)
                ChangeSpotId();
            else
            {
                _curWaitTime -= Time.deltaTime;
                Wait();
                Brake();
            }
        }

        else
            GoToSpot();

        RestoreAngle();
    }

    private void Chasing()
    {
        _isScoping = false;
        GoToPlayer();
        RestoreAngle();
    }

    private void Attacking()
    {
        _isScoping = true;
        GoToShootingPosition();
        LookAtPlayer();
        if (_canShoot)
            Shoot();
    }

    private void GoToShootingPosition()
    {
        if (ShootingPositionToPlayer.magnitude < 2f)
            Brake();
        else
            _velocity = ShootingPositionToPlayer.normalized * ChaseSpeed;
    }

    private void Shoot()
    {
        var bul = Instantiate(bullet, BulletPosition, transform.rotation);
        bul.GetComponent<Rigidbody2D>().velocity = SmallFlyerToPlayer.normalized * bul.bulletSpeed;
        Destroy(bul.gameObject, 5f);

        _canShoot = false;
    }
    
    private void HandleFireRate()
    {
        if (_fireTimer < FireDelay)
        {
            _fireTimer += Time.fixedDeltaTime;
        }
        else
        {
            _canShoot = true;
            _fireTimer = 0;
        }
    }

    private void LookAtPlayer()
    {
        var angle = -Vector2.SignedAngle(SmallFlyerToPlayer, Vector2.left);
        if (-90 <= angle && angle <= 90)
            _angle = angle;
        else if (angle > 90)
            _angle = angle + 180;
        else if (angle < -90)
            _angle = angle - 180;
    }

    private void GoToPlayer()
    {
        _velocity = SmallFlyerToPlayer.normalized * ChaseSpeed;
    }

    private void ChangeSpotId()
    {
        curId = reverseGettingId ? curId - 1 : curId + 1;

        if (curId >= moveSpot.Length || curId < 0)
        {
            reverseGettingId = !reverseGettingId;
            curId = reverseGettingId ? moveSpot.Length - 1 : 0;
        }

        _curWaitTime = waitTime;
    }

    private void GoToSpot()
    {
        _velocity = SmallFlyerToSpot.normalized * PatrolSpeed;
    }

    private void Brake()
    {
        _velocity /= BrakingSpeed;
    }

    private void Die()
    {
        _animator.Play("Destroy");
        if (_curDestructionTime <= 0)
        {
            if (FaceOrientation is Side.Right)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }
            var tempPosition = transform.position;
            var tempRotation = transform.rotation;
            
            Destroy(gameObject);

            var smallFlyerDestroyingCopy = Instantiate(smallFlyerDestroying, tempPosition, tempRotation);
            smallFlyerDestroyingCopy.Activate();
            Destroy(smallFlyerDestroyingCopy.gameObject, 5f);
            
            var smallFlyerExplosion = Instantiate(explosion, tempPosition + Vector3.down, tempRotation);
            smallFlyerExplosion.Explode();
        }
        else
            _curDestructionTime -= Time.deltaTime;
    }
    
    public void GetDamage(int damage)
    {
        _angle -= Math.Min(20, damage/4);
        hp -= damage;
    }
}