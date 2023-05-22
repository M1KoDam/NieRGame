using System;
using UnityEngine;

public class Bullet : MonoBehaviour
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
    
    private void FixedUpdate()
    {
        if (_currentAnimation is "BulletExploding")
        {
            if (AnimCompleted())
                Destroy(gameObject);
            if (AnimPlaying())
                _rb.velocity = Vector2.zero;
        }
    }

    private void Destroy()
    {
        _rb.velocity = Vector2.zero;
        _collider2D.enabled = false;
        _animator.Play("BulletExploding");
        _currentAnimation = "BulletExploding";
    }
    
    private bool AnimCompleted()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }
    
    private bool AnimPlaying(float time = 1)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && _currentAnimation is not "BulletExploding")
            collision.gameObject.GetComponent<Enemy>().GetDamage(damage);
        Destroy();
    }
}