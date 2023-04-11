using System;
using UnityEngine;

public class Pod : MonoBehaviour
{
    public Player player;
    private Camera camera;
    private Vector2 mouseVector;
    public float maxDistance = 0.1f;
    public float speed = 5;
    public float brakeSpeed = 3f;

    public Side faceOrientation = Side.Right;

    // public Side faceOrientation => _rb.velocity.x > 0 ? Side.Right : Side.Left;
    private Vector3 VectorToPlayer => TargetPosition - transform.position;

    private Vector3 TargetPosition => faceOrientation == Side.Right
        ? player.transform.position + new Vector3(2, 3.5f, 0)
        : player.transform.position + new Vector3(-2, 3.5f, 0);

    private float DistanceToPlayer => VectorToPlayer.magnitude;

    private Rigidbody2D _rb;
    private Side _faceOrientation;

    //FixedUpdate
    void MoveToPlayer(bool isShooting)
    {
        _rb.velocity = VectorToPlayer.normalized * (speed * (float)Math.Log(DistanceToPlayer));
    }

    private void Brake()
    {
        _rb.velocity /= brakeSpeed;
    }

    void RestoreDirection()
    {
        _rb.MoveRotation(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var isShooting = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isShooting = true;
            LookAtMouse();
        }
        else
        {
            transform.localScale = faceOrientation == Side.Right
                ? new Vector2(1, 1)
                : new Vector2(-1, 1);
            RestoreDirection();
        }
        if (DistanceToPlayer > maxDistance)
            MoveToPlayer(isShooting);
        else
            Brake();
    }

    void LookAtMouse()
    {
        var angle = GetAngle();
        var localScale = Vector2.one;
        if (angle > 90 || angle < - 90)
            localScale.y = -1f;
        else
            localScale.y = 1f;

        transform.localScale = localScale;
        _rb.MoveRotation(angle);
        if ((0 <= angle && angle <= 90) || (-90 < angle && angle < 0))
            faceOrientation = Side.Right;
        if ((90 < angle && angle <= 180) || (-180 <= angle && angle <= -90))
            faceOrientation = Side.Left;
        
        Debug.Log(angle);
    }

    private float GetAngle()
    {
        mouseVector = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        var rightVector = Vector2.right;
        var scalarComposition = mouseVector.x + rightVector.y * mouseVector.y;
        var modulesComposition = rightVector.magnitude * mouseVector.magnitude;
        var division = scalarComposition / modulesComposition;
        var angle = Mathf.Acos(division) * Mathf.Rad2Deg * (mouseVector.y <= 1 ? -1 : 1);
        return angle;
    }
    

    private void OnDrawGizmos()
    {
        if (transform is not null)
        {
            Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + 100, transform.position.y));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, camera.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}