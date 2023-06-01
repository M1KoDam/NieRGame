using UnityEngine;

public class SpringyBullet : Bullet
{
    private void FixedUpdate()
    {
        if (CurrentAnimation is "SpringyBulletExploding" && AnimCompleted())
        {
            Destroy(gameObject);
        }
    }

    private void Destroy()
    {
        Rb.velocity = Vector2.zero;
        Collider2D.enabled = false;
        Animator.Play("SpringyBulletExploding");
        CurrentAnimation = "SpringyBulletExploding";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && CurrentAnimation is not "SpringyBulletExploding")
            collision.gameObject.GetComponent<Player>().GetDamage(damage, transform);
        
        if (!(collision.gameObject.CompareTag("Platform") 
              || collision.gameObject.CompareTag("Wall") 
              || collision.gameObject.CompareTag("Stairs")))
            Destroy();
    }
}