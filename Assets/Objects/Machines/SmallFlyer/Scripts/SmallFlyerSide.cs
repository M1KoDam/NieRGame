using UnityEngine;

public class SmallFlyerSide : SmallFlyer
{
    protected bool OnFlyScene;
    [SerializeField] private Vector2 fallDirection = new Vector2(1, 0.25f);
    
    protected override void Start()
    {
        base.Start();
        OnFlyScene = false;
        Physics2D.IgnoreLayerCollision(EnemyLayer, PlayerLayer, true);
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
                OnFlyScene = true;
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