using UnityEngine;

public class SmallFlyerSideRush : SmallFlyerSide
{
    protected override IState State
        => hp <= 0
            ? new DeadState()
            : moveSpot.Count == 0
                ? new IdleState()
                : OnFlightScene
                    ? EnemyToPlayer.magnitude <= maxAttackRaduis && Physics2D.Raycast(transform.position,
            EnemyToPlayer, EnemyToPlayer.magnitude, layerGround).collider is null
                        ? new AttackState()
                        : new ChaseState()
                    : new GoToSceneState();
}
