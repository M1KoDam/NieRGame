using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private Player Player;

    private void OnTriggerEnter2D(Collider2D otherCollider2D)
    {
        if (otherCollider2D.CompareTag("Player")) Player.SetHealth(-100);
    }
}
