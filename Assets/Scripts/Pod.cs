using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Pod : MonoBehaviour
{
    public Player player;

    // public Bullet bullet;
    public float maxDistance = 0.1f;
    public float speed = 5f;
    public float brakeSpeed = 3f;

    public bool faceOrientationRight;

    [SerializeField] private Transform shootingPosition;

    private GameObject _pod;
    private Rigidbody2D _rb;

    private bool _canShoot = true;

    private Vector3 VectorToPlayer => TargetPosition - transform.position;

    private Vector3 TargetPosition => faceOrientationRight
        ? player.transform.position + new Vector3(2,3.5f, 0)
        : player.transform.position + new Vector3(-2, 3.5f, 0);

    private float DistanceToPlayer => VectorToPlayer.magnitude;

    void MoveToPlayer()
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
    }

    // Update is called once per frame
    void Update()
    {
        if (DistanceToPlayer > maxDistance)
            MoveToPlayer();
        else
            Brake();

        RestoreDirection();
        
        if (faceOrientationRight == player.faceOrientationRight)
        {
            transform.localScale *= new Vector2(-1, 1);
            faceOrientationRight = !player.faceOrientationRight;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canShoot)
            Shoot();
    }


    private void Shoot()
    {
        // var bul = Instantiate(bullet, shootingPosition.position, transform.rotation);
        // Destroy(bul, 5f);
    }
}