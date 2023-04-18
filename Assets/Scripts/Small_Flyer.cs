using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Flyer : MonoBehaviour
{
    private Rigidbody2D _rb;

    void Fly()
    {
        _rb.velocity = new Vector2(0, 0.2f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Fly();
    }
}
