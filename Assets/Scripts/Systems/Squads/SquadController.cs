using UnityEngine;

public class SquadController : MonoBehaviour
{
    public int m_squadID;

    [SerializeField] private SquadEventChannelSO m_squadEventChannel;

    private ISquadMember m_squadMember;

    private void Awake()
    {
        m_squadMember = GetComponent<ISquadMember>();
        if (m_squadMember == null)
            Debug.LogError($"SquadController on {name} needs a component that implements ISquadMember.");
    }

    private void OnEnable()
    {
        m_squadEventChannel.OnEventRaised += HandleSquadEvent;
    }

    private void OnDisable()
    {
        m_squadEventChannel.OnEventRaised -= HandleSquadEvent;
    }

    private void HandleSquadEvent(SquadEvent squadEvent)
    {
        if (m_squadMember == null) return;
        if (squadEvent.SquadID != m_squadID || squadEvent.Sender == m_squadMember) return;
        m_squadMember?.OnSquadEvent(squadEvent);
    }

    public void SendSquadEvent(SquadEventType type, Object data)
    {
        if (m_squadID != 0)
        {
            var newEvent = new SquadEvent
            {
                SquadID = m_squadID,
                EventType = type,
                Sender = m_squadMember,
                Data = data
            };
            m_squadEventChannel?.RaiseEvent(newEvent);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.white;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"Squad ID: {m_squadID}");
    }
#endif
}
