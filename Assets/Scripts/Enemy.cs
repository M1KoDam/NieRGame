using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static readonly Vector2 RightLocalScale = new(-1, 1);
    protected static readonly Vector2 LeftLocalScale = new(1, 1);
    
    [Header("Basic Settings")] 
    protected Rigidbody2D Rb;
    protected Animator Animator;
    public Player player;
    [SerializeField] protected int hp;

    [Header("Move Settings")]
    [SerializeField] protected Transform[] moveSpot;
    [SerializeField] protected float waitTime;
    protected float CurWaitTime;
    protected Side FaceOrientation;
    
    [Header("Attack Settings")]
    [SerializeField] protected float damage;
    [SerializeField] protected float attackRate;
    [SerializeField] protected int maxAttackRaduis;
    [SerializeField] protected int maxChaseRaduis;

    [Header("Destroying Settings")] 
    [SerializeField] protected EnemyDestroying enemyDestroying;
    [SerializeField] protected Explosion explosion;
    [SerializeField] protected Transform explosionCenter;
    [SerializeField] protected float destructionTime;
    protected float CurDestructionTime;

    [Header("Other Settings")] 
    [SerializeField] protected LayerMask layerGround;
    protected int CurId;
    protected bool ReverseGettingId;
    protected bool CanAttack;
    protected float AttackTimer;
    
    protected const float BrakingSpeed = 2;
    protected const float PatrolSpeed = 3;
    protected const float ChaseSpeed = 5;
    
    protected float AttackDelay => 1 / attackRate; 
    protected Vector2 EnemyToSpot => moveSpot[CurId].transform.position - Rb.transform.position;
    protected Vector2 EnemyToPlayer => player.transform.position - Rb.transform.position;
    protected State GetState
        =>  hp <= 0 
            ? State.Dead 
            : EnemyToPlayer.magnitude <= maxAttackRaduis && Physics2D.Raycast(transform.position, 
                EnemyToPlayer, EnemyToPlayer.magnitude, layerGround).collider is null
                ? State.Attack
                : EnemyToPlayer.magnitude <= maxChaseRaduis
                    ? State.Chase
                    : State.Patrol;

    // Start is called before the first frame update
    protected void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        FaceOrientation = Side.Left;
        CurWaitTime = waitTime;
        CurDestructionTime = destructionTime;
    }
    
    protected void Brake()
    {
        Rb.velocity /= BrakingSpeed;
    }

    public virtual void GetDamage(int damage)
    {
        hp -= damage;
    }
}
