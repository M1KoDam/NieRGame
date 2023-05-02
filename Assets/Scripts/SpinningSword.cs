using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningSword : MonoBehaviour
{
    public Player Player;
    private Rigidbody2D _rb;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _animator.Play("Spinning_Sword_anim");
        _rb.velocity = new Vector2(10, 0.25f);
        InvokeRepeating("DecreaseVelocity", 0, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(Player.transform.position, transform.position) < 1)
        {
            Destroy(gameObject);
        }
    }

    void DecreaseVelocity()
    {
        _rb.velocity = new Vector2(_rb.velocity.x - 1f, 0.25f);
    }
}
