using UnityEngine;
using UnityEngine.AI;
using Utils.Attributes;
using Interfaces.IDamageable;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(VisionController))]
[RequireComponent(typeof(HearingController))]
[RequireComponent(typeof(SquadController))]
[RequireComponent(typeof(PatrolBehaviour))]
[RequireComponent(typeof(SearchBehaviour))]
[RequireComponent(typeof(ChaseBehaviour))]
public class EnemyController : MonoBehaviour, IDamageable, ISquadMember, IPerceptionReceiver
{
    #region Private references
    private VisionController m_vision;
    private HearingController m_hearing;
    private SquadController m_squadController;
    private PatrolBehaviour m_PatrolBehaviour;
    private SearchBehaviour m_SearchBehaviour;
    private ChaseBehaviour m_ChaseBehaviour;
    #endregion

    #region Behaviour
    [Header("Behaviour")]
    //State machine
    public EnemyStateEnum m_startingState = EnemyStateEnum.IDDLE_PATROL;
    [SerializeField][ReadOnly] private EnemyStateEnum m_currentState;

    //Alerted
    [SerializeField]
    private float m_alertedTime = 5f;
    private float m_alertedTimer = 0f;

    private Vector3? m_suspectPosition; // Position to search when alerted
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
        m_vision = GetComponent<VisionController>();
        m_hearing = GetComponent<HearingController>();
        m_squadController = GetComponent<SquadController>();
        m_PatrolBehaviour = GetComponent<PatrolBehaviour>();
        m_SearchBehaviour = GetComponent<SearchBehaviour>();
        m_ChaseBehaviour = GetComponent<ChaseBehaviour>();
    }

    private void Start()
    {
        ChangeState(m_startingState);
    }

    private void OnEnable()
    {
        //Subscribe to events
        m_vision.OnSuspiciousSight += OnSuspiciousSight;
        m_vision.OnConfirmedSight += OnConfirmedSight;
        m_hearing.OnSuspiciousSound += OnSuspiciousSound;
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        m_vision.OnSuspiciousSight -= OnSuspiciousSight;
        m_vision.OnConfirmedSight -= OnConfirmedSight;
        m_hearing.OnSuspiciousSound -= OnSuspiciousSound;
    }

    #region State Machine
    public void ChangeState(EnemyStateEnum newState)
    {
        var previousState = m_currentState;
        m_currentState = newState;
        OnExitState(previousState);
        OnEnterState(newState);
    }

    public bool TryChangeState(EnemyStateEnum newState)
    {
        if (m_currentState == newState)
            return false;

        if (validTransitions.TryGetValue(newState, out var allowedFromStates) &&
        allowedFromStates.Contains(m_currentState))
        {
            ChangeState(newState);
            return true;
        }
        return false;
    }

    public void OnEnterState(EnemyStateEnum state)
    {
        switch (state)
        {
            case EnemyStateEnum.IDDLE_PATROL:
                m_PatrolBehaviour?.Enter();
                break;
            case EnemyStateEnum.ALERTED:
                m_alertedTimer = m_alertedTime;
                break;
            case EnemyStateEnum.SEARCHING:
                m_SearchBehaviour?.Enter(new SearchData
                {
                    Position = m_suspectPosition ?? transform.position,
                    Duration = 5f // Default search duration, can be adjusted
                });
                break;
            case EnemyStateEnum.CHASING:
                m_ChaseBehaviour?.Enter();
                break;
            case EnemyStateEnum.ATTACKING:
            case EnemyStateEnum.LOST_TARGET:
            case EnemyStateEnum.CALLING_FOR_BACKUP:
            case EnemyStateEnum.DEAD:
            default:
                break;
        }
    }

    public void OnExitState(EnemyStateEnum state)
    {
        switch (state)
        {
            case EnemyStateEnum.IDDLE_PATROL:
                m_PatrolBehaviour?.Exit();
                break;
            case EnemyStateEnum.ALERTED:
                m_alertedTimer = 0f;
                break;
            case EnemyStateEnum.SEARCHING:
                m_SearchBehaviour?.Exit();
                break;
            case EnemyStateEnum.CHASING:
                m_ChaseBehaviour?.Exit();
                break;
            case EnemyStateEnum.ATTACKING:
            case EnemyStateEnum.LOST_TARGET:
            case EnemyStateEnum.CALLING_FOR_BACKUP:
            case EnemyStateEnum.DEAD:
            default:
                break;
        }
    }
    #endregion

    private void Update()
    {
        switch (m_currentState)
        {
            case EnemyStateEnum.IDDLE_PATROL:
                m_PatrolBehaviour?.Tick();
                break;
            case EnemyStateEnum.ALERTED:
                m_alertedTimer -= Time.deltaTime;
                if (m_alertedTimer <= 0f)
                {
                    if (Utilities.RandBool())
                    {
                        TryChangeState(EnemyStateEnum.IDDLE_PATROL);
                    }
                    else
                    {
                        TryChangeState(EnemyStateEnum.SEARCHING);
                    }
                }
                break;

            case EnemyStateEnum.SEARCHING:
                m_SearchBehaviour?.Tick();
                if (m_SearchBehaviour.IsFinished)
                {
                    TryChangeState(EnemyStateEnum.IDDLE_PATROL);
                }
                break;
            case EnemyStateEnum.CHASING:
                break;
            case EnemyStateEnum.ATTACKING:
            case EnemyStateEnum.LOST_TARGET:
            case EnemyStateEnum.CALLING_FOR_BACKUP:
            case EnemyStateEnum.DEAD:
            default:
                break;
        }
    }

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

    #region IPerceptionReciever
    public void OnSuspiciousSight(Vector3 position)
    {
        m_suspectPosition = position;
        TryChangeState(EnemyStateEnum.ALERTED);
    }

    public void OnConfirmedSight(Vector3 entity)
    {
        TryChangeState(EnemyStateEnum.CHASING);
    }

    public void OnSuspiciousSound(Vector3 position)
    {
        m_suspectPosition = position;
        TryChangeState(EnemyStateEnum.ALERTED);
    }
    #endregion

    private static readonly Dictionary<EnemyStateEnum, HashSet<EnemyStateEnum>> validTransitions = new()
    {
    { EnemyStateEnum.IDDLE_PATROL, new() { EnemyStateEnum.ALERTED, EnemyStateEnum.SEARCHING, EnemyStateEnum.LOST_TARGET, EnemyStateEnum.CALLING_FOR_BACKUP } },
    { EnemyStateEnum.ALERTED, new() { EnemyStateEnum.IDDLE_PATROL } },
    { EnemyStateEnum.SEARCHING, new() { EnemyStateEnum.ALERTED, EnemyStateEnum.LOST_TARGET } },
    { EnemyStateEnum.CHASING, new() { EnemyStateEnum.ALERTED, EnemyStateEnum.SEARCHING, EnemyStateEnum.CALLING_FOR_BACKUP, EnemyStateEnum.ATTACKING } },
    { EnemyStateEnum.ATTACKING, new() { EnemyStateEnum.CHASING } },
    { EnemyStateEnum.LOST_TARGET, new() { EnemyStateEnum.CHASING, EnemyStateEnum.ATTACKING } },
    { EnemyStateEnum.CALLING_FOR_BACKUP, new() { EnemyStateEnum.CHASING } },
    { EnemyStateEnum.DEAD, new() { EnemyStateEnum.IDDLE_PATROL, EnemyStateEnum.ALERTED, EnemyStateEnum.SEARCHING, EnemyStateEnum.CHASING, EnemyStateEnum.ATTACKING, EnemyStateEnum.LOST_TARGET, EnemyStateEnum.CALLING_FOR_BACKUP } }
    };
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


