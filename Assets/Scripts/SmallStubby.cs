using System;
using UnityEngine;

public class SmallStubby: Enemy
{
    [Header("Step Climb Settings")]
    [SerializeField] private GameObject stayRayUpper;
    [SerializeField] private GameObject stayRayLower;
    
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
            GoToSpot();
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
        GoToPlayer();
    }
    
    #endregion
    
    #region Attack
    private void Attack()
    {
        GoToPlayer();
        if (CanAttack)
            AttackAnimation();
    }

    private void AttackAnimation()
    {
        Animator.Play("StubbyAttack");
    }
    
    private void HandleAttackRate()
    {
        if (AttackTimer < AttackDelay)
        {
            AttackTimer += Time.fixedDeltaTime;
        }
        else
        {
            CanAttack = true;
            AttackTimer = 0;
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
        if (hitLower.collider && Rb.velocity.y >= 0 && Rb.velocity.x * (int)FaceOrientation > 0.1f)
        {
            var hitUpper = Physics2D.Raycast(stayRayUpper.transform.position,
                Vector2.right * (int)FaceOrientation, 0.8f, layerGround);
            if (!hitUpper.collider)
            {
                Rb.position -= new Vector2(-0.01f * (int)FaceOrientation, -0.005f);
            }
        }
    }
    
    private void GoToPlayer()
    {
        Rb.velocity = new Vector2(EnemyToPlayer.normalized.x * ChaseSpeed, Rb.velocity.y);
    }
    
    private void GoToSpot()
    {
        Rb.velocity = new Vector2(EnemyToSpot.normalized.x * PatrolSpeed, Rb.velocity.y);
    }

    #endregion

    #region Damage
    
    private void Die()
    {
        Rb.freezeRotation = false;
        Animator.Play("StubbyDestroy");
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
}
