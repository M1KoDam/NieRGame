using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Background : MonoBehaviour
{
    public float speed = 1;
    private Transform _backTransform;
    private float _backSize;
    private float _backPos;
    void Start()
    {
        _backTransform = GetComponent<Transform>();
        _backSize = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        _backPos += speed * Time.deltaTime;
        _backPos = Mathf.Repeat(_backPos, _backSize);
        _backTransform.position = new Vector3(_backPos, 0, 0);
    }
}
