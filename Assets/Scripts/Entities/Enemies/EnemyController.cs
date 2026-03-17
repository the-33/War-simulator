using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Utils.Attributes;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(VisionController))]
[RequireComponent(typeof(HearingController))]
[RequireComponent(typeof(SquadController))]
[RequireComponent(typeof(PatrolBehaviour))]
[RequireComponent(typeof(SearchBehaviour))]
[RequireComponent(typeof(ChaseBehaviour))]
[RequireComponent(typeof(AttackBehaviour))]
[RequireComponent(typeof(EnemyAnimatorController))]
[RequireComponent(typeof(MovementContext))]
public class EnemyController : MonoBehaviour, IDamageable, ISquadMember, IPerceptionReceiver
{
    #region Private references
    private VisionController m_vision;
    private HearingController m_hearing;
    private SquadController m_squadController;
    private PatrolBehaviour m_PatrolBehaviour;
    private SearchBehaviour m_SearchBehaviour;
    private ChaseBehaviour m_ChaseBehaviour;
    private AttackBehaviour m_AttackBehaviour;
    private EnemyAnimatorController m_animatorController;
    private Transform m_cachedPlayer;
    #endregion

    #region Behaviour
    [Header("Behaviour")]
    public EnemyStateEnum m_startingState = EnemyStateEnum.IDDLE_PATROL;
    [SerializeField][ReadOnly] private EnemyStateEnum m_currentStateEnum;

    [SerializeField] private float m_alertedTime = 5f;
    [SerializeField] private float m_searchDuration = 5f;
    [SerializeField, Range(0f, 1f)] private float m_returnToPatrolChance = 0.2f;

    private Transform m_target;
    private Vector3? m_suspectPosition;
    private EnemyState m_currentState;
    private readonly Dictionary<EnemyStateEnum, EnemyState> m_states = new();
    #endregion

    #region Health
    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;
    public float m_maxHealth { get => _maxHealth; set => _maxHealth = value; }
    [SerializeField] private float _health = 100f;
    public float m_health { get => _health; set => _health = value; }
    #endregion

    // Internal API exposed to state classes
    internal PatrolBehaviour PatrolBehaviour   => m_PatrolBehaviour;
    internal SearchBehaviour SearchBehaviour   => m_SearchBehaviour;
    internal ChaseBehaviour  ChaseBehaviour    => m_ChaseBehaviour;
    internal AttackBehaviour AttackBehaviour   => m_AttackBehaviour;
    internal VisionController Vision           => m_vision;
    internal EnemyAnimatorController AnimatorController => m_animatorController;
    internal Transform Target                  => m_target;
    internal float AlertedTime                 => m_alertedTime;
    internal float ReturnToPatrolChance        => m_returnToPatrolChance;

    internal bool HasValidTarget() => m_target != null && m_target.gameObject.activeInHierarchy;
    internal void ClearTarget()    => m_target = null;
    internal bool IsInAttackRange() => m_vision != null && HasValidTarget() && m_vision.IsOnActionRange(m_target.position);
    internal SearchData CreateSearchData() => new SearchData
    {
        Position = m_suspectPosition ?? transform.position,
        Duration = m_searchDuration
    };

    private void Awake()
    {
        m_vision            = GetComponent<VisionController>();
        m_hearing           = GetComponent<HearingController>();
        m_squadController   = GetComponent<SquadController>();
        m_PatrolBehaviour   = GetComponent<PatrolBehaviour>();
        m_SearchBehaviour   = GetComponent<SearchBehaviour>();
        m_ChaseBehaviour    = GetComponent<ChaseBehaviour>();
        m_AttackBehaviour   = GetComponent<AttackBehaviour>();
        m_animatorController = GetComponent<EnemyAnimatorController>();

        m_states[EnemyStateEnum.IDDLE_PATROL]       = new PatrolState(this);
        m_states[EnemyStateEnum.ALERTED]            = new AlertedState(this);
        m_states[EnemyStateEnum.SEARCHING]          = new SearchingState(this);
        m_states[EnemyStateEnum.CHASING]            = new ChasingState(this);
        m_states[EnemyStateEnum.ATTACKING]          = new AttackingState(this);
        m_states[EnemyStateEnum.LOST_TARGET]        = new LostTargetState(this);
        m_states[EnemyStateEnum.CALLING_FOR_BACKUP] = new CallingForBackupState(this);
        m_states[EnemyStateEnum.DEAD]               = new DeadState(this);
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) m_cachedPlayer = playerObj.transform;
        ChangeState(m_startingState);
    }

    private void OnEnable()
    {
        m_vision.OnSuspiciousSight  += OnSuspiciousSight;
        m_vision.OnConfirmedSight   += OnConfirmedSight;
        m_vision.OnLostSight        += OnLostSight;
        m_hearing.OnSuspiciousSound += OnSuspiciousSound;
    }

    private void OnDisable()
    {
        m_vision.OnSuspiciousSight  -= OnSuspiciousSight;
        m_vision.OnConfirmedSight   -= OnConfirmedSight;
        m_vision.OnLostSight        -= OnLostSight;
        m_hearing.OnSuspiciousSound -= OnSuspiciousSound;
    }

    #region State Machine
    public void ChangeState(EnemyStateEnum newState)
    {
        m_currentState?.Exit();
        m_currentStateEnum = newState;
        m_currentState = m_states[newState];
        m_currentState.Enter();
    }

    public bool TryChangeState(EnemyStateEnum newState)
    {
        if (m_currentState != null)
        {
            if (m_currentStateEnum == newState) return false;
            if (!validTransitions.TryGetValue(m_currentStateEnum, out var allowed) || !allowed.Contains(newState))
                return false;
        }

        ChangeState(newState);
        return true;
    }
    #endregion

    private void Update()
    {
        if (HasValidTarget())
            m_suspectPosition = m_target.position;

        m_currentState?.Tick();
    }

    private void TrackTarget(Transform target)
    {
        if (target == null) return;
        m_target = target;
        m_suspectPosition = target.position;
    }

    private bool IsCombatState() =>
        m_currentStateEnum == EnemyStateEnum.CHASING || m_currentStateEnum == EnemyStateEnum.ATTACKING;

    private void EngageKnownTarget()
    {
        if (!HasValidTarget()) return;
        var desired = IsInAttackRange() ? EnemyStateEnum.ATTACKING : EnemyStateEnum.CHASING;
        if (!TryChangeState(desired) && desired == EnemyStateEnum.ATTACKING)
            TryChangeState(EnemyStateEnum.CHASING);
    }

    #region IDamageable
    public void OnDeath()
    {
        if (m_currentStateEnum == EnemyStateEnum.DEAD) return;
        ChangeState(EnemyStateEnum.DEAD);
    }

    public void OnDamaged()
    {
        if (m_currentStateEnum == EnemyStateEnum.DEAD) return;

        m_animatorController?.TriggerHit();
        m_vision?.SetMaxSuspicion();

        if (m_cachedPlayer != null)
        {
            TrackTarget(m_cachedPlayer);
            m_squadController?.SendSquadEvent(SquadEventType.UnderAttack, m_cachedPlayer);
            if (!IsCombatState()) TryChangeState(EnemyStateEnum.CHASING);
            return;
        }

        m_suspectPosition = transform.position;
        m_squadController?.SendSquadEvent(SquadEventType.UnderAttack, null);
        if (!IsCombatState()) TryChangeState(EnemyStateEnum.ALERTED);
    }
    #endregion

    #region ISquadMember
    public void OnSquadEvent(SquadEvent squadEvent)
    {
        switch (squadEvent.EventType)
        {
            case SquadEventType.EnemyDetected:
                if (squadEvent.Data is Transform target)
                {
                    m_suspectPosition = target.position;
                    if (!IsCombatState()) TryChangeState(EnemyStateEnum.ALERTED);
                }
                break;

            case SquadEventType.UnderAttack:
                if (squadEvent.Data is Transform attacker)
                    m_suspectPosition = attacker.position;
                if (!IsCombatState()) TryChangeState(EnemyStateEnum.ALERTED);
                break;
        }
    }
    #endregion

    #region IPerceptionReceiver
    public void OnSuspiciousSight(Vector3 position)
    {
        m_suspectPosition = position;
        if (!IsCombatState()) TryChangeState(EnemyStateEnum.ALERTED);
    }

    public void OnConfirmedSight(Transform entity)
    {
        if (entity == null || m_currentStateEnum == EnemyStateEnum.DEAD) return;
        TrackTarget(entity);
        m_squadController?.SendSquadEvent(SquadEventType.EnemyDetected, entity);
        EngageKnownTarget();
    }

    public void OnLostSight()
    {
        if (m_currentStateEnum == EnemyStateEnum.DEAD) return;
        if (HasValidTarget()) m_suspectPosition = m_target.position;
        ClearTarget();
        if (IsCombatState()) TryChangeState(EnemyStateEnum.LOST_TARGET);
    }

    public void OnSuspiciousSound(Vector3 position)
    {
        m_suspectPosition = position;
        if (!IsCombatState()) TryChangeState(EnemyStateEnum.ALERTED);
    }
    #endregion

    private static readonly Dictionary<EnemyStateEnum, HashSet<EnemyStateEnum>> validTransitions = new()
    {
        { EnemyStateEnum.IDDLE_PATROL,       new() { EnemyStateEnum.ALERTED, EnemyStateEnum.SEARCHING, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.ALERTED,            new() { EnemyStateEnum.IDDLE_PATROL, EnemyStateEnum.SEARCHING, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.SEARCHING,          new() { EnemyStateEnum.IDDLE_PATROL, EnemyStateEnum.ALERTED, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.CHASING,            new() { EnemyStateEnum.ATTACKING, EnemyStateEnum.LOST_TARGET, EnemyStateEnum.ALERTED, EnemyStateEnum.CALLING_FOR_BACKUP, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.ATTACKING,          new() { EnemyStateEnum.CHASING, EnemyStateEnum.LOST_TARGET, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.LOST_TARGET,        new() { EnemyStateEnum.SEARCHING, EnemyStateEnum.IDDLE_PATROL, EnemyStateEnum.ALERTED, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.CALLING_FOR_BACKUP, new() { EnemyStateEnum.ALERTED, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.DEAD,               new() }
    };
}

public enum EnemyStateEnum
{
    IDDLE_PATROL,
    ALERTED,
    SEARCHING,
    CHASING,
    ATTACKING,
    LOST_TARGET,
    CALLING_FOR_BACKUP,
    DEAD,
}


