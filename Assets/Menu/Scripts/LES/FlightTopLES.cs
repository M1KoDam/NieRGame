using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class FlightTopLES : FlightLES
{
    [Header("Boss")]
    [SerializeField] private Transform bigFlyerSpot;
    [SerializeField] private Transform spawnSpot;
    [SerializeField] private Animator bigFlyerHealthBarAnimator;
    [SerializeField] private BigFlyerHealthBar bigFlyerHealthBar;

    [Header("Control")] 
    [SerializeField] private Animator controlAnimator;
    
    [Header("Enemies")]
    [SerializeField] private BigFlyerTop bigFlyer;
    [SerializeField] private SmallFlyerTopRush smallFlyerRush;
    [SerializeField] private SmallFlyerTopSupport smallFlyerSupport;
    private static readonly int IsHide = Animator.StringToHash("IsHide");

    protected override void CreateEvent()
    {
        if (CurrentEvent == 0 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 1 && EventCompleted)
        {
            controlAnimator.SetBool(IsHide, false);
            Invoke(nameof(CloseController), 5f);
            SpawnFlyer(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyer(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });
            
            SpawnFlyer(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 2 && EventCompleted)
        {
            SpawnFlyer(smallFlyerRush, spawnSpots[0], new[] { moveSpots[4] });
            SpawnFlyer(smallFlyerRush, spawnSpots[1], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerRush, spawnSpots[2], new[] { moveSpots[0] });
            
            SpawnFlyer(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0],  moveSpots[4] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[3] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[7], new[] { moveSpots[2] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 3 && EventCompleted)
        {
            SpawnFlyer(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyer(smallFlyerRush, spawnSpots[1], new[] { moveSpots[1] });
            SpawnFlyer(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerRush, spawnSpots[3], new[] { moveSpots[3] });
            SpawnFlyer(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });

            SpawnFlyer(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0], moveSpots[2], moveSpots[4] });
            
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 4 && EventCompleted)
        {
            SpawnFlyer(smallFlyerRush, spawnSpots[0], new[] { moveSpots[0] });
            SpawnFlyer(smallFlyerRush, spawnSpots[1], new[] { moveSpots[1] });
            SpawnFlyer(smallFlyerRush, spawnSpots[2], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerRush, spawnSpots[3], new[] { moveSpots[3] });
            SpawnFlyer(smallFlyerRush, spawnSpots[4], new[] { moveSpots[4] });

            SpawnFlyer(smallFlyerSupport, spawnSpots[5], new[] { moveSpots[0] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[7], new[] { moveSpots[2] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[9], new[] { moveSpots[4] });

            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 5 && EventCompleted)
        {
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 6 && EventCompleted)
        {
            SpawnBigFlyer(bigFlyer, spawnSpots[7], new[] { moveSpots[2] });
            bigFlyerHealthBarAnimator.SetBool("IsHide", false);
            SpawnFlyer(smallFlyerSupport, spawnSpots[6], new[] { moveSpots[1],  moveSpots[0] });
            SpawnFlyer(smallFlyerSupport, spawnSpots[8], new[] { moveSpots[3],  moveSpots[4] });
            AttackEventIsHappening = true;
        }
        else if (CurrentEvent == 7 && EventCompleted)
        {
            bigFlyerHealthBarAnimator.SetBool("IsHide", true);
            dialogueEventIsHappening = true;
            StartNextDialogue();
        }
        else if (CurrentEvent == 8 && EventCompleted)
        {
            NextLevel();
        }
    }
    
    private void SpawnBigFlyer(SmallFlyerFlightScene flyerType, Transform spawnPosition, IEnumerable<Transform> moveSpots)
    {
        var enemy = Instantiate(flyerType, spawnPosition.position, transform.rotation);
        foreach (var moveSpot in moveSpots)
        {
            enemy.GiveMoveSpot(moveSpot);
        }

        bigFlyerHealthBar.bigFlyer = (BigFlyerTop)enemy;
        SmallFlyers.Add(enemy);
    }

    private void CloseController()
    {
        controlAnimator.SetBool(IsHide, true);
    }
}
