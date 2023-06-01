using System;
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
        attackType.Execute(out var hasFinished);
        finished = hasFinished;
    }

    public void ExecuteRandomAttack(out bool finished)
    {
        ExecuteAttack(_currentAttack is not null && _attackProcessing
                ? _currentAttack
                : _enemy.attackTypes[Random.Range(0, _enemy.attackTypes.Length)],
            out var hasFinished);

        _attackProcessing = !hasFinished;
        finished = hasFinished;
    }

    public void Sleep(float seconds, out bool done)
    {
        done = false;
        _timer += Time.fixedDeltaTime;

        if (_timer >= seconds)
        {
            _timer = 0;
            done = true;
        }
    }
}