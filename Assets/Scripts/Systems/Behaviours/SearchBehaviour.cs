using UnityEngine;
using UnityEngine.AI;

public class SearchBehaviour : MonoBehaviour, IBehaviour
{
    private IMovementContext _movementContext;
    private Vector3 _searchZone;
    private float _searchTime;
    private float _searchTimer;
    private bool _isFinished;

    public bool IsFinished => _isFinished;

    private IAnimator _animator;

    public float m_speed;

    private void Awake()
    {
        _movementContext = GetComponent<IMovementContext>();
        _animator = GetComponent<IAnimator>();
    }

    public void Enter(object context = null)
    {
        if (context == null || !(context is SearchData))
        {
            Debug.LogError("SearchBehaviour requires a SearchData context.");
            return;
        }

        var data = (SearchData)context;
        _searchZone = data.Position;
        _searchTime = data.Duration;

        _movementContext.MoveTo(_searchZone);
        _searchTimer = 0f;
        _isFinished = false;
    }

    public void Exit()
    {
        _movementContext.ResetPath();
        _isFinished = false;
        _searchTimer = 0f;
    }

    public void Tick()
    {
        if (_isFinished) return;

        if (_movementContext.IsPathComplete)
        {
            _searchTimer += Time.deltaTime;
            if (_searchTimer >= _searchTime)
                _isFinished = true;
        }
    }
}

public struct SearchData
{
    public Vector3 Position;
    public float Duration;
}