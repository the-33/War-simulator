using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChaseBehaviour : MonoBehaviour, IBehaviour
{
    private bool _isFinished;
    public bool IsFinished => _isFinished;

    private NavMeshAgent _agent;

    private Transform _target;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Enter(object context = null)
    {
        if (context == null || !(context is Transform)) return;

        _target = (Transform)context;
        _agent.SetDestination(_target.position);
    }

    public void Exit()
    {
        _agent.ResetPath();
    }

    public void Tick()
    {

    }
}