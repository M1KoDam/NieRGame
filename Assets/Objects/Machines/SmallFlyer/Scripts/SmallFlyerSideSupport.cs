using UnityEngine;

public class SmallFlyerSideSupport : SmallFlyer
{
    private bool _onFlyScene;
    [SerializeField] private Vector2 fallDirection = new Vector2(1, 0.25f);

    protected override IState state
        => hp <= 0
            ? new DeadState()
            : _onFlyScene
                ? new AttackState()
                : new GoToSceneState();
    
    protected override void Start()
    {
        base.Start();
        _onFlyScene = false;
        Physics2D.IgnoreLayerCollision((int)EnemyLayer, (int)PlayerLayer, true);
    }

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

    public override void GoToScene()
    {
        IgnoreLayerCollision(true);
        LookAtPlayer();
        
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

    protected override void GoToSpot()
    {
        Rb.velocity = EnemyToSpot.normalized * chaseSpeed;
    }

    public override void Die()
    {
        IgnoreLayerCollision(false);
        base.Die();
        Rb.velocity -= fallDirection*0.75f;
    }
}
