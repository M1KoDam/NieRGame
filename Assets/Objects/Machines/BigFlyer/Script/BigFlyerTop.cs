using UnityEngine;

public class BigFlyerTop: SmallFlyerTop
{
    //[SerializeField] private Transform[] supportingGuns;

    protected override IState State
        => hp <= 0
            ? new DeadState()
            : OnFlyScene
                ? new AttackState()
                : new GoToSceneState();

    public override void Attack()
    {
        AttackType2();
    }

    private void AttackType1()
    {
        IgnoreLayerCollision(false);
        GoToShootingPosition();
        LookAtPlayer();
        if (CanAttack)
        {
            CanAttack = false;
            Shoot();
            Invoke(nameof(WaitForAttack), attackRate);
        }
    }
    
    private void AttackType2()
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
}
