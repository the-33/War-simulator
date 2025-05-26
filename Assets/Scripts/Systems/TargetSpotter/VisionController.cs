using UnityEngine;

public class VisionController : MonoBehaviour
{
    private ITargetSpotter m_TargetSpotter;

    [Header("Vision Settings")]
    public float viewDistance = 15f;
    [Range(0, 360)]
    public float viewAngle = 110f;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    private void Awake()
    {
        m_TargetSpotter = GetComponent<ITargetSpotter>();
        if (m_TargetSpotter == null)
            Debug.LogError($"VisionController on {name} needs a component that implements ITargetSpotter.");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
    }
#endif
}
