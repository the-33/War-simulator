using UnityEngine;

public class AlertedState : EnemyState
{
    private float _timer;

    public AlertedState(EnemyController brain) : base(brain) { }

    public override void Enter() => _timer = Brain.AlertedTime;

    public override void Tick()
    {
        if (Brain.HasValidTarget())
        {
            Brain.TryChangeState(EnemyStateEnum.CHASING);
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        Brain.TryChangeState(Utilities.RandBool(Brain.ReturnToPatrolChance)
            ? EnemyStateEnum.IDDLE_PATROL
            : EnemyStateEnum.SEARCHING);
    }

    public override void Exit() => _timer = 0f;
}
