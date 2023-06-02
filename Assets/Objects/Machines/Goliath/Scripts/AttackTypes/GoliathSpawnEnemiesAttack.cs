using UnityEngine;

public class GoliathSpawnEnemiesAttack : AttackType
{
    private readonly Goliath _goliath;
    private int _attackStatus;
    private float _timer;

    public GoliathSpawnEnemiesAttack(Goliath goliath)
    {
        _goliath = goliath;
    }

    public override void Execute(out bool finished)
    {
        finished = false;

        if (_attackStatus == 0)
            SpawnEnemies();
        if (_attackStatus == 1)
            Sleep(10);
        if (_attackStatus == 2)
            finished = true;
    }

    private void SpawnEnemies()
    {
        foreach (var spawnpoint in _goliath.spawnPoints)
        {
            var randomEnemyPrefab = _goliath.enemyPrefabs[Random.Range(0, _goliath.enemyPrefabs.Length-1)];
            Object.Instantiate(randomEnemyPrefab, spawnpoint.position, _goliath.transform.rotation);
        }

        _attackStatus++;
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