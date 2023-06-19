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
            Sleep(1.5f);
        if (_attackStatus == 1)
            SpawnEnemies();
        if (_attackStatus == 2)
            Sleep(10);
        if (_attackStatus == 3)
            finished = true;
    }

    private void SpawnEnemies()
    {
        var spawnpoint1Index = Random.Range(0, _goliath.spawnPoints.Length - 1);
        var spawnpoint2Index = (Random.Range(1, _goliath.spawnPoints.Length - 1) + spawnpoint1Index)
                               % _goliath.spawnPoints.Length;

        var spawnpoints = new[]
        {
            _goliath.spawnPoints[spawnpoint1Index],
            _goliath.spawnPoints[spawnpoint2Index]
        };
        
        foreach (var spawnpoint in spawnpoints)
        {
            var randomEnemyPrefab = _goliath.enemyPrefabs[Random.Range(0, _goliath.enemyPrefabs.Length)];
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