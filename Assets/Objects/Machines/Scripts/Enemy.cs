using UnityEditor.Overlays;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected static readonly Vector2 RightLocalScale = new(-1, 1);
    protected static readonly Vector2 LeftLocalScale = new(1, 1);

    [Header("Basic Settings")]
    protected Rigidbody2D Rb;
    protected Animator Animator;
    protected string CurrentAnimation;
    public GameObject player;
    [SerializeField] protected int hp;

    [Header("Move Settings")]
    [SerializeField] protected float brakingSpeed = 2;

    [SerializeField] protected float patrolSpeed = 3;
    [SerializeField] protected float chaseSpeed = 5;
    [SerializeField] protected Transform[] moveSpot;
    [SerializeField] protected float waitTime;
    protected float CurWaitTime;
    protected Side FaceOrientation;

    [Header("Attack Settings")]
    [SerializeField] protected int damage;

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
    protected bool IsDamaged;

    protected Vector2 EnemyToSpot => moveSpot[CurId].transform.position - Rb.transform.position;
    protected Vector2 EnemyToPlayer => player.transform.position - Rb.transform.position;

    protected virtual IState state
        => hp <= 0
            ? new DeadState()
            : EnemyToPlayer.magnitude <= maxAttackRaduis && Physics2D.Raycast(transform.position,
                EnemyToPlayer, EnemyToPlayer.magnitude, layerGround).collider is null
                ? new AttackState()
                : EnemyToPlayer.magnitude <= maxChaseRaduis
                    ? new ChaseState()
                    : new PatrolState();

    protected virtual void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        FaceOrientation = Side.Left;
        CurWaitTime = waitTime;
        CurDestructionTime = destructionTime;
        CanAttack = true;
    }

    public abstract void Patrol();
    public abstract void Chase();
    public abstract void Attack();
    public abstract void GoToScene();
    public abstract void Die();

    protected virtual Side GetFaceOrientation() =>
        Rb.velocity.x < 0
            ? Side.Left
            : Rb.velocity.x > 0
                ? Side.Right
                : FaceOrientation;

    protected void ChangeSpotId()
    {
        CurId = ReverseGettingId ? CurId - 1 : CurId + 1;

        if (CurId >= moveSpot.Length || CurId < 0)
        {
            ReverseGettingId = !ReverseGettingId;
            CurId = ReverseGettingId ? moveSpot.Length - 1 : 0;
        }

        CurWaitTime = waitTime;
    }

    protected void Brake()
    {
        Rb.velocity /= brakingSpeed;
    }

    public virtual void GetDamage(int inputDamage, Transform attackVector)
    {
        if (GetState is State.Dead)
            return;
        
        hp -= inputDamage;
        
        var damageVector = (transform.position - attackVector.position).x >= 0 ? -1 : 1;
        Rb.velocity = new Vector2(-Math.Min(inputDamage / 5, 5) * damageVector, 0);
        Rb.AddForce(new Vector2(0, Math.Min(inputDamage * 500, 20000)));
        
        IsDamaged = true;
        Invoke(nameof(RemoveDamaged), 2);
    }

    private void RemoveDamaged()
    {
        IsDamaged = false;
    }

    protected void WaitForAttack()
    {
        CanAttack = true;
    }

    #region Animation

    protected bool AnimPlaying(float time = 1)
    {
        return Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < time;
    }

    protected bool AnimCompleted()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    protected bool CheckAnimTime(float time)
    {
        return Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > time;
    }

    protected void ChangeAnimation(string anim)
    {
        if (CurrentAnimation == anim)
            return;

        CurrentAnimation = anim;
        Animator.Play(anim);
    }

    #endregion

    protected virtual void OnDrawGizmosSelected()
    {
        var position = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, maxAttackRaduis);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, maxChaseRaduis);
    }
}