using UnityEngine;

/// <summary>
/// Attach to the Player. Emits periodic footstep sounds based on movement speed
/// so nearby enemies can react to them.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FootstepSoundEmitter : MonoBehaviour
{
    [Header("Walk")]
    [SerializeField] private float m_walkSoundRadius  = 8f;
    [SerializeField] private float m_walkInterval     = 0.55f;

    [Header("Sprint")]
    [SerializeField] private float m_sprintSoundRadius = 15f;
    [SerializeField] private float m_sprintInterval    = 0.35f;

    [Header("Thresholds")]
    [SerializeField] private float m_moveMinSpeed    = 0.5f;
    [SerializeField] private float m_sprintMinSpeed  = 5.0f;

    private CharacterController _cc;
    private float _timer;

    private void Awake() => _cc = GetComponent<CharacterController>();

    private void Update()
    {
        float horizontalSpeed = new Vector3(_cc.velocity.x, 0f, _cc.velocity.z).magnitude;

        if (horizontalSpeed < m_moveMinSpeed)
        {
            _timer = 0f;
            return;
        }

        bool sprinting = horizontalSpeed >= m_sprintMinSpeed;
        float interval = sprinting ? m_sprintInterval : m_walkInterval;
        float radius   = sprinting ? m_sprintSoundRadius : m_walkSoundRadius;

        _timer += Time.deltaTime;
        if (_timer >= interval)
        {
            _timer = 0f;
            SoundEmitter.Emit(transform.position, radius);
        }
    }
}
