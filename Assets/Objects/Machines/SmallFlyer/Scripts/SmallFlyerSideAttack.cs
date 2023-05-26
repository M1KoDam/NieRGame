using UnityEngine;

public class SmallFlyerSideAttack : SmallFlyer
{
    private bool _onFlyScene;
    [SerializeField] private Vector2 _fallDirection = new Vector2(1, 0.25f);

    protected override State GetState
        => _onFlyScene
        ? base.GetState
        : State.GoToScene;
    
    protected override void GoToScene()
    {
        GetComponent<Collider2D>().enabled = false;

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
    
    protected override void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        base.Die();
        Rb.velocity -= _fallDirection;
    }
}
