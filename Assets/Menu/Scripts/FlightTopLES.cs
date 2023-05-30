using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightTopLES : LevelEventSystem
{
    [SerializeField] private Transform bigFlyerSpot;
    [SerializeField] private Transform spawnSpot;
    
    public override void CreateEvent()
    {
        if (CurrentEvent == 1 && !EventIsHappening)
        {
            StartNextDialogue();
        }
        if (CurrentEvent == 2 && !EventIsHappening)
        {
            StartNextDialogue();
        }
    }
}
