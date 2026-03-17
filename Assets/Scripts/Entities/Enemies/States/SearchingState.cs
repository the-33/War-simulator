public class SearchingState : EnemyState
{
    public SearchingState(EnemyController brain) : base(brain) { }

    public override void Enter() => Brain.SearchBehaviour.Enter(Brain.CreateSearchData());

    public override void Tick()
    {
        Brain.SearchBehaviour.Tick();
        if (Brain.SearchBehaviour.IsFinished)
            Brain.TryChangeState(EnemyStateEnum.IDDLE_PATROL);
    }

    public override void Exit() => Brain.SearchBehaviour.Exit();
}
