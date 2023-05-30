using UnityEngine;

public class EnemyBullet : Bullet
{
    private void FixedUpdate()
    {
        if (CurrentAnimation is "EnemyBulletExploding" && AnimCompleted())
        {
            Destroy(gameObject);
        }
    }

    public void Destroy()
    {
        Rb.velocity = Vector2.zero;
        Collider2D.enabled = false;
        Animator.Play("EnemyBulletExploding");
        CurrentAnimation = "EnemyBulletExploding";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && CurrentAnimation is not "EnemyBulletExploding")
            collision.gameObject.GetComponent<Player>().GetDamage(damage, transform);
        Destroy();
    }
}
