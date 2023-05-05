using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SpinningSword : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float time = 1.5f;
    [SerializeField] private LayerMask enemies;
    private Vector3 DistanceToPlayer => player.transform.position - _rb.transform.position;

    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy();
    }
    
    void FixedUpdate()
    {
        if (Math.Abs(player.transform.position.x - transform.position.x) < 1f)
        {
            Destroy();
        }
        _rb.velocity += new Vector2(DistanceToPlayer.x / (time*10), 0.2f + DistanceToPlayer.y / (time*10));
    }
    
    private void Damage()
    {
        var hitedEnemies = Physics2D.OverlapCapsuleAll(transform.position,
            new Vector2(3, 2), CapsuleDirection2D.Vertical, enemies);
        foreach (var enemy in hitedEnemies)
        {
            //Debug.Log(enemy);
        }
    }

    public void Create()
    {
        InvokeRepeating(nameof(Damage), 0, 0.25f);
        gameObject.SetActive(true);
        transform.position = player.transform.position + new Vector3(3*(int)player.faceOrientation, 0,0 );
        _rb.velocity = new Vector2(20*(int)player.faceOrientation, 0.2f);
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<SmallFlyer>().GetDamage(20);
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
            Debug.Log(true);
        }
    }
}
