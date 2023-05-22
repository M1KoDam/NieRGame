using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SmallStubby: Enemy
{
    [SerializeField] private Transform attackCollider;
    [SerializeField] private LayerMask pLayerLayer;
    [SerializeField] private Vector2 attackRadius = new (3, 1);

    [Header("Step Climb Settings")]
    [SerializeField] private GameObject stayRayUpper;
    [SerializeField] private GameObject stayRayLower;

    // Update is called once per frame
    private void Update()
    {
        if (CurrentAnimation is not "StubbyStartAttack" or "StubbyAttack")
        {
            StepClimb();
        }
        
        if (GetState == State.Dead)
            return;
        
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;
    }
    
    private void FixedUpdate()
    {
        if (CurrentAnimation is "StubbyStartAttack" or "StubbyAttack")
        {
            Attack();
            return;
        }

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
            case State.Dead:
                Die();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        ChangeFaceOrientation();
    }

    #region Patrol
    
    private void Patrol()
    {
        if (EnemyToSpot.magnitude < 1f)
        {
            if (CurWaitTime <= 0)
                ChangeSpotId();
            else
            {
                CurWaitTime -= Time.deltaTime;
                Brake();
            }
        }

        else
            GoTo(EnemyToSpot, patrolSpeed);
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
    
    private void Chase()
    {
        GoTo(EnemyToPlayer, chaseSpeed);
    }
    
    #endregion
    
    #region Attack
    private void Attack()
    {
        if (CanAttack)
        {
            CanAttack = false;
            ChangeAnimation("StubbyStartAttack");
            Invoke(nameof(WaitForAttack), attackRate);
            return;
        }
        
        if (CurrentAnimation is "StubbyStartAttack" && AnimCompleted())
        {
            ChangeAnimation("StubbyAttack");
            var hitPlayer = Physics2D.OverlapBox(attackCollider.position, attackRadius, 0, pLayerLayer);
            if (hitPlayer)
                hitPlayer.GetComponent<Player>().GetDamage(damage, transform);
            return;
        }
        
        if (CurrentAnimation is "StubbyAttack" && AnimCompleted())
        {
            ChangeAnimation("StubbyIdle");
        }
    }

    #endregion
    
    #region FaceOrientation
    
    private void ChangeFaceOrientation()
    {
        FaceOrientation = GetState is State.Attack
            ? EnemyToPlayer.x > 0 
                ? Side.Right 
                : Side.Left
            : Rb.velocity.x < 0
                ? Side.Left
                : Rb.velocity.x > 0
                    ? Side.Right
                    : FaceOrientation;
    }
    
    #endregion
    
    #region Move
    
    private void StepClimb()
    {
        var hitLower = Physics2D.Raycast(stayRayLower.transform.position,
            Vector2.right * (int)FaceOrientation, 1, layerGround);
        if (hitLower.collider)
        {
            var hitUpper = Physics2D.Raycast(stayRayUpper.transform.position,
                Vector2.right * (int)FaceOrientation, 1, layerGround);
            if (!hitUpper.collider)
            {
                Rb.position -= new Vector2(-0.01f * (int)FaceOrientation, -0.005f);
            }
        }
    }

    private void GoTo(Vector2 distance, float speed)
    {
        ChangeAnimation(Math.Abs(Rb.velocity.x) > 0.1f ? "StubbyMovement" : "StubbyIdle");
        Rb.velocity = new Vector2(distance.normalized.x * speed, Rb.velocity.y);
    }

    #endregion

    #region GetDamage

    private void Die()
    {
        Rb.freezeRotation = false;
        ChangeAnimation("StubbyDestroy");
        if (CurDestructionTime <= 0)
        {
            if (FaceOrientation is Side.Right)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }
            var tempPosition = transform.position;
            var tempRotation = transform.rotation;
            
            Destroy(gameObject);

            var smallStubbyDestroyingCopy = Instantiate(enemyDestroying, tempPosition, tempRotation);
            smallStubbyDestroyingCopy.Activate();
            Destroy(smallStubbyDestroyingCopy.gameObject, 5f);
            
            var smallStubbyExplosion = Instantiate(explosion, explosionCenter.position, tempRotation);
            smallStubbyExplosion.force = 15000;
            smallStubbyExplosion.Explode();
        }
        else
            CurDestructionTime -= Time.deltaTime;
    }
    
    #endregion
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(attackCollider.position, attackRadius);
        var position = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, maxAttackRaduis);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, maxChaseRaduis);
    }
}
