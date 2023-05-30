using UnityEngine;

public abstract class SmallFlyerFlightScene : SmallFlyer
{
    protected bool OnFlightScene;
    [SerializeField] protected Vector2 fallDirection = new Vector2(-1, -0.25f);

    protected override void Start()
    {
        base.Start();
        OnFlightScene = false;
        Physics2D.IgnoreLayerCollision(EnemyLayer, PlayerLayer, true);
    }

    public override void GoToScene()
    {
        IgnoreCollision(true);
        LookAtPlayer();

        if (EnemyToSpot.magnitude < 1f)
        {
            if (CurWaitTime <= 0)
            {
                ChangeSpotId();
                OnFlightScene = true;
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
        IgnoreCollision(true);
        Rb.velocity += fallDirection;
        base.Die();
    }
}