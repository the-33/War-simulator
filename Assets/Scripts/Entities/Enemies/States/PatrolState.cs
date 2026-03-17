public class PatrolState : EnemyState
{
    public PatrolState(EnemyController brain) : base(brain) { }

    public override void Enter() => Brain.PatrolBehaviour.Enter();
    public override void Tick()  => Brain.PatrolBehaviour.Tick();
    public override void Exit()  => Brain.PatrolBehaviour.Exit();
}
