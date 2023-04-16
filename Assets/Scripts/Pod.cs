using Unity.VisualScripting;
using UnityEngine;


public class Pod : MonoBehaviour
{
    public Player player;
    public Bullet bullet;

    private Rigidbody2D _rb;
    private Camera _camera;

    private static readonly Vector3 RightPosition = new(2, 3.5f, 0);
    private static readonly Vector3 LeftPosition = new(-2, 3.5f, 0);
    private static readonly Vector3 ShootingPosition = new(0, 3.5f, 0);

    private Vector3 BulletPosition => FaceOrientation == Side.Right
        ? transform.position + RightShootingOffset
        : transform.position + LeftShootingOffset;

    private static readonly Vector3 RightShootingOffset = new(1, 0, 0);
    private static readonly Vector3 LeftShootingOffset = new(-1, 0, 0);

    private static readonly Vector3 RightLocalScale = new(1, 1);
    private static readonly Vector3 LeftLocalScale = new(-1, 1);

    private const float BrakingSpeed = 3;
    private const float Speed = 5;
    private const float MaxDistance = 0.1f;

    private Vector3 _velocity;
    private float _angle;
    private bool _isScoping;

    private Side FaceOrientation
        => _isScoping
            ? -90 <= _angle && _angle <= 90
                ? Side.Right
                : Side.Left
            : _velocity.x > 0
                ? Side.Right
                : Side.Left;

    private Vector3 PodToPlayer => TargetPosition - _rb.transform.position;
    private float DistanceToPlayer => PodToPlayer.magnitude;
    private Vector2 PodToMouse => (_camera.ScreenToWorldPoint(Input.mousePosition) - _rb.transform.position);

    private Vector3 TargetPosition => _isScoping
        ? player.transform.position + ShootingPosition
        : FaceOrientation == Side.Right
            ? player.transform.position + RightPosition
            : player.transform.position + LeftPosition;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isScoping = true;
            LookAtMouse();
            Shoot();
        }
        else
        {
            _isScoping = false;
            LookUpwards();
        }

        if (DistanceToPlayer > MaxDistance)
            MoveToPlayer();
        else
            Brake();
    }

    private void Shoot()
    {
        var bul = Instantiate(bullet, BulletPosition, transform.rotation);
        Destroy(bul, 5f);
    }

    private void Update()
    {
        transform.localScale = FaceOrientation == Side.Right
            ? RightLocalScale
            : LeftLocalScale;

        _rb.MoveRotation(_angle);
        _rb.velocity = _velocity;
    }

    private void LookAtMouse()
    {
        var angle = -Vector2.SignedAngle(PodToMouse, Vector2.right);
        if (-90 <= angle && angle <= 90)
            _angle = angle;
        else if (angle > 90)
            _angle = angle + 180;
        else if (angle < -90)
            _angle = angle - 180;
    }

    private void LookUpwards()
    {
        _angle = 0;
    }

    private void MoveToPlayer()
    {
        _velocity = PodToPlayer.normalized * (Speed * Mathf.Log(DistanceToPlayer));
    }

    private void Brake()
    {
        _velocity /= BrakingSpeed;
    }
}