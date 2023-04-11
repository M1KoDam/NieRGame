using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DefaultNamespace;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = System.Numerics.Vector3;

public class Pod : MonoBehaviour
{
    public Player player;
    public float maxDistance = 5;
    public float speed = 10f;
    private Camera camera;
    private Vector2 mouseVector;

    public Side faceOrientation = Side.Right;
    
    private Rigidbody2D _rb;

    //FixedUpdate
    void MoveToPlayer(bool isShooting)
    {
        float velocityY = 0;
        float velocityX = 0;
        var heading = transform.position - player.transform.position;
        var distance = heading.magnitude;
        if (Math.Abs(transform.position.y - player.transform.position.y - 10) > 0.1f)
        {
            velocityY = -((heading.y-3)/distance) * speed;
        }
        if (distance > maxDistance)
        {
            velocityX = -(heading/distance).x * speed;
            if (player.faceOrientation != faceOrientation && isShooting == false)
            {
                if (faceOrientation is Side.Left)
                    transform.localScale = new Vector2(1, 1);
                else
                    transform.localScale = new Vector2(-1, 1);
                faceOrientation = faceOrientation == Side.Right ? Side.Left : Side.Right;
            }
        }

        _rb.velocity = new Vector2(velocityX, velocityY + 0.2f);
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
            if (faceOrientation is Side.Left) transform.localScale = new Vector2(-1, 1);
            RestoreDirection();
        }
        MoveToPlayer(isShooting);
        Debug.Log(faceOrientation);
    }

    void LookAtMouse()
    {
        var angle = GetAngle();
        Vector2 localScale = Vector2.one;
        if (angle > 90 || angle < -90)
            localScale.y = -1f;
        else
            localScale.y = 1f;
        
        transform.localScale = localScale;
        _rb.MoveRotation(angle);
        if (angle is >= 0 and <= 90 or > -90 and < 0)
            faceOrientation = Side.Right;
        if (angle is > 90 and <= 180 or >= -180 and <= -90)
            faceOrientation = Side.Left;
        Debug.Log(angle);
    }

    private float GetAngle()
    {
        mouseVector = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector2 rightVector = new Vector2(1, 0);
        var scalarComposition = rightVector.x * mouseVector.x + rightVector.y * mouseVector.y;
        var modulesComposition = rightVector.magnitude * mouseVector.magnitude;
        var division = scalarComposition / modulesComposition;
        var angle = Mathf.Acos(division) * Mathf.Rad2Deg * (int)GetSide();
        return angle;
    }

    Side GetSide()
    {
        Side side = Side.Right;
        if (mouseVector.y <= Vector2.right.y)
            side = Side.Left;
        return side;
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
