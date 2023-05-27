public class AttackState : IState
{
    public void Execute(Enemy enemy)
    {
        enemy.Attack();
    }
}