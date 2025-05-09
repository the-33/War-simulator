using UnityEngine;
using UnityEngine.AI;
using Utils.Attributes;
using Interfaces.IDamageable;

[RequireComponent(typeof(NavMeshAgent))]
public class BasicEnemyController : MonoBehaviour, IDamageable
{

    #region Private references
    private NavMeshAgent m_agent;
    #endregion

    #region Behaviour
    [Header("Behaviour")]
    public Transform m_target;
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

    public void onDeath()
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
