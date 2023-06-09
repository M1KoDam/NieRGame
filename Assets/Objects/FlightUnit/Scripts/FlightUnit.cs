using UnityEngine;
using UnityEngine.SceneManagement;

public class FlightUnit : Player
{
    [Header("Main Settings")]
    [SerializeField] private float speed = 0.5f;
    [SerializeField, Range(0, 20)] private float maxSway = 5;
    [SerializeField, Range(0, 10)] private float rotationSpeed = 2.5f;
    [SerializeField] private float fireRate = 5;
    [SerializeField] private ViewType view;

    [Header("Objects")]
    [SerializeField] private PlayerBullet bullet;
    [SerializeField] private Transform[] gunPositions;
    [SerializeField] private GameObject[] engines;
    
    private bool _canShoot;
    private bool _swayDown;
    private float _fireTimer;
    private float _rotationTimer;
    private int _swayCount;

    private float MaxSway => maxSway * Mathf.Deg2Rad;
    private static Vector2 MovementDelta => new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    private Vector2? _target;
    private Vector2? CustomMovementDelta => _target is null ? null : _target - transform.position;
    private double FireDelay => 1 / fireRate;
    private float RotationSpeed => rotationSpeed * Mathf.Deg2Rad;
    public Sounds sounds;

    private void FixedUpdate()
    {
        ManageEngines();
        if (State is PlayerState.UnActive or PlayerState.Dead)
            return;
        if (Dead()) return;
        HandleMovement();
        HandleShooting();
        HandleFireRate();
    }

    private bool HandleDeath()
    {
        if (health <= 0 && State is not PlayerState.Dead)
        {
            State = PlayerState.Dead;
            Invoke(nameof(Respawn), 5f);
            return true;
        }

        return false;
    }

    private void ManageEngines()
    {
        var enginesStatus = !(Input.GetKey(KeyCode.A));
        foreach (var engine in engines)
        {
            engine.SetActive(enginesStatus);
        }
    }

    private void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    public void MoveToThePoint(Vector2 point)
    {
        _target = point;
    }

    private void HandleMovement()
    {
        if (CustomMovementDelta is not null && CustomMovementDelta.Value.magnitude < 0.5f)
            _target = null;

        var movement = CustomMovementDelta ?? MovementDelta;

        Rb.MovePosition(Rb.position + movement.normalized * speed);
        HandleAngleSways();
        if (movement.magnitude < 0.01f)
            HandleMovementSways();
    }

    private void HandleAngleSways()
    {
        var rot = transform.rotation;
        var movement = CustomMovementDelta ?? MovementDelta;

        if (view == ViewType.Top)
        {
            if (movement.y > 0)
                rot.x = Mathf.Max(rot.x - RotationSpeed, -MaxSway);
            else if (movement.y < 0)
                rot.x = Mathf.Min(rot.x + RotationSpeed, MaxSway);
            else
                rot.x = rot.x == 0
                    ? 0
                    : rot.x > 0
                        ? Mathf.Max(rot.x - RotationSpeed, 0)
                        : Mathf.Min(rot.x + RotationSpeed, 0);
        }
        else
        {
            if (movement.y > 0)
                rot.z = Mathf.Min(rot.z + RotationSpeed, MaxSway);
            else if (movement.y < 0)
                rot.z = Mathf.Max(rot.z - RotationSpeed, -MaxSway);
            else
                rot.z = rot.z == 0
                    ? 0
                    : rot.z > 0
                        ? Mathf.Max(rot.z - RotationSpeed, 0)
                        : Mathf.Min(rot.z + RotationSpeed, 0);
        }

        transform.rotation = rot;
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _canShoot)
        {
            sounds.AllSounds["FlightUnitShot"].PlaySound();
            Shoot();
        }
    }

    private void Shoot()
    {
        foreach (var pos in gunPositions)
        {
            var bul = Instantiate(bullet, pos.position, transform.rotation);
            bul.GetComponent<Rigidbody2D>().velocity = transform.right * bul.bulletSpeed;
            Destroy(bul.gameObject, 5f);
        }

        _canShoot = false;
    }

    private void HandleMovementSways()
    {
        _swayCount++;
        if (_swayCount > 60)
        {
            _swayCount = 0;
            _swayDown = !_swayDown;
        }

        if (_swayCount < 30)
            return;

        transform.position += _swayDown ? Vector3.down / 100f : Vector3.up / 100f;
    }
}