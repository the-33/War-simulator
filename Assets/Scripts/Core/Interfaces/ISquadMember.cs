public interface ISquadMember
{
    /// <summary>
    /// Called when a squad event is received.
    /// </summary>
    /// <param name="squadEvent"></param>
    void OnSquadEvent(SquadEvent squadEvent);
}