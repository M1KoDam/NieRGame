using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FlightSideLES : LevelEventSystem
{
    [Header("Enemies")]
    [SerializeField] private SmallFlyerSideRush smallFlyerRush;
    [SerializeField] private SmallFlyerSideSupport smallFlyerSupport;

    protected override void CreateEvent()
    {
        if (CurrentEvent == 0 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 1 && EventCompleted)
        {
            SpawnFlyerSide(smallFlyerRush, spawnSpots[1], new[] { moveSpots[0] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[3], new[] { moveSpots[4] });
            
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 2 && EventCompleted)
        {
            SpawnFlyerSide(smallFlyerRush, spawnSpots[1], new[] { moveSpots[1] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[3], new[] { moveSpots[3] });

            SpawnFlyerSide(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0] });
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1] });
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[7], new[] { moveSpots[2] });
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3] });
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[9], new[] { moveSpots[4] });

            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 3 && EventCompleted)
        {
            SpawnFlyerSide(smallFlyerRush, spawnSpots[0], new[] { moveSpots[4] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[1], new[] { moveSpots[3] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[3], new[] { moveSpots[1] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[4], new[] { moveSpots[0] });
            
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[4], moveSpots[0] });
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3], moveSpots[2], moveSpots[1] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 4 && EventCompleted)
        {
            SpawnFlyerSide(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyerSide(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });
            
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyerSide(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 5 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 6 && EventCompleted)
        {
            NextLevel();
        }
    }
}
