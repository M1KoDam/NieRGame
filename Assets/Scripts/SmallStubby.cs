using System;
using System.Reflection.Emit;
using UnityEngine;

public class SmallStubby: Enemy
{
    private Rigidbody2D _rb;
    private Animator _animator;
    public Player player;

    public Transform[] moveSpot;
    private int curId;
    private bool reverseGettingId;
    private bool _canAttack = true;

    public float waitTime;
    private float _curWaitTime;
    public float destructionTime;
    private float _curDestructionTime;

    private const float BrakingSpeed = 2;
    private const float PatrolSpeed = 3;
    private const float ChaseSpeed = 5;
    [SerializeField] private float fireRate; // частота атаки 
    [SerializeField] private float damage;
    [SerializeField] private GameObject stayRayUpper;
    [SerializeField] private GameObject stayRayLower;
    [SerializeField] private LayerMask layerGround;

    private Side FaceOrientation =>
        _rb.velocity.x < 0
            ? Side.Left
            : Side.Right;

    private State GetState
        =>  hp <= 0 
            ? State.Dead 
            : SmallStubbyToPlayer.magnitude <= 2 && Physics2D.Raycast(transform.position, 
                SmallStubbyToPlayer, SmallStubbyToPlayer.magnitude, layerGround).collider is null
                ? State.Attack
                : SmallStubbyToPlayer.magnitude < 25
                    ? State.Chase
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
        _animator = GetComponent<Animator>();
        _curWaitTime = waitTime;
        _curDestructionTime = destructionTime;
    }

    // Update is called once per frame
    private void Update()
    {
        StepClimb();
        
        if (GetState == State.Dead)
            return;
        
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;
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
    
    private void StepClimb()
    {
        var hitLower = Physics2D.Raycast(stayRayLower.transform.position,
            Vector2.right * (int)FaceOrientation, 1, layerGround);
        if (hitLower.collider && _rb.velocity.y >= 0 && _rb.velocity.x * (int)FaceOrientation > 0.1f)
        {
            var hitUpper = Physics2D.Raycast(stayRayUpper.transform.position,
                Vector2.right * (int)FaceOrientation, 0.8f, layerGround);
            if (true)
            {
                _rb.position -= new Vector2(-0.05f * (int)FaceOrientation, -0.025f);
            }
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
        if (_canAttack)
            Attack();
    }

    private void Attack()
    {
        Debug.Log(true);
        _animator.Play("StubbyAttack");
    }
    
    private void HandleFireRate() //для задержки атаки, но я это место пока не трогал
    {
        if (_fireTimer < FireDelay)
        {
            _fireTimer += Time.fixedDeltaTime;
        }
        else
        {
            _canAttack = true;
            _fireTimer = 0;
        }
    }

    private void GoToPlayer()
    {
        _rb.velocity = new Vector2(SmallStubbyToPlayer.normalized.x * ChaseSpeed, _rb.velocity.y);
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
        _rb.velocity = new Vector2(SmallStubbyToSpot.normalized.x * PatrolSpeed, _rb.velocity.y);
    }

    private void Brake()
    {
        _rb.velocity /= BrakingSpeed;
    }

    private void Die()
    {
        if (_curDestructionTime <= 0)
            Destroy(gameObject);
        else
            _curDestructionTime -= Time.deltaTime;
    }
}

