public class IdleState : IState
{
    public void Execute(Enemy enemy)
    {
        enemy.DoIdle();
    }
}