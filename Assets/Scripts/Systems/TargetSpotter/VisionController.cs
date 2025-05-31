using System;
using UnityEngine;

public class VisionController : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Suspicion Settings")]
    public float suspicionIncreaseRate = 10f;
    public float suspicionDecayRate = 5f;
    public float suspicionThreshold = 30f;
    public float confirmationThreshold = 60f;

    public float suspicionLevel { get; private set; } = 0f;

    public Action<Vector3> OnSuspiciousSight;
    public Action<Vector3> OnConfirmedSight;
    public Action OnLostSight;

    private Transform target;
    private float checkInterval = 0.2f;
    private float checkTimer = 0f;
    private bool hasConfirmedTarget = false;
    private bool hasTriggeredSuspicion = false;

    private void Start()
    {
        // Optionally find player target here
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            PerformVisionCheck();
        }

        HandleSuspicionDecay();
    }

    void PerformVisionCheck()
    {
        if (target == null) return;

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= viewRadius && Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
        {
            if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
            {
                suspicionLevel += suspicionIncreaseRate * checkInterval;
                suspicionLevel = Mathf.Min(suspicionLevel, confirmationThreshold);

                if (suspicionLevel >= confirmationThreshold && !hasConfirmedTarget)
                {
                    hasConfirmedTarget = true;
                    OnConfirmedSight?.Invoke(target.position);
                }
                else if (suspicionLevel >= suspicionThreshold && !hasTriggeredSuspicion)
                {
                    hasTriggeredSuspicion = true;
                    OnSuspiciousSight?.Invoke(target.position);
                }

                return;
            }
        }

        // Lost visual contact
        if (hasConfirmedTarget || hasTriggeredSuspicion)
        {
            OnLostSight?.Invoke();
        }
    }

    void HandleSuspicionDecay()
    {
        if (!hasConfirmedTarget && suspicionLevel > 0f)
        {
            suspicionLevel -= suspicionDecayRate * Time.deltaTime;
            suspicionLevel = Mathf.Max(suspicionLevel, 0f);

            if (suspicionLevel < suspicionThreshold && hasTriggeredSuspicion)
            {
                hasTriggeredSuspicion = false;
            }
        }
    }

#if UNITY_EDITOR
    public Vector3 DirFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
#endif
}
