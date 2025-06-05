using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VisionController : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetMask; // Layer del Player
    public LayerMask obstacleMask;

    [Range(0, 1)]
    public float actionRangeMult = 0.8f;

    [Header("Suspicion Settings")]
    public float suspicionIncreaseRate = 10f;
    public float suspicionDecayRate = 5f;
    public float suspicionThreshold = 30f;
    public float confirmationThreshold = 60f;
    public float overflowThreshold = 50f;

    [Header("Display Settings")]
    [SerializeField] private Slider suspicionSlider;

    [Header("Offset")]
    public Transform viewTransform;

    public float suspicionLevel { get; private set; } = 0f;

    public Action<Vector3> OnSuspiciousSight;
    public Action<Transform> OnConfirmedSight;
    public Action OnLostSight;

    private float checkInterval = 0.2f;
    private Transform player;
    private Vector3 lastSeenPosition;

    private bool isSeeingPlayer = false;
    private bool hasTriggeredSuspicion = false;
    private bool hasConfirmedTarget = false;
    private bool hasLostSight = false;

    private void OnEnable() => StartCoroutine(VisionCheckCoroutine());

    private void OnDisable() => StopAllCoroutines();

    IEnumerator VisionCheckCoroutine()
    {
        while (true)
        {
            PerformVisionCheck();
            HandleSuspicionDecay();
            HandleSlider();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void PerformVisionCheck()
    {
        if (player == null)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    player = hit.transform;
                    break;
                }
            }

            if (player == null)
            {
                isSeeingPlayer = false;
                return;
            }
        }

        Vector3 flatForward = new Vector3(viewTransform.forward.x, 0, viewTransform.forward.z).normalized;

        Vector3 dirToPlayer = (player.position - viewTransform.position).normalized;
        float distance = Vector3.Distance(viewTransform.position, player.position);
        float angleBetween = Vector3.Angle(flatForward, dirToPlayer);

        bool inViewRange = distance <= viewRadius && angleBetween <= viewAngle / 2f;

        if (!inViewRange)
        {
            player = null;
            isSeeingPlayer = false;
            return;
        }

        bool lineClear = !Physics.Linecast(viewTransform.position, player.position, obstacleMask);

        if (lineClear)
        {
            lastSeenPosition = player.position;
            suspicionLevel += suspicionIncreaseRate * checkInterval;
            suspicionLevel = Mathf.Min(suspicionLevel, confirmationThreshold + overflowThreshold);

            if (!hasTriggeredSuspicion && suspicionLevel >= suspicionThreshold)
            {
                hasTriggeredSuspicion = true;
                OnSuspiciousSight?.Invoke(lastSeenPosition);
            }

            if (!hasConfirmedTarget && suspicionLevel >= confirmationThreshold)
            {
                hasConfirmedTarget = true;
                OnConfirmedSight?.Invoke(player);
                suspicionLevel = confirmationThreshold + overflowThreshold;
                hasLostSight = false;
            }

            isSeeingPlayer = true;
        }
        else
        {
            isSeeingPlayer = false;
        }
    }

    void HandleSuspicionDecay()
    {
        if (!isSeeingPlayer && suspicionLevel > 0f)
        {
            suspicionLevel -= suspicionDecayRate * checkInterval;
            suspicionLevel = Mathf.Max(suspicionLevel, 0f);

            if (suspicionLevel < suspicionThreshold)
            {
                hasTriggeredSuspicion = false;
            }

            if (suspicionLevel < confirmationThreshold)
            {
                hasConfirmedTarget = false;
                if (!hasLostSight)
                {
                    hasLostSight = true;
                    OnLostSight?.Invoke();
                }
            }
        }
    }

    void HandleSlider()
    {
        if (suspicionSlider == null) return;

        suspicionSlider.gameObject.SetActive(suspicionLevel > 0f);


        if (suspicionLevel >= suspicionThreshold && suspicionLevel < confirmationThreshold)
        {
            suspicionSlider.fillRect.GetComponent<Image>().color = Color.yellow; // Suspicion level
        }
        else if (suspicionLevel >= confirmationThreshold)
        {
            suspicionSlider.fillRect.GetComponent<Image>().color = Color.red; // Confirmed sight
        }
        else
        {
            suspicionSlider.fillRect.GetComponent<Image>().color = Color.green; // No suspicion
        }

        suspicionSlider?.SetValueWithoutNotify(suspicionLevel / confirmationThreshold);
    }

    public bool IsOnActionRange(Vector3 target)
    {
        Vector3 flatForward = new Vector3(viewTransform.forward.x, 0, viewTransform.forward.z).normalized;
        Vector3 dirToTarget = (target - viewTransform.position).normalized;
        float distance = Vector3.Distance(viewTransform.position, target);
        float angleBetween = Vector3.Angle(flatForward, dirToTarget);

        return distance <= (viewRadius * actionRangeMult) && angleBetween <= viewAngle / 2f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 flatForward = new Vector3(viewTransform.forward.x, 0, viewTransform.forward.z).normalized;

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireArc(viewTransform.position, Vector3.up, Quaternion.Euler(0, -viewAngle / 2, 0) * flatForward, viewAngle, viewRadius);

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireArc(viewTransform.position, Vector3.up, Quaternion.Euler(0, -viewAngle / 2, 0) * flatForward, viewAngle, viewRadius * actionRangeMult);
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
#endif
}
