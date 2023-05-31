using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float maxHealth = 100;

    protected Rigidbody2D Rb;
    protected PlayerState State;
    
    public virtual void Start()
    {
        State = PlayerState.Default;
        Rb = GetComponent<Rigidbody2D>();
    }

    public virtual void GetDamage(int inputDamage, Transform attackVector)
    {
        health -= inputDamage;
    }

    public float GetHealth()
    {
        return Math.Max(0, Math.Min(health / maxHealth, 1));
    }
}
