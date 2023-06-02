using UnityEngine;

public class AttackManager
{
    private readonly Enemy _enemy;
    private AttackType _currentAttack;
    private float _timer;

    public AttackManager(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void ExecuteAttack(AttackType attackType, out bool finished)
    {
        _currentAttack = attackType;
        attackType.Execute(out var hasFinished);
        finished = hasFinished;

        if (finished)
        {
            _currentAttack.Reset();
            _currentAttack = null;
        }
    }

    public void ExecuteRandomAttack(out bool finished)
    {
        if (_currentAttack is null)
        {
            var random = new System.Random();
            var randomNumber = random.Next(0, _enemy.attackTypes.Length);
            ExecuteAttack(_enemy.attackTypes[randomNumber], out var hasFinished);
            finished = hasFinished;
        }
        else
        {
            ExecuteAttack(_currentAttack, out var hasFinished);
            finished = hasFinished;
        }
    }
}