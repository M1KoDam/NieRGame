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
        if (State is DeadState || IsDamaged)
            return;
        
        if (CurrentAnimation is not "StubbyStartAttack" or "StubbyAttack")
        {
            StepClimb();
        }

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

        State.Execute(this);
        FaceOrientation = GetFaceOrientation();
    }

    public override void Patrol()
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

    public override void Chase()
    {
        GoTo(EnemyToPlayer, chaseSpeed);
    }

    public override void Attack()
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

    protected override Side GetFaceOrientation() =>
        State is AttackState
            ? EnemyToPlayer.x > 0
                ? Side.Right
                : Side.Left
            : base.GetFaceOrientation();

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

    public override void GoToScene()
    {
        throw new Exception("this type of enemy don't support 'GoToScene' work mode");
    }

    #endregion
    
    public override void DoIdle()
    {
        throw new Exception("this type of enemy don't support 'DoIdle' work mode");
    }

    #region GetDamage

    public override void Die()
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
    
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(attackCollider.position, attackRadius);
        base.OnDrawGizmosSelected();
    }
}
