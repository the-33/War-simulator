using System;
using UnityEngine;

[Serializable]
public class SquadEvent
{
    public int SquadID;
    public SquadEventType EventType;
    public ISquadMember Sender;
    public object Data;
}

public enum SquadEventType
{
    EnemyDetected,
    Testing
}