using System;
using UnityEngine;

public class HearingController : MonoBehaviour
{
    [Header("Hearing Settings")]
    public float m_hearingRange = 10f;

    public event Action<Vector3> OnSuspiciousSound;

    private void OnEnable()  => SoundEmitter.Register(this);
    private void OnDisable() => SoundEmitter.Unregister(this);

    public bool IsWithinHearing(Vector3 position, float soundRadius)
    {
        float effectiveRange = Mathf.Max(m_hearingRange, soundRadius);
        return Vector3.Distance(transform.position, position) <= effectiveRange;
    }

    /// <summary>Called by SoundEmitter when this listener can hear the sound.</summary>
    public void HearSound(Vector3 position)
    {
        OnSuspiciousSound?.Invoke(position);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_hearingRange);
    }
#endif
}
