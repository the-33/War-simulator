using System;
using UnityEngine;

[Serializable]
public abstract class SquadEvent
{
    public int SquadID;
    public ISquadMember Sender;
}

// This class serves as a base class for squad-related events.
public class EnemyDetectedEvent : SquadEvent
{
    public Vector3 Position;
    public GameObject Target;
}

public class TestingEvent : SquadEvent
{
    public string Message;
}