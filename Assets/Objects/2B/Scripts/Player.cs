using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected GameLES gameLes;

    protected Rigidbody2D Rb;
    protected PlayerState State;
    
    public virtual void Start()
    {
        State = PlayerState.Active;
        Rb = GetComponent<Rigidbody2D>();
    }

    public void SetHealth(float inputHealth)
    {
        maxHealth = inputHealth;
        health = maxHealth;
    }
    
    protected bool Dead()
    {
        if (State is PlayerState.Dead or PlayerState.UnActive)
            return true;

        if (health > 0) return false;
        Die();
        return true;
    }
    
    private void Die()
    {
        State = PlayerState.Dead;
        Invoke(nameof(Respawn), 3);
    }

    private void Respawn()
    {
        gameLes.Respawn();
    }
    
    public void UnActivePlayer()
    {
        State = PlayerState.UnActive;
    }
    
    public void ActivePlayer()
    {
        State = PlayerState.Active;
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
