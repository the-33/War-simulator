using System;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    private Vector3[] _waypoints;
    private bool _isInitialized = false;

    public Vector3[] Waypoints
    {
        get
        {
            if (!_isInitialized)
                Initialize();

            return _waypoints;
        }
    }

    private void Initialize()
    {
        int childCount = transform.childCount;
        _waypoints = new Vector3[childCount];

        // Guardar posiciones y destruir hijos en un solo bucle
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            _waypoints[i] = child.position;
            Destroy(child.gameObject);
        }

        _isInitialized = true;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (_waypoints != null)
        {
            for (int i = 0; i < _waypoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_waypoints[i], 0.5f);
                if (i < _waypoints.Length - 1)
                {
                    Gizmos.DrawLine(_waypoints[i], _waypoints[i + 1]);
                }
                else
                {
                    Gizmos.DrawLine(_waypoints[i], _waypoints[0]);
                }
            }
        }
    }
#endif
}
