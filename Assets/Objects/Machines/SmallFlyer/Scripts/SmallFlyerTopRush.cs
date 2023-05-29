using UnityEngine;

public class SmallFlyerTopRush : SmallFlyerTop
{
    protected override IState State
        => hp <= 0
            ? new DeadState()
            : OnFlyScene
                ? EnemyToPlayer.magnitude <= maxAttackRaduis && Physics2D.Raycast(transform.position,
                    EnemyToPlayer, EnemyToPlayer.magnitude, layerGround).collider is null
                    ? new AttackState()
                    : new ChaseState()
                : new GoToSceneState();

    public override void Attack()
    {
        RushAttack();
    }
}