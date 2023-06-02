using UnityEngine;

public class GoliathShootingAttack : AttackType
{
    private readonly Goliath _goliath;
    private int _attackStatus;
    private float _fireTimer;
    private bool _canShoot;
    private float _timer;

    public GoliathShootingAttack(Goliath goliath)
    {
        _goliath = goliath;
    }

    public override void Execute(out bool finished)
    {
        finished = false;

        if (_attackStatus == 0)
            HandleShooting();

        if (_attackStatus == 0)
            Sleep(10.5f * _goliath.fireDelay);
        if (_attackStatus == 1)
            finished = true;
    }

    public override void Reset()
    {
        _attackStatus = 0;
    }

    private void HandleShooting()
    {
        if (_canShoot)
            Shoot();
        
        HandleFireRate();
    }
    
    private void HandleFireRate()
    {
        if (_fireTimer < 3 * _goliath.fireDelay)
        {
            _fireTimer += Time.fixedDeltaTime;
        }
        else
        {
            _canShoot = true;
            _fireTimer = 0;
        }
    }

    private void Shoot()
    {
        var bulletPrefab = _goliath.starPrefab;
        var bul = Object.Instantiate(bulletPrefab, _goliath.bulletPosition.position, _goliath.head.transform.rotation);
        foreach (var rb in bul.GetComponentsInChildren<Rigidbody2D>())
            rb.velocity = -bul.transform.right * 8;
        _goliath.sounds.AllSounds["EnemyShot"].PlaySound();
        Object.Destroy(bul.gameObject, 5f);
        
        _canShoot = false;
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