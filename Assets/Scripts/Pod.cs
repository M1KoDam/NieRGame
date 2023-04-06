using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Pod : MonoBehaviour
{
    public Player player;
    public float maxDistance = 5;
    public float speed = 10f;

    public bool faceOrientationRight = true;
    
    private Rigidbody2D _rb;

    void MoveToPlayer()
    {
        _rb.velocity = new Vector2(0, 0.2f);
        var heading = transform.position - player.transform.position;
        var distance = heading.magnitude;
        if (distance > maxDistance)
        {
            _rb.velocity = new Vector2(-(heading/distance).x * speed, _rb.velocity.y);
            if (player.faceOrientationRight != faceOrientationRight)
            {
                transform.localScale *= new Vector2(-1, 1);
                faceOrientationRight = !faceOrientationRight;
            }
        }
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