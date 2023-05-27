public class PatrolState : IState
{
    public void Execute(Enemy enemy)
    {
        enemy.Patrol();
    }
}