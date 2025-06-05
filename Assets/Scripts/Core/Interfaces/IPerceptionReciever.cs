
using UnityEngine;

public interface IPerceptionReceiver
{
    #region Sight
    /// <summary>
    /// Called when an entity is sighted.
    /// </summary>
    /// <param name="position"></param>
    void OnSuspiciousSight(Vector3 position);

    /// <summary>
    /// Called when an entity is confirmed sighted.
    /// </summary>
    /// <param name="entity"></param>
    void OnConfirmedSight(Transform entity);

    /// <summary>
    /// Called when the entity loses sight of a target.
    /// </summary>
    void OnLostSight();
    #endregion

    #region Hearing
    /// <summary>
    /// Called when a suspicious sound is heard.
    /// </summary>
    /// <param name="position"></param>
    void OnSuspiciousSound(Vector3 position);
    #endregion
}