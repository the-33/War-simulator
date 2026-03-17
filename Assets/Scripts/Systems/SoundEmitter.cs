using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static registry. Any script that makes noise calls SoundEmitter.Emit() to
/// notify every nearby HearingController.
/// </summary>
public static class SoundEmitter
{
    private static readonly List<HearingController> _registry = new();

    public static void Register(HearingController h)
    {
        if (h == null || _registry.Contains(h))
            return;

        _registry.Add(h);
    }

    public static void Unregister(HearingController h)
    {
        if (h == null)
            return;

        _registry.Remove(h);
    }

    /// <param name="position">World-space origin of the sound.</param>
    /// <param name="radius">Max distance the sound can travel.</param>
    public static void Emit(Vector3 position, float radius)
    {
        radius = Mathf.Max(0f, radius);

        for (int i = _registry.Count - 1; i >= 0; i--)
        {
            var h = _registry[i];
            if (h == null) { _registry.RemoveAt(i); continue; }

            float effectiveRange = Mathf.Max(radius, h.m_hearingRange);
            if (Vector3.Distance(position, h.transform.position) <= effectiveRange)
                h.HearSound(position);
        }
    }
}
