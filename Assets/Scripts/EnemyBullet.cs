using System;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private Collider2D _collider2D;
    private string _currentAnimation;
    
    public int bulletSpeed;
    [SerializeField] private int damage;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        
        _rb.useAutoMass = true;
    }

    private void Update()
    {
        if (_currentAnimation is "EnemyBulletExploding" && AnimCompleted())
        {
            Destroy(gameObject);
        }
    }

    public void Destroy()
    {
        _rb.velocity = Vector2.zero;
        _collider2D.enabled = false;
        _animator.Play("EnemyBulletExploding");
        _currentAnimation = "EnemyBulletExploding";
    }
    
    private bool AnimCompleted()
    {
        return Math.Abs(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1) < 0.1f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _currentAnimation is not "EnemyBulletExploding")
            collision.gameObject.GetComponent<Player>().GetDamage(damage, transform);
        Destroy();
    }
}
