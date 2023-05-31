using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmallGunner : Enemy
{
    [Header("Gun Settings")]
    [SerializeField] private EnemyBullet bullet;
    [SerializeField] private Transform gun;

    [Header("Step Climb Settings")]
    [SerializeField] private GameObject stayRayUpper;
    [SerializeField] private GameObject stayRayLower;
    
    public Sounds sounds;
    
    private Vector2 BulletPosition => gun.transform.position;

    // Update is called once per frame
    private void Update()
    {
        if (State is DeadState || IsDamaged)
            return;
        
        StepClimb();

        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;
    }
    
    private void FixedUpdate()
    {
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
        ChangeAnimation("GunnerIdle");
        if (CanAttack)
        {
            CanAttack = false;
            Shoot();
            Invoke(nameof(WaitForAttack), attackRate);
        }
    }
    
    private void Shoot()
    {
        var bul = Instantiate(bullet, BulletPosition, transform.rotation);
        bul.GetComponent<Rigidbody2D>().velocity = EnemyToPlayer.normalized * bul.bulletSpeed;
        sounds.AllSounds["EnemyShot"].PlaySound();
        Destroy(bul.gameObject, 5f); ;
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
        ChangeAnimation(Math.Abs(Rb.velocity.x) > 0.1f ? "GunnerMovement" : "GunnerIdle");
        Rb.velocity = new Vector2(distance.normalized.x * speed, Rb.velocity.y);
    }

    public override void GoToScene()
    {
        throw new Exception("this type of smallFlyer don't support 'GoToScene' work mode");
    }

    #endregion

    #region GetDamage

    public override void Die()
    {
        Rb.freezeRotation = false;
        ChangeAnimation("GunnerDestroy");
        if (CurDestructionTime <= 0)
        {
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
