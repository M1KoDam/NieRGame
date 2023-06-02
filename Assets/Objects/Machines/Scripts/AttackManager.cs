using UnityEngine;
using Random = UnityEngine.Random;

public class AttackManager
{
    private readonly Enemy _enemy;
    private AttackType _currentAttack;
    private bool _attackProcessing;
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
        ExecuteAttack(_currentAttack ?? _enemy.attackTypes[Random.Range(0, _enemy.attackTypes.Length - 1)],
            out var hasFinished);

        _attackProcessing = !hasFinished;
        finished = hasFinished;
    }
}