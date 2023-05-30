using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FlightSideLES : LevelEventSystem
{
    [SerializeField] private SmallFlyerSideRush smallFlyerRush;
    [SerializeField] private SmallFlyerSideSupport smallFlyerSupport;

    public override void CreateEvent()
    {
        if (CurrentEvent == 0 && !dialogueEventIsHappening)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
    }
}
