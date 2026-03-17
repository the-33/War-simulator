public class DeadState : EnemyState
{
    public DeadState(EnemyController brain) : base(brain) { }

    public override void Enter()
    {
        Brain.Vision?.DisableBar();
        Brain.AnimatorController?.TriggerDeath();
    }
}
