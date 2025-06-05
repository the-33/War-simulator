using UnityEngine;
using UnityEngine.AI;

public interface IMovementContext
{
    bool IsMoving { get; }
    bool IsTrulyStopped { get; }
    bool HasPath { get; }
    bool IsPathComplete { get; }

    void MoveTo(Vector3 destination, MovemenMode mode = MovemenMode.Normal);
    void StopMoving();
    void StartMoving();
    void ResetPath();
}

public enum MovemenMode
{
    Slow, Normal, Quick, Iddle
}