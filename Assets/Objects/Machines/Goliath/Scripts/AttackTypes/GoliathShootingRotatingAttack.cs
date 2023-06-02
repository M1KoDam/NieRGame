using UnityEngine;

public class GoliathShootingRotatingAttack : AttackType
{
    private readonly Goliath _goliath;
    private int _attackStatus;
    private float _fireTimer;
    private bool _canShoot;

    public GoliathShootingRotatingAttack(Goliath goliath)
    {
        _goliath = goliath;
    }

    public override void Execute(out bool finished)
    {
        finished = false;

        if (0 <= _attackStatus && _attackStatus <= 3)
            HandleShooting();
        
        if (_attackStatus == 0)
            Rotate(45);
        if (_attackStatus == 1)
            Rotate(-45);
        if (_attackStatus == 2)
            Rotate(45);
        if (_attackStatus == 3)
            Rotate(0);
        if (_attackStatus == 4)
            finished = true;
    }

    private void HandleShooting()
    {
        if (_canShoot)
            Shoot();
        
        HandleFireRate();
    }
    
    private void HandleFireRate()
    {
        if (_fireTimer < _goliath.fireDelay)
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
        var bulletPrefab = Random.Range(1, _goliath.springyBulletRate) == 1
            ? _goliath.springyBulletPrefab
            : _goliath.bulletPrefab;
        var bul = Object.Instantiate(bulletPrefab, _goliath.bulletPosition.position, _goliath.head.transform.rotation);
        bul.GetComponent<Rigidbody2D>().velocity = -bul.transform.right * bul.bulletSpeed;
        _goliath.sounds.AllSounds["EnemyShot"].PlaySound();
        Object.Destroy(bul.gameObject, 5f);
        
        _canShoot = false;
    }

    private void Rotate(float angle)
    {
        _goliath.head.targetRotation = angle;
        if (_goliath.head.rotationOnTarget)
            _attackStatus++;
    }
    
    public override void Reset()
    {
        _attackStatus = 0;
    }
}