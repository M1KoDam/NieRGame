using UnityEngine;

public class FlightSideLES : FlightLES
{
    [Header("Enemies")]
    [SerializeField] private SmallFlyerSideRush smallFlyerRush;
    [SerializeField] private SmallFlyerSideSupport smallFlyerSupport;

    protected override void CreateEvent()
    {
        if (CurrentEvent == 0 && EventCompleted)
        {
            Physics2D.IgnoreLayerCollision(EnemyLayer, PlayerLayer, true);
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 1 && EventCompleted)
        {
            SpawnFlyer(smallFlyerRush, spawnSpots[1], new[] { moveSpots[0] });
            SpawnFlyer(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerRush, spawnSpots[3], new[] { moveSpots[4] });
            
            SpawnFlyer(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 2 && EventCompleted)
        {
            SpawnFlyer(smallFlyerRush, spawnSpots[1], new[] { moveSpots[1] });
            SpawnFlyer(smallFlyerRush, spawnSpots[3], new[] { moveSpots[3] });

            SpawnFlyer(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[7], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[9], new[] { moveSpots[4] });

            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 3 && EventCompleted)
        {
            SpawnFlyer(smallFlyerRush, spawnSpots[0], new[] { moveSpots[4] });
            SpawnFlyer(smallFlyerRush, spawnSpots[1], new[] { moveSpots[3] });
            SpawnFlyer(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerRush, spawnSpots[3], new[] { moveSpots[1] });
            SpawnFlyer(smallFlyerRush, spawnSpots[4], new[] { moveSpots[0] });
            
            SpawnFlyer(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[4], moveSpots[0] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3], moveSpots[2], moveSpots[1] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 4 && EventCompleted)
        {
            SpawnFlyer(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyer(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });
            
            SpawnFlyer(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 5 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 6 && EventCompleted)
        {
            Physics2D.IgnoreLayerCollision(EnemyLayer, PlayerLayer, false);
            NextLevel();
        }
    }
}
