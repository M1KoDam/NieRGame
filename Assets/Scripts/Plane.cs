using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    [SerializeField] private Transform bulletPosition;
    [SerializeField] private float speed;
    [SerializeField] private float fireRate;

    private static Vector2 MovementDelta => new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    private double FireDelay => 1 / fireRate;

    private Rigidbody2D _rb;
    private bool _canShoot;
    private float _fireTimer;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleShooting();
        HandleFireRate();
    }

    private void HandleFireRate()
    {
        if (_fireTimer < FireDelay)
        {
            _fireTimer += Time.fixedDeltaTime;
        }
        else
        {
            _canShoot = true;
            _fireTimer = 0;
        }
    }


    private void HandleMovement()
    {
        _rb.MovePosition(_rb.position + MovementDelta.normalized * speed);
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _canShoot)
            Shoot();
    }

    private void Shoot()
    {
        var bul = Instantiate(bullet, bulletPosition.position, transform.rotation);
        bul.GetComponent<Rigidbody2D>().velocity = transform.right * bul.bulletSpeed;
        Destroy(bul.gameObject, 5f);

        _canShoot = false;
    }
}