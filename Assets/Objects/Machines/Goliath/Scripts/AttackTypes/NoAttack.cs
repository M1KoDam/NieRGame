using UnityEngine;

public class NoAttack : AttackType
{
    private readonly Goliath _goliath;
    private int _attackStatus;
    private float _timer;

    public NoAttack(Goliath goliath)
    {
        _goliath = goliath;
    }
    
    public override void Execute(out bool finished)
    {
        finished = false;   
        
        if (_attackStatus == 0)
            Sleep(1f);
        if (_attackStatus == 1)
            finished = true;
    }

    public override void Reset()
    {
        _attackStatus = 0;
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