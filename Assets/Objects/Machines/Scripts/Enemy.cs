using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Enemy : MonoBehaviour
{
    protected static readonly Vector2 RightLocalScale = new(-1, 1);
    protected static readonly Vector2 LeftLocalScale = new(1, 1);

    [Header("Basic Settings")] protected Rigidbody2D Rb;
    protected Animator Animator;
    protected string CurrentAnimation;
    public GameObject player;
    [SerializeField] protected int hp;

    [Header("Move Settings")] [SerializeField]
    protected float brakingSpeed = 2;

    [SerializeField] protected float patrolSpeed = 3;
    [SerializeField] protected float chaseSpeed = 5;
    [SerializeField] protected Transform[] moveSpot;
    [SerializeField] protected float waitTime;
    protected float CurWaitTime;
    protected Side FaceOrientation;

    [Header("Attack Settings")] [SerializeField]
    protected int damage;

    [SerializeField] protected float attackRate;
    [SerializeField] protected int maxAttackRaduis;
    [SerializeField] protected int maxChaseRaduis;

    [Header("Destroying Settings")] [SerializeField]
    protected EnemyDestroying enemyDestroying;

    [SerializeField] protected Explosion explosion;
    [SerializeField] protected Transform explosionCenter;
    [SerializeField] protected float destructionTime;
    protected float CurDestructionTime;

    [Header("Other Settings")] [SerializeField]
    protected LayerMask layerGround;

    protected int CurId;
    protected bool ReverseGettingId;
    protected bool CanAttack;

    protected Vector2 EnemyToSpot => moveSpot[CurId].transform.position - Rb.transform.position;
    protected Vector2 EnemyToPlayer => player.transform.position - Rb.transform.position;

    protected virtual State GetState
        => hp <= 0
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
        CanAttack = true;
    }

    protected void HandleState()
    {
        var state = GetState;
        switch (state)
        {
            case State.Chase:
                Chase();
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Attack:
                Attack();
                break;
            case State.GoToScene:
                GoToScene();
                break;
            case State.Dead:
                Die();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected abstract void Patrol();
    protected abstract void Chase();
    protected abstract void Attack();
    protected abstract void GoToScene();
    protected abstract void Die();

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

    public virtual void GetDamage(int inputDamage)
    {
        hp -= inputDamage;
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
