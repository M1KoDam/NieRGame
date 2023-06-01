using System;
using UnityEngine;

public class Goliath : Enemy
{
    protected override IState State
        => hp <= 0
            ? new DeadState()
            : new AttackState();

    protected override void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        if (State is DeadState)
            return;
    }

    protected void FixedUpdate()
    {
        State.Execute(this);
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
        throw new NotImplementedException();
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
}