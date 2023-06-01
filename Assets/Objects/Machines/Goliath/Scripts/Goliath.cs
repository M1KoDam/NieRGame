using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Goliath : Enemy
{
    [SerializeField] private float speed;
    [SerializeField] private Transform[] platformLevels;
    [SerializeField] private Transform armRight;
    [SerializeField] private GameObject handRight;
    [SerializeField] private Transform elbowRight;
    [SerializeField] private GameObject fistRight;
    [SerializeField] private float armRotationSpeed;
    [SerializeField] private float handRotationSpeed;
    private int _currentPlatformLevel;
    private Vector3? target;

    private const float IdleArmAngle = 0;
    private const float IdleHandAngle = 0;

    private const float SwingArmAngle = -150;
    private const float SwingHandAngle = -37;

    private const float KickArmAngle = -30;
    private const float KickHandAngle = 10;

    private const int AttackEndStatusCode = 228;

    private float currentArmAngle;
    private float currentHandAngle;

    private int attackStatus = AttackEndStatusCode;
    private int currentAttackType;

    private float timer;

    private float targetArmAngle { get; set; }
    private float targetHandAngle { get; set; }

    private Vector3? vectorToTarget => (target - transform.position)?.normalized;
    private bool HandOnTarget => Math.Abs(currentHandAngle - targetHandAngle) <= handRotationSpeed;
    private bool ArmOnTarget => Math.Abs(currentArmAngle - targetArmAngle) <= armRotationSpeed;
    private bool BothOnTarget => HandOnTarget && ArmOnTarget;

    protected override IState State
        => hp <= 0
            ? new DeadState()
            : new AttackState();

    protected override void Start()
    {
        target = platformLevels[2].position;
    }

    protected void Update()
    {
        if (State is DeadState)
            return;
    }

    private void HandleHandMovement()
    {
        if (!ArmOnTarget)
        {
            var currentArmSpeed = currentArmAngle > targetArmAngle ? -armRotationSpeed : armRotationSpeed;
            handRight.transform.RotateAround(armRight.position, Vector3.forward, currentArmSpeed);
            currentArmAngle += currentArmSpeed;
        }

        if (!HandOnTarget)
        {
            var currentHandSpeed = currentHandAngle > targetHandAngle ? -handRotationSpeed : handRotationSpeed;
            fistRight.transform.RotateAround(elbowRight.position, Vector3.forward, currentHandSpeed);
            currentHandAngle += currentHandSpeed;
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

        // Debug.Log($"HAND: current:{currentHandAngle}, target:{targetHandAngle}");
        // Debug.Log($"ARM: current:{currentArmAngle}, target:{targetArmAngle}");
    }

    public override void Patrol()
    {
        throw new NotImplementedException();
    }

    public override void Chase()
    {
        throw new NotImplementedException();
    }

    public override void Attack()
    {
        if (attackStatus == AttackEndStatusCode)
        {
            currentAttackType = Random.Range(1, 2);
            attackStatus = 0;
        }
        
        if (currentAttackType == 1)
            AttackType1();
        
        if (currentAttackType == 2)
            AttackType2();
    }

    private void AttackType2()
    {
        attackStatus = AttackEndStatusCode;
    }

    private void AttackType1()
    {
        if (attackStatus == 0)
            Swing();
        if (attackStatus == 1)
            Wait(2);
        if (attackStatus == 2)
            Kick();
        if (attackStatus == 3)
            Wait(2);
        if (attackStatus == 4)
            Idle();
        if (attackStatus == 5)
            attackStatus = AttackEndStatusCode;
    }

    private void Wait(int seconds)
    {
        timer += Time.fixedDeltaTime;

        if (timer >= seconds)
        {
            timer = 0;
            attackStatus++;
        }
    }

    private void Swing()
    {
        targetArmAngle = SwingArmAngle;
        targetHandAngle = SwingHandAngle;

        if (BothOnTarget)
            attackStatus++;
    }

    private void Kick()
    {
        targetArmAngle = KickArmAngle;
        targetHandAngle = KickHandAngle;
        
        if (BothOnTarget)
            attackStatus++;
    }

    private void Idle()
    {
        targetArmAngle = IdleArmAngle;
        targetHandAngle = IdleHandAngle;
        
        if (BothOnTarget)
            attackStatus++;
    }

    public override void GoToScene()
    {
        throw new NotImplementedException();
    }

    public override void DoIdle()
    {
        throw new NotImplementedException();
    }

    public override void Die()
    {
        throw new NotImplementedException();
    }

    public void MoveHead(int transformLevel)
    {
        target = platformLevels[transformLevel].position;
    }
}