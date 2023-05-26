using UnityEngine;

public class SmallFlyerSideRush : SmallFlyer
{
    private bool _onFlyScene;
    [SerializeField] private Vector2 fallDirection = new Vector2(1, 0.25f);

    protected override IState state
        => hp <= 0
            ? new DeadState()
            : _onFlyScene
                ? EnemyToPlayer.magnitude <= maxAttackRaduis && Physics2D.Raycast(transform.position,
        EnemyToPlayer, EnemyToPlayer.magnitude, layerGround).collider is null
                    ? new AttackState()
                    : new ChaseState()
                : new GoToSceneState();

    protected override void Start()
    {
        base.Start();
        _onFlyScene = false;
        Physics2D.IgnoreLayerCollision((int)EnemyLayer, (int)PlayerLayer, true);
    }

    public override void GoToScene()
    {
        IgnoreLayerCollision(true);

        if (EnemyToSpot.magnitude < 1f)
        {
            if (CurWaitTime <= 0)
            {
                ChangeSpotId();
                _onFlyScene = true;
            }
            else
            {
                CurWaitTime -= Time.deltaTime;
                Wait();
                Brake();
            }
        }

        else
            GoToSpot();
    }

    public override void Die()
    {
        IgnoreLayerCollision(false);
        base.Die();
        Rb.velocity -= fallDirection;
    }
}
