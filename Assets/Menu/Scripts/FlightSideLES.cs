using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FlightSideLES : LevelEventSystem
{
    [SerializeField] private Transform bigFlyerSpot;
    [SerializeField] private Transform spawnSpot;
    
    private void FixedUpdate()
    {
        if (CurrentEvent == 1 && !EventIsHappening)
        {
            StartNextDialogue();
        }
    }
}
