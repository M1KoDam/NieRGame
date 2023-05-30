using System;
using System.Collections;
using UnityEngine;

public class RailwayCarriage : MonoBehaviour
{
    [SerializeField] private Player player;
    private Collider2D _collider;
    private Collider2D _playerCollider;
    private bool _ignorePlayer;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _playerCollider = player.GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        Physics2D.IgnoreCollision(_collider, _playerCollider, _ignorePlayer);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetAxis("Vertical") < 0 && collision.gameObject.CompareTag("Player") && !_ignorePlayer)
        {
            _ignorePlayer = true;
            Invoke(nameof(RestoreCollision), 0.5f);
        }
    }

    private void RestoreCollision()
    {
        _ignorePlayer = false;
    }
}