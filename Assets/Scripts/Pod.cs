using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
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
    public float maxSpeedDelta = 1f;
    private Camera camera;
    public float offset;
    private Vector2 mouseVector;

    public bool faceOrientationRight = true;
    
    private Rigidbody2D _rb;

    //FixedUpdate
    void MoveToPlayer()
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
            if (player.faceOrientationRight != faceOrientationRight)
            {
                transform.localScale *= new Vector2(-1, 1);
                faceOrientationRight = !faceOrientationRight;
            }
        }
        /**
        var velocityXSign = velocityX < 0 ? -1 : 1;
        var velocityYSign = velocityY < 0 ? -1 : 1;

        //if (distance <= maxDistance + 1  && distance >= maxDistance)
        //{
            //velocityX = 1f * velocityXSign;
            //velocityY = 1f * velocityYSign;
            //velocityX = _rb.velocity.x - velocityX > maxSpeedDelta ? maxSpeedDelta * velocityXSign : velocityX;
            //velocityY = _rb.velocity.y - velocityY > maxSpeedDelta ? maxSpeedDelta * velocityYSign : velocityY;
       // }
       */

        _rb.velocity = new Vector2(velocityX, velocityY + 0.2f);
        Debug.Log(_rb.velocity);
    }

    void RestoreDirection()
    {
        //transform.localScale = Vector2.one;
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
        if (Input.GetKey(KeyCode.LeftShift))
            LookAtMouse();
        else
            RestoreDirection();
        MoveToPlayer();
    }
    
    private enum Side
    {
        Left = -1,
        Right = 1
    }

    void LookAtMouse()
    {
        Debug.Log(Input.mousePosition);
        mouseVector = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector2 rightVector = new Vector2(1, 0);
        var scalarComposition = rightVector.x * mouseVector.x + rightVector.y * mouseVector.y;
        var modulesComposition = rightVector.magnitude * mouseVector.magnitude;
        var division = scalarComposition / modulesComposition;
        var angle = Mathf.Acos(division) * Mathf.Rad2Deg * (int)GetSide();
        Vector2 localScale = Vector2.one;
        if (angle > 90 || angle < -90)
            localScale.y = -1f;
        else
            localScale.y = 1f;
        transform.localScale = localScale;
        _rb.MoveRotation(angle);
        //transform.rotation = quaternion.Euler(0, 0, angle);
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
            
            //Vector2 mouseVector = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            //Vector2 currentVector = (Vector2)transform.position - Vector2.left;
            Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + 100, transform.position.y));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, camera.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    
}