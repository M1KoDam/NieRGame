using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected Rigidbody2D Rb;
    protected Animator Animator;
    protected Collider2D Collider2D;
    protected string CurrentAnimation;

    public int bulletSpeed;
    [SerializeField] protected int damage;

    protected void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Collider2D = GetComponent<Collider2D>();
        
        Rb.useAutoMass = true;
    }

    protected bool AnimCompleted()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    protected bool AnimPlaying(float time = 1)
    {
        return Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < time;
    }

}