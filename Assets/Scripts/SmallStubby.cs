using System;
using System.Reflection.Emit;
using UnityEngine;

public class SmallStubby: MonoBehaviour
{
    private Rigidbody2D _rb;
    public Player player;
    [SerializeField] private int hp;

    public Transform[] moveSpot;
    private int curId;
    private bool reverseGettingId;
    private bool _canBeat;

    public float waitTime;
    private float _curWaitTime;
    public float destructionTime;
    private float _curDestructionTime;

    private const float BrakingSpeed = 2;
    private const float PatrolSpeed = 3;
    private const float ChaseSpeed = 5;
    [SerializeField] private float fireRate; // частота атаки 
    [SerializeField] private float damage;

    private Vector2 _velocity;

    private Side FaceOrientation =>
        _velocity.x < 0
            ? Side.Left
            : Side.Right;

    private State GetState
        =>  hp <= 0 
            ? State.Dead 
            : SmallStubbyToPlayer.magnitude > 1 && SmallStubbyToPlayer.magnitude < 25
                ? State.Chase
                : SmallStubbyToPlayer.magnitude <= 1
                    ? State.Attack
                    : State.Patrol;

    private static readonly Vector2 RightLocalScale = new(-1, 1);
    private static readonly Vector2 LeftLocalScale = new(1, 1);

    private float FireDelay => 1 / fireRate; // переписать
    private Vector2 SmallStubbyToSpot => moveSpot[curId].transform.position - _rb.transform.position;
    private Vector2 SmallStubbyToPlayer => player.transform.position - _rb.transform.position;


    private float _fireTimer; // аналогично
   
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _curWaitTime = waitTime;
        _curDestructionTime = destructionTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GetState == State.Dead)
            return;
        
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;

        _rb.velocity = _velocity;
    }
    
    private void FixedUpdate()
    {
        //HandleFireRate();
        
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
                Attacking(); // <-----------
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
        if (SmallStubbyToSpot.magnitude < 1f)
        {
            if (_curWaitTime <= 0)
                ChangeSpotId();
            else
            {
                _curWaitTime -= Time.deltaTime;
                Brake();
            }
        }

        else
            GoToSpot();
    }

    private void Chasing()
    {
        GoToPlayer();
    }

    private void Attacking()
    {
        GoToPlayer();
        if (_canBeat)
            Beat();
    }

    private void Jump()
    {
        // создать как у плеера 2 нижних точки, чтобы по лестницам умел ходить, ну и ещё jump из плеера
    }

    private void Beat()
    {
        //запускай анимацию, ну как ты умеешь
    }
    
    private void HandleFireRate() //для задержки атаки, но я это место пока не трогал
    {
        if (_fireTimer < FireDelay)
        {
            _fireTimer += Time.fixedDeltaTime;
        }
        else
        {
            _canBeat = true;
            _fireTimer = 0;
        }
    }

    private void GoToPlayer()
    {
        _velocity = new Vector2(SmallStubbyToPlayer.normalized.x * ChaseSpeed, _rb.velocity.y);
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
        _velocity = new Vector2(SmallStubbyToSpot.normalized.x * PatrolSpeed, _rb.velocity.y);
    }

    private void Brake()
    {
        _velocity /= BrakingSpeed;
    }

    private void Die()
    {
        if (_curDestructionTime <= 0)
            Destroy(gameObject);
        else
            _curDestructionTime -= Time.deltaTime;
    }
    
    public void GetDamage(int damage)
    {
        hp -= damage;
    }
}

