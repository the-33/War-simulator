using UnityEngine;
using UnityEngine.AI;
using Utils.Attributes;
using Interfaces.IDamageable;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyVision))] //Remove if vision is snapped to model element
[RequireComponent(typeof(EnemyHearing))]
public class EnemyController : MonoBehaviour, IDamageable
{

    #region Private references
    private NavMeshAgent m_agent;
    private EnemyVision m_vision;
    private EnemyHearing m_hearing;
    #endregion

    #region Behaviour
    [Header("Behaviour")]
    //State machine
    public EnemyStateEnum m_startingState = EnemyStateEnum.IDDLE_PATROL;
    [SerializeField][ReadOnly] private EnemyStateEnum m_currentState = EnemyStateEnum.IDDLE_PATROL;

    //Patrolling
    public WaypointController m_WaypointController;
    private Vector3[] m_patrolPoints;
    [SerializeField][ReadOnly] private int m_currentPatrolPoint = 0;

    //Alerted
    [SerializeField] private float m_alertedTime = 5f;
    private float m_alertedTimer = 0f;

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
        m_vision = GetComponent<EnemyVision>();
        m_hearing = GetComponent<EnemyHearing>();

        if (m_WaypointController != null)
        {
            m_patrolPoints = m_WaypointController.Waypoints;
            if (m_patrolPoints.Length == 0)
            {
                m_patrolPoints = new Vector3[1];
                m_patrolPoints[0] = transform.position;
            }
        }
        else
        {
            m_patrolPoints = new Vector3[1];
            m_patrolPoints[0] = transform.position;
        }

        ChangeState(m_startingState);
    }

    public void ChangeState(EnemyStateEnum newState)
    {
        m_currentState = newState;
        switch (m_currentState)
        {
            case EnemyStateEnum.IDDLE_PATROL:
                m_agent.SetDestination(m_patrolPoints[m_currentPatrolPoint]);
                break;
            case EnemyStateEnum.ALERTED:
                m_alertedTimer = m_alertedTime;
                break;
            default:
                break;
        }
    }

    private void Update()
    {

        CheckView();
        CheckSound();
        switch (m_currentState)
        {
            case EnemyStateEnum.IDDLE_PATROL:
                Patrol();
                break;
            case EnemyStateEnum.ALERTED:
                Alerted();
                break;
            case EnemyStateEnum.SEARCHING:
                Searching();
                break;
            case EnemyStateEnum.CHASING:
                Chasing();
                break;
            case EnemyStateEnum.ATTACKING:
                break;
            case EnemyStateEnum.LOST_TARGET:
                break;
            case EnemyStateEnum.CALLING_FOR_BACKUP:
                break;
            case EnemyStateEnum.DEAD:
                break;
        }
    }

    #region Enemy State Logic
    private void Patrol()
    {
        //Check if the enemy is close to the patrol point
        if (m_agent.remainingDistance == 0f && m_patrolPoints.Length > 1)
        {
            m_currentPatrolPoint++;
            if (m_currentPatrolPoint >= m_patrolPoints.Length)
            {
                m_currentPatrolPoint = 0;
            }

            //Set the next patrol point
            m_agent.SetDestination(m_patrolPoints[m_currentPatrolPoint]);
        }
    }

    private void Alerted()
    {
        m_alertedTimer -= Time.deltaTime;
        if (m_alertedTimer <= 0f)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                ChangeState(EnemyStateEnum.SEARCHING);
            }
            else
            {
                ChangeState(EnemyStateEnum.IDDLE_PATROL);
            }
        }
    }

    private void Searching()
    {
        //TODO: Make the enemy search for the player on the last known position
    }

    private void Chasing()
    {

    }
    #endregion

    private void CheckView()
    {
        if (m_vision.CanSeeAnyTarget())
        {

        }
        else
        {

        }
        //TODO: Check enemy view
    }

    private void CheckSound()
    {
        //TODO: Check enemy sound
    }

    public void OnDeath()
    {

    }
}

public enum EnemyStateEnum
{
    IDDLE_PATROL, //-> Alerted, Chasing
    ALERTED, //-> Chasing, Iddle, Searching
    SEARCHING, //-> Iddle, Chasing
    CHASING, //-> LostTarget, Attacking
    ATTACKING, //-> Chasing, LostTarget
    LOST_TARGET, //-> Searching, Iddle
    CALLING_FOR_BACKUP, //-> Iddle, Chasing
    DEAD,
}
