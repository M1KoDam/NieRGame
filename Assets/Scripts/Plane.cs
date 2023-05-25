using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    [SerializeField] private Transform[] bulletPositions;
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField, Range(0, 20)] private float maxSway;
    [SerializeField, Range(0, 0.033f)] private float rotationSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private ViewType view;

    private float MaxSway => maxSway * Mathf.Deg2Rad;

    private static Vector2 MovementDelta => new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    private double FireDelay => 1 / fireRate;

    private Rigidbody2D _rb;
    private bool _canShoot;
    private float _fireTimer;
    private float _rotationTimer;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        HandleSways();
    }

    private void HandleSways()
    {
        var rot = transform.rotation;
        
        if (view == ViewType.Top)
        {
            if (MovementDelta.y > 0)
                rot.x = Mathf.Max(rot.x - rotationSpeed, -MaxSway);
            else if (MovementDelta.y < 0)
                rot.x = Mathf.Min(rot.x + rotationSpeed, MaxSway);
            else
                rot.x = 0;
        }
        else
        {
            if (MovementDelta.y > 0)
                rot.z = Mathf.Max(rot.x + rotationSpeed, MaxSway);
            else if (MovementDelta.y < 0)
                rot.z = Mathf.Min(rot.x - rotationSpeed, -MaxSway);
            else
                rot.z = 0;
        }

        transform.rotation = rot;
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _canShoot)
            Shoot();
    }

    private void Shoot()
    {
        foreach (var pos in bulletPositions)
        {
            var bul = Instantiate(bullet, pos.position, transform.rotation);
            bul.GetComponent<Rigidbody2D>().velocity = transform.right * bul.bulletSpeed;
            Destroy(bul.gameObject, 5f);
        }

        _canShoot = false;
    }
    
    public void GetDamage(int inputDamage, Transform attackVector)
    {
        health -= inputDamage;
    }
}