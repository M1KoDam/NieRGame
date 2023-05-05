using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    public int bulletSpeed;
    [SerializeField] private int damage;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rb.useAutoMass = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            Physics.IgnoreCollision(collision.collider.GetComponent<Collider>(), GetComponent<Collider>());
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().GetDamage(damage);
        }
        Destroy(gameObject);
    }
}
