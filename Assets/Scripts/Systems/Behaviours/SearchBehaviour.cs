using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SearchBehaviour : MonoBehaviour, IBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _searchZone;
    private float _searchTime;
    private float _searchTimer;
    private bool _isFinished;

    public bool IsFinished => _isFinished;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
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

        _agent.SetDestination(_searchZone);
        _searchTimer = 0f;
        _isFinished = false;
    }

    public void Exit()
    {
        _agent.ResetPath();
        _isFinished = false;
        _searchTimer = 0f;
    }

    public void Tick()
    {
        if (_agent.pathPending || _isFinished) return;

        if (_agent.remainingDistance <= _agent.stoppingDistance)
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