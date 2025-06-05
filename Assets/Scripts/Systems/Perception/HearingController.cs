using System;
using UnityEngine;

public class HearingController : MonoBehaviour
{
    public event Action<Vector3> OnSuspiciousSound;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
    }
#endif
}
