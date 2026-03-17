public interface IBehaviour
{
    void Tick();
    void Exit();
    bool IsFinished { get; }
}