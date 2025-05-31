using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolBehaviour : MonoBehaviour, IBehaviour
{
    public WaypointController m_waypointController;
    public float m_waitTime = 2f;

    [SerializeField] private Vector3[] _patrolPoints;
    private int _currentPoint = 0;
    private NavMeshAgent _agent;
    private float _waitTimer = 0f;

    public bool IsFinished => false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Enter(object context = null)
    {
        if ((_patrolPoints == null || _patrolPoints.Length == 0) && m_waypointController != null)
        {
            _patrolPoints = m_waypointController.Waypoints;
        }

        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            _patrolPoints = new Vector3[] { transform.position };
        }

        _currentPoint = 0;
        _agent?.SetDestination(_patrolPoints[_currentPoint]);
        _waitTimer = 0f;
    }

    public void Tick()
    {

        if (_patrolPoints.Length == 0 || (_patrolPoints.Length == 1 && !_agent.pathPending)) return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= m_waitTime)
            {
                _waitTimer = 0f;
                _currentPoint = (_currentPoint + 1) % _patrolPoints.Length;
                MoveToNextPoint();
            }
        }
        else
        {
            _waitTimer = 0f;
        }
    }

    private void MoveToNextPoint()
    {
        _agent.SetDestination(_patrolPoints[_currentPoint]);
    }

    public void Exit()
    {
    }
}