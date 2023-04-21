using System;
using Unity.VisualScripting;
using UnityEngine;

public class SmallFlyer : MonoBehaviour
{
    private Rigidbody2D _rb;
    public Player player;
    public Bullet bullet;

    public Transform[] moveSpot;
    public int curId;

    public float waitTime;
    private float _time;
    private bool _isScoping;

    private const float BrakingSpeed = 3;
    private const float PatrolSpeed = 3;
    private const float ChaseSpeed = 5;

    private Vector2 _velocity;
    private float _angle;
    
    private static readonly Vector2 LeftShootingPosition = new(8, 3f);
    private static readonly Vector2 RightShootingPosition = new(-8, 3f);

    private Side FaceOrientation
        => _isScoping
            ? -90 <= _angle && _angle <= 90
                ? Side.Left
                : Side.Right
            : _velocity.x > 0
                ? Side.Right
                : Side.Left;

    private State GetState 
        => SmallFlyerToPlayer.magnitude > 10 && SmallFlyerToPlayer.magnitude < 20 
            ? State.Chase
            : SmallFlyerToPlayer.magnitude <= 10
                ? State.Attack
                : State.Patrol;

    private static readonly Vector2 RightLocalScale = new(-1, 1);
    private static readonly Vector2 LeftLocalScale = new(1, 1);

    private Vector2 SmallFlyerToSpot => moveSpot[curId].transform.position - _rb.transform.position;
    private Vector2 SmallFlyerToPlayer => player.transform.position - _rb.transform.position;
    private Vector2 BulletPosition => (Vector2)_rb.transform.position + SmallFlyerToPlayer.normalized;

    private Vector2 ShootingPosition => FaceOrientation is Side.Left
        ? (Vector2)player.transform.position + LeftShootingPosition
        : (Vector2)player.transform.position + RightShootingPosition;

    private bool _swayDown;
    private int _swayCount;

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
        _time = waitTime;
    }
    
    private void Wait()
    {
        _velocity = new Vector2(0, 0.2f);
    }

    private void RestoreAngle()
    {
        if (_angle != 0)
            _angle /= 2;
        if (_angle < 1f)
            _angle = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;
        
        _rb.MoveRotation(_angle);
        _rb.velocity = _velocity + Sway();
    }

    private void FixedUpdate()
    {
        _swayCount += 1;
        var state = GetState;
        Debug.Log(SmallFlyerToPlayer.magnitude);
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void Patrolling()
    {
        _isScoping = false;
        if (SmallFlyerToSpot.magnitude < 1f)
        {
            if (_time <= 0)
                ChangeSpotId();
            else
            {
                _time -= Time.deltaTime;
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
        //LookAtPlayer();
        //Shoot();
    }

    private void GoToShootingPosition()
    {
        if (((Vector2)_rb.transform.position - ShootingPosition).magnitude < 2f)
            Brake();
        else 
            _velocity = ShootingPosition.normalized * ChaseSpeed;
    }

    private void Shoot()
    {
        var bul = Instantiate(bullet, BulletPosition, transform.rotation);
        Destroy(bul.gameObject, 5f);
    }

    private void LookAtPlayer()
    {
        //var strongVector = FaceOrientation == Side.Left ? Vector2.left : Vector2.right;
        var angle = -Vector2.SignedAngle(SmallFlyerToPlayer, Vector2.right);
        Debug.Log(angle);
        /*
        if (-90 <= angle && angle <= 90)
            _angle = angle;
        else if (angle > 90)
            _angle = angle + 180;
        else if (angle < -90)
            _angle = angle - 180;
            */
    }

    private void GoToPlayer()
    {
        _velocity = SmallFlyerToPlayer.normalized * ChaseSpeed;
    }

    private void ChangeSpotId()
    {
        curId++;
        
        if (curId >= moveSpot.Length)
            curId = 0;

        _time = waitTime;
    }

    private void GoToSpot()
    { 
        _velocity = SmallFlyerToSpot.normalized * PatrolSpeed;
    }
    
    private void Brake()
    {
        _velocity /= BrakingSpeed;
    }
}
