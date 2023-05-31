using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FlightTopLES : LevelEventSystem
{
    [SerializeField] private Transform bigFlyerSpot;
    [SerializeField] private Transform spawnSpot;
    
    [SerializeField] private BigFlyerTop bigFlyer;
    [SerializeField] private SmallFlyerTopRush smallFlyerRush;
    [SerializeField] private SmallFlyerTopSupport smallFlyerSupport;

    public override void CreateEvent()
    {
        if (CurrentEvent == 0 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 1 && EventCompleted)
        {
            SpawnFlyerTop(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });
            
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 2 && EventCompleted)
        {
            SpawnFlyerTop(smallFlyerRush, spawnSpots[0], new[] { moveSpots[4] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[1], new[] { moveSpots[2] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[2], new[] { moveSpots[0] });
            
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0],  moveSpots[4] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[3] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[7], new[] { moveSpots[2] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 3 && EventCompleted)
        {
            SpawnFlyerTop(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[1], new[] { moveSpots[1] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[3], new[] { moveSpots[3] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });

            SpawnFlyerTop(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0], moveSpots[2], moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 4 && EventCompleted)
        {
            SpawnFlyerTop(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[1], new[] { moveSpots[1] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[3], new[] { moveSpots[3] });
            SpawnFlyerTop(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });

            SpawnFlyerTop(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[7], new[] { moveSpots[2] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[9], new[] { moveSpots[4] });

            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 5 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 6 && EventCompleted)
        {
            SpawnFlyerTop(bigFlyer, spawnSpots[7], new[] { moveSpots[2] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyerTop(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 7 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 8 && EventCompleted)
        {
            NextLevel();
        }
    }
}
