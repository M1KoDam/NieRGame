public class ChaseState : IState
{
    public void Execute(Enemy enemy)
    {
        enemy.Chase();
    }
}