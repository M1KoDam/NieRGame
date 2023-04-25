using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    public int bulletSpeed;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rb.useAutoMass = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}