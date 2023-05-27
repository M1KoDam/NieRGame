using UnityEngine;

public class SmallFlyerTopSupport: SmallFlyerSideSupport
{
    protected override void Wait()
    {
        Rb.velocity = Vector2.zero;
    }
    
    public override void Die()
    {
        base.Die();
        if (((Vector2)transform.localScale).magnitude >= 0.01f)
            transform.localScale -= new Vector3(0.005f, 0.005f, 0);
    }
}
