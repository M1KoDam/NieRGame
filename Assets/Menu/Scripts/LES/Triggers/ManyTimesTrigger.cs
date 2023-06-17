using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ManyTimesTrigger : MonoBehaviour
{
    [SerializeField] protected PlatformerLES platformerLES;
    [SerializeField] protected int triggerSignal;
    [SerializeField] protected bool blockPlayer;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (!otherCollider.gameObject.CompareTag("Player")) return; 
        platformerLES.GetTriggerSignal(triggerSignal, blockPlayer);
    }
}
