using System;
using UnityEngine;

public class Goliath : Enemy
{
    [NonSerialized] public float ArmRotationSpeed;
    [NonSerialized] public float HandRotationSpeed;
    
    [SerializeField] private float speed;
    [SerializeField] private Transform[] platformLevels;
    [SerializeField] private Transform armRight;
    [SerializeField] private GameObject handRight;
    [SerializeField] private Transform elbowRight;
    [SerializeField] private GameObject fistRight;

    private AttackManager _attackManager;
    private int _currentPlatformLevel;

    private Vector3? _targetPosition;
    private int targetPosition { set => _targetPosition = platformLevels[value].position; }

    private float _currentArmAngle;
    private float _currentHandAngle;

    public float targetArmAngle { get; set; }
    public float targetHandAngle { get; set; }

    public override AttackType[] attackTypes { get; set; }
    private Vector3? vectorToTarget => (_targetPosition - transform.position)?.normalized;
    private bool handOnTarget => Math.Abs(_currentHandAngle - targetHandAngle) <= HandRotationSpeed;
    private bool armOnTarget => Math.Abs(_currentArmAngle - targetArmAngle) <= ArmRotationSpeed;
    public bool bothOnTarget => handOnTarget && armOnTarget;

    protected override IState State
        => hp <= 0
            ? new DeadState()
            : new AttackState();

    protected override void Start()
    {
        targetPosition = 2;
        attackTypes = new AttackType[] { new GoliathSawAttack(this) };
        _attackManager = new AttackManager(this);
        base.Start();
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
    
    public override void Chase() { throw new NotImplementedException(); }
    public override void Patrol() { throw new NotImplementedException(); }
    public override void GoToScene() { throw new NotImplementedException(); }
    public override void DoIdle() { throw new NotImplementedException(); }
    public override void Die() { throw new NotImplementedException(); }
}