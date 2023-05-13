using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmallFlyerDestroying : MonoBehaviour
{
    public void Activate()
    {
        foreach(var childBody in GetComponentsInChildren<Rigidbody2D>())
        {
            Debug.Log(childBody);
            childBody.bodyType = RigidbodyType2D.Dynamic;
            childBody.mass = 5f;
        }
    }
}
