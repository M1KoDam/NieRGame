using UnityEngine;

public class SmallFlyerTopSupport: SmallFlyerTop
{
    protected override IState State
        => hp <= 0
            ? new DeadState()
            : moveSpot.Count == 0
                ? new IdleState()
                : OnFlightScene
                    ? new AttackState()
                    : new GoToSceneState();
    
    public override void Attack()
    {
        SupportAttack();
    }

    protected override void GoToSpot()
    {
        Rb.velocity = EnemyToSpot.normalized * chaseSpeed;
    }
}
