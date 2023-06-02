using UnityEngine;

public class GoliathSawAttack : AttackType
{
    private readonly Goliath _goliath;
    private int _attackStatus;
    private float _timer;

    public GoliathSawAttack(Goliath goliath)
    {
        _goliath = goliath;
    }
    
    public override void Execute(out bool finished)
    {
        finished = false;
        
        if (_attackStatus == 0)
            Swing();
        if (_attackStatus == 1)
            Sleep(0.5f);
        if (_attackStatus == 2)
            Kick();
        if (_attackStatus == 3)
            Sleep(0.5f);
        if (_attackStatus == 4)
            Idle();
        if (_attackStatus == 5)
            finished = true;
    }

    public override void Reset()
    {
        _attackStatus = 0;
    }

    private void Swing()
    {
        _goliath.ArmRotationSpeed = 1;
        _goliath.HandRotationSpeed = 1;
        
        _goliath.targetArmAngle = GoliathStatics.SwingArmAngle;
        _goliath.targetHandAngle = GoliathStatics.SwingHandAngle;

        if (_goliath.bothOnTarget)
            _attackStatus++;
    }
    
    private void Kick()
    {
        _goliath.ArmRotationSpeed = 3;
        _goliath.HandRotationSpeed = 3;

        _goliath.targetArmAngle = GoliathStatics.KickArmAngle;
        _goliath.targetHandAngle = GoliathStatics.KickHandAngle;
        
        if (_goliath.bothOnTarget)
            _attackStatus++;
    }
    
    private void Idle()
    {
        _goliath.HandRotationSpeed = 0.25f;
        _goliath.ArmRotationSpeed = 0.25f;
        
        _goliath.targetArmAngle = GoliathStatics.IdleArmAngle;
        _goliath.targetHandAngle = GoliathStatics.IdleHandAngle;
        
        if (_goliath.bothOnTarget)
            _attackStatus++;
    }

    private void Sleep(float seconds)
    {
        _timer += Time.fixedDeltaTime;

        if (_timer >= seconds)
        {
            _timer = 0;
            _attackStatus++;
        }
    }
}