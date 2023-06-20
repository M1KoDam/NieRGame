using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SpinningSword : MonoBehaviour
{
    [SerializeField] private Android player;
    [SerializeField] private float time = 1.5f;
    [SerializeField] private LayerMask enemies;
    [SerializeField] private LayerMask enemyBullet;
    [SerializeField] private Vector2 attackRadius;
    [SerializeField] private int damage;
    private Vector3 DistanceToPlayer => player.transform.position - _rb.transform.position;

    private Rigidbody2D _rb;

    private List<Collider2D> _hitEnemies;
    private Vector2 TransformCoord => transform.position + new Vector3(0, 0.18f, 0);

    private float _startVelocityX;
    private bool _enemiesClear = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hitEnemies = new List<Collider2D>();
        Destroy();
    }
    
    void FixedUpdate()
    {
        if (_rb.velocity.x * _startVelocityX < 0 && !_enemiesClear)
        {
            _hitEnemies.Clear();
            _enemiesClear = true;
        }
        var _tempHitEnemies = Physics2D.OverlapBoxAll(TransformCoord, attackRadius, 0, enemies);
        foreach (var enemy in _tempHitEnemies)
        {
            if (_hitEnemies.Contains(enemy))
                continue;
            if (enemy.GetComponent<Enemy>() is null)
                enemy.GetComponentInParent<Enemy>().GetDamage(damage, transform);
            else
                enemy.GetComponent<Enemy>().GetDamage(damage, transform);
            _hitEnemies.Add(enemy);
        }
        
        var hitBullets = Physics2D.OverlapBoxAll(TransformCoord, attackRadius, 0, enemyBullet);
        foreach (var bullet in hitBullets)
        {
            bullet.GetComponent<EnemyBullet>().Destroy();
        }
        
        transform.rotation = new Quaternion(0, 0, 0, 0);
        if (Math.Abs(player.transform.position.x - transform.position.x) < 1f)
        {
            Destroy();
        }
        _rb.velocity += new Vector2(DistanceToPlayer.x / (time*10), 0.2f + DistanceToPlayer.y / (time*10));
    }

    public void Create()
    {
        gameObject.SetActive(true);
        transform.position = player.transform.position + new Vector3(3*(int)player.GetFaceOrientation(), 0,0 );
        _rb.velocity = new Vector2(20*(int)player.GetFaceOrientation(), 0.2f);
        _startVelocityX = _rb.velocity.x;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        _hitEnemies.Clear();
        _enemiesClear = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(TransformCoord, attackRadius);
    }
}
