using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionScale;
    public float radius;
    public float force;

    private Animator _animator;
    private bool _exploding;
    
    private void Update()
    {
        if (_exploding && AnimCompleted())
            Destroy(gameObject);
    }

    public void Explode()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Explosion");
        _exploding = true;
        transform.localScale = new Vector2(explosionScale, explosionScale);
        var overlappedColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var overlappedCollider in overlappedColliders)
        {
            var attachedRigidbody = overlappedCollider.attachedRigidbody;
            if (attachedRigidbody)
            {
                attachedRigidbody.AddExplosionForce(force, transform.position, radius);
            }
        }
    }
    
    private bool AnimCompleted()
    {
        return Math.Abs(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1) < 0.01f;
    }
}
