using UnityEngine;

public class HearingController : MonoBehaviour
{
    private ITargetSpotter m_TargetSpotter;

    private void Awake()
    {
        m_TargetSpotter = GetComponent<ITargetSpotter>();
        if (m_TargetSpotter == null)
            Debug.LogError($"HearingController on {name} needs a component that implements ITargetSpotter.");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
    }
#endif
}
