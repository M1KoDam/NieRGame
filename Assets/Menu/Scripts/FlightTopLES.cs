using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightTopLES : LevelEventSystem
{
    private void FixedUpdate()
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
