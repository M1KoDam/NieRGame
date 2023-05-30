using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FlightSideLES : LevelEventSystem
{
    public override void CreateEvent()
    {
        if (CurrentEvent == 1 && !EventIsHappening)
        {
            StartNextDialogue();
        }
    }
}
