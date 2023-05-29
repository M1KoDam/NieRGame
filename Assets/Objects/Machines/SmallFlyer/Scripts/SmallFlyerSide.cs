public class SmallFlyerSide : SmallFlyerFlightScene
{
    public override void Die()
    {
        IgnoreLayerCollision(true);
        base.Die();
        Rb.velocity -= fallDirection;
    }
}