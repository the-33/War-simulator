public interface IBehaviour
{
    /// <summary>
    /// Starts the behaviour.
    /// </summary>
    void Enter(object context = null);
    /// <summary>
    /// Updates the behaviour.
    /// </summary>
    void Tick();
    /// <summary>
    /// Stops the behaviour.
    /// </summary>
    void Exit();

    bool IsFinished { get; }
}