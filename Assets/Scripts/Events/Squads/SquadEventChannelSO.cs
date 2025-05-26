using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Squads/Squad Event Channel")]
public class SquadEventChannelSO : ScriptableObject
{
    public event Action<SquadEvent> OnEventRaised;
    public void RaiseEvent<T>(T eventData) where T : SquadEvent
    {
        OnEventRaised?.Invoke(eventData);
    }
}
