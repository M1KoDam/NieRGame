public class DeadState : IState
{
    public void Execute(Enemy enemy)
    {
        enemy.Die();
    }
}