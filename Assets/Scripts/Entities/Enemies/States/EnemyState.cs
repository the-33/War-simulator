public abstract class EnemyState
{
    protected readonly EnemyController Brain;

    protected EnemyState(EnemyController brain)
    {
        Brain = brain;
    }

    public virtual void Enter() { }
    public virtual void Tick()  { }
    public virtual void Exit()  { }
}
