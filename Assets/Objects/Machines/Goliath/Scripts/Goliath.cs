using System;
using System.Collections;
using UnityEngine;

public class Goliath : Enemy
{
    [SerializeField] public Transform[] explosionPoints;
    
    [SerializeField] public Enemy[] enemyPrefabs;
    [SerializeField] public Transform[] spawnPoints;

    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public GameObject starPrefab;

    [SerializeField] public SpringyBullet springyBulletPrefab;
    [SerializeField] public Transform bulletPosition;
    [SerializeField] public GoliathHead head;
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
    }

    protected void Update()
    {
        if (State is DeadState)
            return;
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
        position.y += vectorToTarget.Value.y * speed;
        transform.position = position;
    }

    protected void FixedUpdate()
    {
        State.Execute(this);

        HandleMovement();
        HandleHandMovement();
    }

    public override void Attack()
    {
        _attackManager.ExecuteRandomAttack(out var finished);
    }

    public override void Chase()
    {
        throw new NotImplementedException();
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
            var tempLocalScale = transform.localScale;

            Destroy(gameObject);

            var smallFlyerDestroyingCopy = Instantiate(enemyDestroying, tempPosition, tempRotation);
            smallFlyerDestroyingCopy.transform.localScale = tempLocalScale;
            smallFlyerDestroyingCopy.Activate();
            foreach (var rb in smallFlyerDestroyingCopy.GetComponentsInChildren<Rigidbody2D>())
                rb.gravityScale = 0.25f;
            Destroy(smallFlyerDestroyingCopy.gameObject, 10f);

            foreach (var explosionPoint in explosionPoints)
                StartCoroutine(CreateExplosion(explosionPoint.position, tempRotation));
        }
        else
            CurDestructionTime -= Time.deltaTime;
    }

    private IEnumerator CreateExplosion(Vector3 position, Quaternion rotation)
    {
        var smallFlyerExplosion = Instantiate(explosion, position, rotation);
        smallFlyerExplosion.force = 100000;
        smallFlyerExplosion.explosionScale = 1.85f;
        smallFlyerExplosion.Explode();
        yield return new WaitForSeconds(0.25f);
    }
}