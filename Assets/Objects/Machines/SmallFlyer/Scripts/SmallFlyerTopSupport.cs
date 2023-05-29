using UnityEngine;

public class SmallFlyerTopSupport: SmallFlyerTop
{
    protected override IState State
        => hp <= 0
            ? new DeadState()
            : OnFlyScene
                ? new AttackState()
                : new GoToSceneState();
    
    public override void Attack()
    {
        IgnoreLayerCollision(false);
        LookAtPlayer();
        
        if (EnemyToSpot.magnitude < 1f)
        {
            if (CurWaitTime <= 0)
                ChangeSpotId();
            else
            {
                CurWaitTime -= Time.deltaTime;
                Wait();
                Brake();
            }
        }

        else
            GoToSpot();
        
        if (CanAttack)
        {
            CanAttack = false;
            Shoot();
            Invoke(nameof(WaitForAttack), attackRate);
        }
    }

    protected override void GoToSpot()
    {
        Rb.velocity = EnemyToSpot.normalized * chaseSpeed;
    }
}
