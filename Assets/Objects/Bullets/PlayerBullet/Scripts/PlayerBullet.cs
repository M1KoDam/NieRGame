using System;
using UnityEngine;

public class PlayerBullet : Bullet
{
    private void FixedUpdate()
    {
        if (CurrentAnimation is "BulletExploding")
        {
            if (AnimCompleted())
                Destroy(gameObject);
            if (AnimPlaying())
                Rb.velocity = Vector2.zero;
        }
    }

    private void Destroy()
    {
        Rb.velocity = Vector2.zero;
        Collider2D.enabled = false;
        Animator.Play("BulletExploding");
        CurrentAnimation = "BulletExploding";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && CurrentAnimation is not "BulletExploding")
            collision.gameObject.GetComponent<Enemy>().GetDamage(damage, transform);
        Destroy();
    }
}