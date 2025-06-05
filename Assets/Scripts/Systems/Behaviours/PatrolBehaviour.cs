using UnityEngine;
using UnityEngine.AI;

public class PatrolBehaviour : MonoBehaviour, IBehaviour
{
    public WaypointController m_waypointController;
    public bool IsFinished => false;

    public float m_waitTime = 2f;
    private float _waitTimer = 0f;

    [SerializeField] private Vector3[] _patrolPoints;
    private int _currentPoint = 0;

    private IAnimator _animator;
    private IMovementContext _movementContext;

    private void Awake()
    {
        _currentPoint = 0;
        _animator = GetComponent<IAnimator>();
        _movementContext = GetComponent<IMovementContext>();
        if (_movementContext == null)
            Debug.LogError("IMovementContext not found on " + gameObject.name);
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

        _movementContext.MoveTo(_patrolPoints[_currentPoint]);
        _waitTimer = 0f;
    }

    public void Tick()
    {
        if (_patrolPoints.Length == 0 || (_patrolPoints.Length == 1 && !_movementContext.HasPath))
            return;

        if (_movementContext.IsPathComplete)
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
        _movementContext.MoveTo(_patrolPoints[_currentPoint]);
    }

    public void Exit()
    {
    }
}