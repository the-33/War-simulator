using System;
using UnityEngine;

public class HearingController : MonoBehaviour
{

    public float m_hearingRange = 10f;

    public event Action<Vector3> OnSuspiciousSound;

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
