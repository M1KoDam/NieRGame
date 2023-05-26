using UnityEngine;

public class SmallFlyerSideStay : SmallFlyer
{
    private bool _onFlyScene;
    [SerializeField] private Vector2 _fallDirection = new Vector2(1, 0.25f);

    protected override State GetState
        => hp <= 0
            ? State.Dead
            : _onFlyScene
                ? State.Attack
                : State.GoToScene;

    protected override void Attack()
    {
        GetComponent<Collider2D>().enabled = true;
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

    protected override void GoToScene()
    {
        GetComponent<Collider2D>().enabled = false;

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

    protected override void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        base.Die();
        Rb.velocity -= _fallDirection;
    }
}
