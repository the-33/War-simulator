using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{

    [Header("Vision Settings")]
    public float viewDistance = 15f;
    [Range(0, 360)]
    public float viewAngle = 110f;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    List<Transform> visibleTargets = new List<Transform>();

    private void Awake()
    {
        StartCoroutine(FOVRoutine());
    }

    IEnumerator FOVRoutine()
    {
        var delay = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return delay;
            FieldOfViewCheck();
        }
    }

    void FieldOfViewCheck()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

            if (angleToTarget < viewAngle / 2f)
            {
                float distToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position + Vector3.up, dirToTarget, distToTarget, obstructionMask))
                {
                    visibleTargets.Add(targetTransform);
                }
            }
        }
    }

    public bool CanSeeTarget(Transform target)
    {
        return visibleTargets.Contains(target);
    }

    public bool CanSeeAnyTarget()
    {
        return visibleTargets.Count > 0;
    }

    public Transform GetClosestVisibleTarget()
    {
        if (visibleTargets.Count == 0) return null;

        Transform closest = visibleTargets[0];
        float minDist = Vector3.Distance(transform.position, closest.position);

        foreach (Transform t in visibleTargets)
        {
            float dist = Vector3.Distance(transform.position, t.position);
            if (dist < minDist)
            {
                closest = t;
                minDist = dist;
            }
        }

        return closest;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Draw the view radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        //Draw the view angle lines
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);

        // Opcional: dibujar un sector de visión aproximado
        Gizmos.color = new Color(1, 1, 0, 0.1f);
        int segments = 30;
        float angleStep = viewAngle / segments;
        Vector3 prevPoint = transform.position + (Quaternion.Euler(0, -viewAngle / 2f, 0) * forward) * viewDistance;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -viewAngle / 2f + angleStep * i;
            Vector3 nextPoint = transform.position + (Quaternion.Euler(0, angle, 0) * forward) * viewDistance;
            Gizmos.DrawLine(transform.position, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
#endif
}