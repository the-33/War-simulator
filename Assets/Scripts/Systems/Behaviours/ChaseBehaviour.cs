using UnityEngine;
using UnityEngine.AI;

public class ChaseBehaviour : MonoBehaviour, IBehaviour
{
    public bool IsFinished => _isFinished;
    private bool _isFinished;

    public float m_positionUpdateTime = 0.5f;
    private float _positionUpdateTimer = 0f;

    private Transform _target;

    private IAnimator _animator;
    private IMovementContext _movementContext;

    private void Awake()
    {
        _animator = GetComponent<IAnimator>();
        _movementContext = GetComponent<IMovementContext>();
        if (_movementContext == null)
            Debug.LogError("IMovementContext not found on " + gameObject.name);
    }

    public void Enter(object context = null)
    {
        if (context == null || !(context is Transform)) return;

        _target = (Transform)context;
        _movementContext.MoveTo(_target.position, MovemenMode.Quick);
        _positionUpdateTimer = 0;

    }

    public void Exit()
    {
        _movementContext.ResetPath();
        _positionUpdateTimer = 0;
        _target = null;
    }

    public void Tick()
    {
        if (_positionUpdateTimer < m_positionUpdateTime)
        {
            _positionUpdateTimer += Time.deltaTime;
            return;
        }
        _positionUpdateTimer = 0;
        _movementContext.MoveTo(_target.position, MovemenMode.Quick);
    }
}