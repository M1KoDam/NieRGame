using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TwoSidesTrigger : ManyTimesTrigger
{
    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (!otherCollider.gameObject.CompareTag("Player")) return;
        platformerLES.GetTriggerSignal(triggerSignal+5, blockPlayer);
    }
}
