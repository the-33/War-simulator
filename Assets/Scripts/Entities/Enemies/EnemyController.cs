using UnityEngine;
using UnityEngine.AI;
using Utils.Attributes;
using Interfaces.IDamageable;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(VisionController))] //Remove if vision is snapped to model element
[RequireComponent(typeof(HearingController))]
[RequireComponent(typeof(SquadController))]
public class EnemyController : MonoBehaviour, IDamageable, ISquadMember, ITargetSpotter
{

    #region Private references
    private NavMeshAgent m_agent;
    private VisionController m_vision;
    private HearingController m_hearing;
    private SquadController m_squadController;

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

    //Spotting
    [SerializeField] private float m_spottingTime = 2f;
    private float m_spottingTimer = 0f;

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
        m_vision = GetComponent<VisionController>();
        m_hearing = GetComponent<HearingController>();
        m_squadController = GetComponent<SquadController>();

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

            //Send Testing event
            TestingEvent test = new TestingEvent
            {
                Message = $"Enemy reached patrol point {m_currentPatrolPoint}"
            };

            m_squadController.SendSquadEvent(test);

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

    #region IDamageable
    public void OnDeath()
    {

    }

    public void OnDamaged()
    {

    }
    #endregion

    #region ISquadMember
    public void OnSquadEvent(SquadEvent squadEvent)
    {
        //Add Squad Event Logic here
        if (squadEvent is TestingEvent testEvent)
        {
            Debug.Log($"Enemy {name} received squad event: {testEvent.Message}");
        }
        //Add other squad events here...

        else
        {
            Debug.LogWarning($"Enemy {name} received unknown squad event.");
        }
    }
    #endregion


    #region ITargetSpotter
    public void SpotTarget()
    {
        m_spottingTimer += Time.deltaTime;
        if (m_spottingTimer >= m_spottingTime)
        {
            m_spottingTimer = 0f;
            // Logic to handle target spotted
            Debug.Log($"{name} spotted a target!");
            ChangeState(EnemyStateEnum.CHASING);
        } else
        {

        }
    }
    #endregion
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
