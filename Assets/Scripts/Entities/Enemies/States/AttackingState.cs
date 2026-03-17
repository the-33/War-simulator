public class AttackingState : EnemyState
{
    public AttackingState(EnemyController brain) : base(brain) { }

    public override void Enter()
    {
        if (!Brain.HasValidTarget())
        {
            Brain.TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }
        Brain.AttackBehaviour.Enter(Brain.Target);
    }

    public override void Tick()
    {
        if (!Brain.HasValidTarget())
        {
            Brain.TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }

        if (!Brain.IsInAttackRange())
        {
            Brain.TryChangeState(EnemyStateEnum.CHASING);
            return;
        }

        Brain.AttackBehaviour.Tick();
    }

    public override void Exit() => Brain.AttackBehaviour.Exit();
}
