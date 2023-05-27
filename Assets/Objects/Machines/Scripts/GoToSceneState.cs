public class GoToSceneState : IState
{
    public void Execute(Enemy enemy)
    {
        enemy.GoToScene();
    }
}