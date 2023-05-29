using UnityEngine;

public class SpringyBullet : Bullet
{
    private void FixedUpdate()
    {
        // if (CurrentAnimation is "ПОМЕНЯЙ МЕНЯ" && AnimCompleted())
        // {
        // Destroy(gameObject);
        // }
    }

    private void Destroy()
    {
        Rb.velocity = Vector2.zero;
        Collider2D.enabled = false;
        // Animator.Play("ПОМЕНЯЙ МЕНЯ");
        // CurrentAnimation = "ПОМЕНЯЙ МЕНЯ";


        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && CurrentAnimation is not "ПОМЕНЯЙ МЕНЯ")
        {
            collision.gameObject.GetComponent<Player>().GetDamage(damage, transform);
            Destroy();
        }
    }
}