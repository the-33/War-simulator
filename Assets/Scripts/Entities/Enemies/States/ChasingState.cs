public class ChasingState : EnemyState
{
    public ChasingState(EnemyController brain) : base(brain) { }

    public override void Enter()
    {
        if (!Brain.HasValidTarget())
        {
            Brain.TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }
        Brain.ChaseBehaviour.Enter(Brain.Target);
    }

    public override void Tick()
    {
        if (!Brain.HasValidTarget())
        {
            Brain.TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }

        if (Brain.IsInAttackRange())
        {
            Brain.TryChangeState(EnemyStateEnum.ATTACKING);
            return;
        }

        Brain.ChaseBehaviour.Tick();
    }

    public override void Exit() => Brain.ChaseBehaviour.Exit();
}
