using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightSideLES : LevelEventSystem
{
    private void FixedUpdate()
    {
        if (CurrentEvent == 1 && !EventIsHappening)
        {
            StartNextDialogue();
        }
    }
}
