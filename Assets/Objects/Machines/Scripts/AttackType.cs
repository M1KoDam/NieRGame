public abstract class AttackType
{
    public abstract void Execute(out bool finished);
    public abstract void Reset();
}