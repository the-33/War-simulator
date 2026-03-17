using System;
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
    private sealed class EnemyStateDefinition
    {
        public Action Enter;
        public Action Tick;
        public Action Exit;
    }

    #region Private references
    private VisionController m_vision;
    private HearingController m_hearing;
    private SquadController m_squadController;
    private PatrolBehaviour m_PatrolBehaviour;
    private SearchBehaviour m_SearchBehaviour;
    private ChaseBehaviour m_ChaseBehaviour;
    private AttackBehaviour m_AttackBehaviour;
    private EnemyAnimatorController m_animatorController;
    #endregion

    #region Behaviour
    [Header("Behaviour")]
    //State machine
    public EnemyStateEnum m_startingState = EnemyStateEnum.IDDLE_PATROL;
    [SerializeField][ReadOnly] private EnemyStateEnum m_currentState;

    //Alerted
    [SerializeField]
    private float m_alertedTime = 5f;
    [SerializeField]
    private float m_searchDuration = 5f;
    [SerializeField, Range(0f, 1f)]
    private float m_returnToPatrolChance = 0.2f;
    private float m_alertedTimer = 0f;

    private Vector3? m_suspectPosition; // Position to search when alerted
    private Transform m_target; // Target to chase or attack
    private bool m_hasActiveState;
    private readonly Dictionary<EnemyStateEnum, EnemyStateDefinition> m_stateDefinitions = new();
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
        // Perceptions
        m_vision = GetComponent<VisionController>();
        m_hearing = GetComponent<HearingController>();

        // Behaviours
        m_squadController = GetComponent<SquadController>();
        m_PatrolBehaviour = GetComponent<PatrolBehaviour>();
        m_SearchBehaviour = GetComponent<SearchBehaviour>();
        m_ChaseBehaviour = GetComponent<ChaseBehaviour>();
        m_AttackBehaviour = GetComponent<AttackBehaviour>();

        //Other
        m_animatorController = GetComponent<EnemyAnimatorController>();

        ConfigureStateMachine();
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
        m_vision.OnLostSight += OnLostSight;
        m_hearing.OnSuspiciousSound += OnSuspiciousSound;
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        m_vision.OnSuspiciousSight -= OnSuspiciousSight;
        m_vision.OnConfirmedSight -= OnConfirmedSight;
        m_vision.OnLostSight -= OnLostSight;
        m_hearing.OnSuspiciousSound -= OnSuspiciousSound;
    }

    #region State Machine
    public void ChangeState(EnemyStateEnum newState)
    {
        if (m_hasActiveState && m_stateDefinitions.TryGetValue(m_currentState, out var currentStateDefinition))
        {
            currentStateDefinition.Exit?.Invoke();
        }

        m_currentState = newState;
        m_hasActiveState = true;

        if (m_stateDefinitions.TryGetValue(newState, out var newStateDefinition))
        {
            newStateDefinition.Enter?.Invoke();
        }
    }

    public bool TryChangeState(EnemyStateEnum newState)
    {
        if (m_hasActiveState && m_currentState == newState)
            return false;

        if (!m_hasActiveState)
        {
            ChangeState(newState);
            return true;
        }

        if (validTransitions.TryGetValue(m_currentState, out var allowedNextStates) &&
            allowedNextStates.Contains(newState))
        {
            ChangeState(newState);
            return true;
        }

        return false;
    }

    private void ConfigureStateMachine()
    {
        m_stateDefinitions.Clear();

        m_stateDefinitions[EnemyStateEnum.IDDLE_PATROL] = new EnemyStateDefinition
        {
            Enter = () => m_PatrolBehaviour?.Enter(),
            Tick = () => m_PatrolBehaviour?.Tick(),
            Exit = () => m_PatrolBehaviour?.Exit()
        };

        m_stateDefinitions[EnemyStateEnum.ALERTED] = new EnemyStateDefinition
        {
            Enter = EnterAlertedState,
            Tick = TickAlertedState,
            Exit = ExitAlertedState
        };

        m_stateDefinitions[EnemyStateEnum.SEARCHING] = new EnemyStateDefinition
        {
            Enter = EnterSearchState,
            Tick = TickSearchState,
            Exit = ExitSearchState
        };

        m_stateDefinitions[EnemyStateEnum.CHASING] = new EnemyStateDefinition
        {
            Enter = EnterChaseState,
            Tick = TickChaseState,
            Exit = ExitChaseState
        };

        m_stateDefinitions[EnemyStateEnum.ATTACKING] = new EnemyStateDefinition
        {
            Enter = EnterAttackState,
            Tick = TickAttackState,
            Exit = ExitAttackState
        };

        m_stateDefinitions[EnemyStateEnum.LOST_TARGET] = new EnemyStateDefinition
        {
            Enter = EnterLostTargetState,
            Tick = TickSearchState,
            Exit = ExitSearchState
        };

        m_stateDefinitions[EnemyStateEnum.CALLING_FOR_BACKUP] = new EnemyStateDefinition();
        m_stateDefinitions[EnemyStateEnum.DEAD] = new EnemyStateDefinition
        {
            Enter = EnterDeadState
        };
    }
    #endregion

    private void Update()
    {
        if (HasValidTarget())
        {
            m_suspectPosition = m_target.position;
        }

        if (!m_hasActiveState)
        {
            return;
        }

        if (m_stateDefinitions.TryGetValue(m_currentState, out var stateDefinition))
        {
            stateDefinition.Tick?.Invoke();
        }
    }

    private void EnterAlertedState()
    {
        m_alertedTimer = m_alertedTime;
    }

    private void TickAlertedState()
    {
        if (HasValidTarget())
        {
            TryChangeState(EnemyStateEnum.CHASING);
            return;
        }

        m_alertedTimer -= Time.deltaTime;
        if (m_alertedTimer > 0f)
        {
            return;
        }

        TryChangeState(Utilities.RandBool(m_returnToPatrolChance)
            ? EnemyStateEnum.IDDLE_PATROL
            : EnemyStateEnum.SEARCHING);
    }

    private void ExitAlertedState()
    {
        m_alertedTimer = 0f;
    }

    private void EnterSearchState()
    {
        m_SearchBehaviour?.Enter(CreateSearchData());
    }

    private void EnterLostTargetState()
    {
        m_SearchBehaviour?.Enter(CreateSearchData());
    }

    private void TickSearchState()
    {
        m_SearchBehaviour?.Tick();
        if (m_SearchBehaviour != null && m_SearchBehaviour.IsFinished)
        {
            TryChangeState(EnemyStateEnum.IDDLE_PATROL);
        }
    }

    private void ExitSearchState()
    {
        m_SearchBehaviour?.Exit();
    }

    private void EnterChaseState()
    {
        if (!HasValidTarget())
        {
            ClearTarget();
            TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }

        m_ChaseBehaviour?.Enter(m_target);
    }

    private void TickChaseState()
    {
        if (!HasValidTarget())
        {
            ClearTarget();
            TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }

        if (m_vision != null && m_vision.IsOnActionRange(m_target.position))
        {
            TryChangeState(EnemyStateEnum.ATTACKING);
            return;
        }

        m_ChaseBehaviour?.Tick();
    }

    private void ExitChaseState()
    {
        m_ChaseBehaviour?.Exit();
    }

    private void EnterAttackState()
    {
        if (!HasValidTarget())
        {
            ClearTarget();
            TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }

        m_AttackBehaviour?.Enter(m_target);
    }

    private void TickAttackState()
    {
        if (!HasValidTarget())
        {
            ClearTarget();
            TryChangeState(EnemyStateEnum.LOST_TARGET);
            return;
        }

        if (m_vision == null || !m_vision.IsOnActionRange(m_target.position))
        {
            TryChangeState(EnemyStateEnum.CHASING);
            return;
        }

        m_AttackBehaviour?.Tick();
    }

    private void ExitAttackState()
    {
        m_AttackBehaviour?.Exit();
    }

    private void EnterDeadState()
    {
        m_vision?.DisableBar();
        m_animatorController?.TriggerDeath();
    }

    private SearchData CreateSearchData()
    {
        return new SearchData
        {
            Position = m_suspectPosition ?? transform.position,
            Duration = m_searchDuration
        };
    }

    private bool HasValidTarget()
    {
        return m_target != null && m_target.gameObject.activeInHierarchy;
    }

    private void TrackTarget(Transform target)
    {
        if (target == null)
        {
            return;
        }

        m_target = target;
        m_suspectPosition = target.position;
    }

    private void ClearTarget()
    {
        m_target = null;
    }

    private bool IsCombatState(EnemyStateEnum state)
    {
        return state == EnemyStateEnum.CHASING || state == EnemyStateEnum.ATTACKING;
    }

    private void EngageKnownTarget()
    {
        if (!HasValidTarget())
        {
            return;
        }

        var desiredState = m_vision != null && m_vision.IsOnActionRange(m_target.position)
            ? EnemyStateEnum.ATTACKING
            : EnemyStateEnum.CHASING;

        if (!TryChangeState(desiredState) && desiredState == EnemyStateEnum.ATTACKING)
        {
            TryChangeState(EnemyStateEnum.CHASING);
        }
    }

    #region IDamageable
    public void OnDeath()
    {
        if (m_currentState == EnemyStateEnum.DEAD)
        {
            return;
        }

        ChangeState(EnemyStateEnum.DEAD);
    }

    public void OnDamaged()
    {
        if (m_currentState == EnemyStateEnum.DEAD)
        {
            return;
        }

        m_animatorController?.TriggerHit();
        m_vision?.SetMaxSuspicion();

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            TrackTarget(player);
            m_squadController?.SendSquadEvent(SquadEventType.UnderAttack, player);

            if (!IsCombatState(m_currentState))
            {
                TryChangeState(EnemyStateEnum.CHASING);
            }

            return;
        }

        m_suspectPosition = transform.position;
        m_squadController?.SendSquadEvent(SquadEventType.UnderAttack, null);

        if (!IsCombatState(m_currentState))
        {
            TryChangeState(EnemyStateEnum.ALERTED);
        }
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
                    if (!IsCombatState(m_currentState))
                    {
                        TryChangeState(EnemyStateEnum.ALERTED);
                    }
                }
                break;

            case SquadEventType.UnderAttack:
                if (squadEvent.Data is Transform attacker)
                {
                    m_suspectPosition = attacker.position;
                }

                if (!IsCombatState(m_currentState))
                {
                    TryChangeState(EnemyStateEnum.ALERTED);
                }
                break;

            case SquadEventType.Testing:
                break;

            default:
                break;
        }
    }
    #endregion

    #region IPerceptionReciever
    public void OnSuspiciousSight(Vector3 position)
    {
        m_suspectPosition = position;
        if (!IsCombatState(m_currentState))
        {
            TryChangeState(EnemyStateEnum.ALERTED);
        }
    }

    public void OnConfirmedSight(Transform entity)
    {
        if (entity == null || m_currentState == EnemyStateEnum.DEAD)
        {
            return;
        }

        TrackTarget(entity);
        m_squadController?.SendSquadEvent(SquadEventType.EnemyDetected, entity);
        EngageKnownTarget();
    }

    public void OnLostSight()
    {
        if (m_currentState == EnemyStateEnum.DEAD)
        {
            return;
        }

        if (HasValidTarget())
        {
            m_suspectPosition = m_target.position;
        }

        ClearTarget();

        if (IsCombatState(m_currentState))
        {
            TryChangeState(EnemyStateEnum.LOST_TARGET);
        }
    }

    public void OnSuspiciousSound(Vector3 position)
    {
        m_suspectPosition = position;
        if (!IsCombatState(m_currentState))
        {
            TryChangeState(EnemyStateEnum.ALERTED);
        }
    }

    #endregion

    private static readonly Dictionary<EnemyStateEnum, HashSet<EnemyStateEnum>> validTransitions = new()
    {
        { EnemyStateEnum.IDDLE_PATROL, new() { EnemyStateEnum.ALERTED, EnemyStateEnum.SEARCHING, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.ALERTED, new() { EnemyStateEnum.IDDLE_PATROL, EnemyStateEnum.SEARCHING, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.SEARCHING, new() { EnemyStateEnum.IDDLE_PATROL, EnemyStateEnum.ALERTED, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.CHASING, new() { EnemyStateEnum.ATTACKING, EnemyStateEnum.LOST_TARGET, EnemyStateEnum.ALERTED, EnemyStateEnum.CALLING_FOR_BACKUP, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.ATTACKING, new() { EnemyStateEnum.CHASING, EnemyStateEnum.LOST_TARGET, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.LOST_TARGET, new() { EnemyStateEnum.SEARCHING, EnemyStateEnum.IDDLE_PATROL, EnemyStateEnum.ALERTED, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.CALLING_FOR_BACKUP, new() { EnemyStateEnum.ALERTED, EnemyStateEnum.CHASING, EnemyStateEnum.DEAD } },
        { EnemyStateEnum.DEAD, new() }
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


