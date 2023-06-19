using System;
using System.Collections;
using UnityEngine;

public class Goliath : Enemy
{
    public float maxHp = 100;
    [SerializeField] public Sounds sounds;
    [SerializeField] public Transform[] explosionPoints;
    
    [SerializeField] public Enemy[] enemyPrefabs;
    [SerializeField] public Transform[] spawnPoints;

    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public GameObject starPrefab;

    [SerializeField] public SpringyBullet springyBulletPrefab;
    [SerializeField] public Transform bulletPosition;
    [SerializeField] public GoliathHead head;
    [SerializeField] public Saw saw;
    [SerializeField] public GoliathElbow elbow;
    [SerializeField] public int springyBulletRate;
    [SerializeField] public float fireRate;

    [NonSerialized] public float ArmRotationSpeed;
    [NonSerialized] public float HandRotationSpeed;

    [SerializeField] private float speed;
    [SerializeField] private Transform[] platformLevels;
    [SerializeField] private Transform armRight;
    [SerializeField] private GameObject handRight;
    [SerializeField] private Transform elbowRight;
    [SerializeField] private GameObject fistRight;
    
    [SerializeField] private PlatformerLES _platformerLes;

    private AttackManager _attackManager;
    private Vector3? _targetPosition;
    private float _currentArmAngle;
    private float _currentHandAngle;
    private int _currentPlatformLevel;
    private bool _isActive;

    public float targetArmAngle { get; set; }
    public float targetHandAngle { get; set; }
    public override AttackType[] attackTypes { get; set; }
    private Vector3? vectorToTarget => (_targetPosition - transform.position)?.normalized;
    private bool handOnTarget => Math.Abs(_currentHandAngle - targetHandAngle) <= HandRotationSpeed;
    private bool armOnTarget => Math.Abs(_currentArmAngle - targetArmAngle) <= ArmRotationSpeed;
    public bool bothOnTarget => handOnTarget && armOnTarget;

    public float fireDelay => 1 / fireRate;

    protected override IState State
        => hp <= 0
            ? new DeadState()
            : _isActive
                ? new AttackState()
                : new IdleState();

    private int targetPosition
    {
        set => _targetPosition = platformLevels[value].position;
    }

    public void Activate()
    {
        _isActive = true;
    }

    protected override void Start()
    {
        attackTypes = new AttackType[]
        {
            new GoliathSawAttack(this),
            new GoliathShootingRotatingAttack(this),
            new GoliathShootingAttack(this),
            new NoAttack(this),
            new GoliathSpawnEnemiesAttack(this)
        };
        
        targetPosition = 2;
        _attackManager = new AttackManager(this);
        base.Start();
        Collider = head.GetComponent<Collider2D>();
        Activate();
        sounds.AllSounds["Saw"].PlaySoundLoop();
    }

    private void HandleHandMovement()
    {
        if (!armOnTarget)
        {
            var currentArmSpeed = _currentArmAngle > targetArmAngle ? -ArmRotationSpeed : ArmRotationSpeed;
            handRight.transform.RotateAround(armRight.position, Vector3.forward, currentArmSpeed);
            _currentArmAngle += currentArmSpeed;
        }

        if (!handOnTarget)
        {
            var currentHandSpeed = _currentHandAngle > targetHandAngle ? -HandRotationSpeed : HandRotationSpeed;
            fistRight.transform.RotateAround(elbowRight.position, Vector3.forward, currentHandSpeed);
            _currentHandAngle += currentHandSpeed;
        }
    }

    private void HandleMovement()
    {
        if (vectorToTarget is null)
            return;

        var position = transform.position;
        if (Mathf.Abs(vectorToTarget.Value.y) > speed)
            position.y += speed * Mathf.Sign(vectorToTarget.Value.y);
        transform.position = position;
    }

    protected void FixedUpdate()
    {
        State.Execute(this);

        if (Mathf.Abs(head.transform.position.y - player.transform.position.y) > 6f)
            Chase();
        else if (Mathf.Abs(head.transform.position.y - player.transform.position.y) < 0.5f)
            _targetPosition = null;
        
        HandleMovement();
        HandleHandMovement();
    }

    public override void Attack()
    {
        _attackManager.ExecuteRandomAttack(out var finished);
    }

    public override void Chase()
    {
        _targetPosition = player.transform.position + GoliathStatics.HeadOffset;
    }

    public override void Patrol()
    {
        throw new NotImplementedException();
    }

    public override void GoToScene()
    {
        throw new NotImplementedException();
    }

    public override void DoIdle()
    {
    }

    public override void Die()
    {
        if (CurDestructionTime <= 0)
        {
            if (FaceOrientation is Side.Right)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }

            var tempPosition = transform.position;
            var tempRotation = transform.rotation;

            Destroy(gameObject);

            var smallFlyerDestroyingCopy = Instantiate(enemyDestroying, tempPosition, tempRotation);
            smallFlyerDestroyingCopy.Activate();
            foreach (var rb in smallFlyerDestroyingCopy.GetComponentsInChildren<Rigidbody2D>())
                rb.gravityScale = 0.75f;
            
            Destroy(smallFlyerDestroyingCopy.gameObject, 100f);

            foreach (var explosionPoint in explosionPoints)
            {
                var goliathExplosion = Instantiate(explosion, explosionPoint.position, tempRotation);
                goliathExplosion.force = 15f;
                goliathExplosion.explosionScale = 1.95f;
                goliathExplosion.Explode();
            }

            _platformerLes.GetTriggerSignal(-1, true);
        }
        else
            CurDestructionTime -= Time.deltaTime;
    }

    public override void GetDamage(int inputDamage, Transform attackVector)
    {
        if (inputDamage >= 20)
        {
            CanAttack = false;
            CancelInvoke(nameof(WaitForAttack));
            Invoke(nameof(WaitForAttack), 1);
        }
        
        hp -= inputDamage;
    }

    public float GetHealth()
    {
        return Math.Max(0, Math.Min(hp / maxHp, 1));
    }
}