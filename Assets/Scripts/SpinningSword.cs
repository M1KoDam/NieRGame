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
        transform.rotation = new Quaternion(0, 0, 0, 0);
        if (Math.Abs(player.transform.position.x - transform.position.x) < 1f)
        {
            Destroy();
        }
        _rb.velocity += new Vector2(DistanceToPlayer.x / (time*10), 0.2f + DistanceToPlayer.y / (time*10));
    }

    public void Create()
    {
        gameObject.SetActive(true);
        transform.position = player.transform.position + new Vector3(3*(int)player.GetFaceOrientation(), 0,0 );
        _rb.velocity = new Vector2(20*(int)player.GetFaceOrientation(), 0.2f);
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
        }
    }
}
