using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Pod : MonoBehaviour
{
    public Player player;
    public float maxDistance = 5;
    public float speed = 100f;

    public bool faceOrientationRight = true;
    
    private Rigidbody2D _rb;

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
    }

    // Update is called once per frame
    void Update()
    {
        RestoreDirection();
        MoveToPlayer();
    }
}