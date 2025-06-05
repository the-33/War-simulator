using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MovementContext : MonoBehaviour, IMovementContext
{

    public float m_slowSpeed = 1f;
    public float m_normalSpeed = 3.5f;
    public float m_quickSpeed = 6f;


    private NavMeshAgent _agent;

    #region IMovementContext
    public bool IsMoving =>
    !_agent.pathPending &&
    _agent.hasPath &&
    _agent.remainingDistance > _agent.stoppingDistance &&
    _agent.velocity.sqrMagnitude > 0.01f;

    public bool IsTrulyStopped =>
        !_agent.pathPending &&
        (_agent.remainingDistance <= _agent.stoppingDistance) &&
        (!_agent.hasPath || _agent.velocity.sqrMagnitude < 0.01f);

    public bool HasPath => _agent.hasPath;

    public bool IsPathComplete =>
        !_agent.pathPending &&
        _agent.remainingDistance <= _agent.stoppingDistance;
    #endregion

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination, MovemenMode mode = MovemenMode.Normal)
    {
        if (mode == MovemenMode.Iddle)
        {
            ResetPath();
            return;
        }

        switch (mode)
        {
            case MovemenMode.Slow:
                _agent.speed = m_slowSpeed;
                break;
            case MovemenMode.Normal:
                _agent.speed = m_normalSpeed;
                break;
            case MovemenMode.Quick:
                _agent.speed = m_quickSpeed;
                break;
            default:
                _agent.speed = m_normalSpeed;
                break;
        }

        _agent.destination = destination;
    }

    public void StopMoving()
    {
        _agent.isStopped = true;
    }

    public void ResetPath()
    {
        _agent.ResetPath();
    }

    public void StartMoving()
    {
        _agent.isStopped = false;
    }
}
