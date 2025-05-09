using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionController : MonoBehaviour
{
    [Header("View properties")]
    public float m_viewRadius;
    public float m_viewAngle;

    [Header("LayerMasks")]
    public LayerMask m_targetMask;
    public LayerMask m_obstructionMask;

    public List<Transform> visibleTargets = new List<Transform>();

    #region Timer
    private float m_checkInterval = 0.2f;
    private float m_checkTimer;
    #endregion

    public void Update()
    {
        m_checkTimer += Time.deltaTime;
        if (m_checkTimer >= m_checkInterval)
        {
            m_checkTimer = 0f;
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, m_viewRadius, m_targetMask);

        foreach (var col in targetsInViewRadius)
        {
            Transform target = col.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < m_viewAngle / 2f)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, m_obstructionMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
}