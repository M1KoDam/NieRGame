using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Trigger : MonoBehaviour
{
    [SerializeField] private PlatformerLES platformerLES;
    [SerializeField] private int triggerSignal;
    
    private bool _isTriggered;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (!otherCollider.gameObject.CompareTag("Player")) return;
        
        if (!_isTriggered)
            platformerLES.GetTriggerSignal(triggerSignal);
        _isTriggered = true;
    }
}
