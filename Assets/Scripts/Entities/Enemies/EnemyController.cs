using UnityEngine;
using UnityEngine.AI;
using Utils.Attributes;
using Interfaces.IDamageable;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamageable
{

    #region Private references
    private NavMeshAgent m_agent;
    #endregion

    #region Behaviour
    [Header("Behaviour")]
    public Transform m_target;
    [SerializeField][ReadOnly] private int m_currentPatrolPointIndex = 0;
    public Transform[] m_patrolPoints;
    public EnemyState m_startingState = EnemyState.Idle;
    [SerializeField][ReadOnly] private EnemyState m_currentState = EnemyState.Idle;
    #endregion

    #region Health
    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;
    public float m_maxHealth { get => _maxHealth; set => _maxHealth = value; }
    [SerializeField] private float _health = 100f;
    public float m_health { get => _health; set => _health = value; }
    #endregion

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        ChangeState(m_startingState);
    }

    public void ChangeState(EnemyState newState)
    {
        m_currentState = newState;
        switch (m_currentState)
        {
            case EnemyState.Idle:
                m_agent.isStopped = true;
                break;
            case EnemyState.Patrol:
                m_agent.isStopped = false;
                break;
            case EnemyState.Chase:
                m_agent.isStopped = false;
                break;
            case EnemyState.Attack:
                m_agent.isStopped = true;
                break;
            case EnemyState.Dead:
                m_agent.isStopped = true;
                break;
        }
    }

    private void Update()
    {
        CheckView();
        switch (m_currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Patrol:
                break;

            case EnemyState.Chase:
                break;

            case EnemyState.Attack:
                break;

            case EnemyState.Dead:
                break;
        }
    }

    private void CheckView()
    {
        if (m_currentState == EnemyState.Dead) return;
    }

    public void OnDeath()
    {
    }
}

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Dead
}
