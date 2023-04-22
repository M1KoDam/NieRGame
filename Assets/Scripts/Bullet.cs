using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Camera _camera;
    public int bulletSpeed;
    
    private Vector2 BulletToMouse => (_camera.ScreenToWorldPoint(Input.mousePosition) - _rb.transform.position);
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        Accelerate();
    }
    
    private void Accelerate()
    {
        _rb.velocity = bulletSpeed * BulletToMouse.normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}