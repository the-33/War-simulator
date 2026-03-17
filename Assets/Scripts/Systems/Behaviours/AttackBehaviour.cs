using UnityEngine.AI;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour, IBehaviour
{
    public bool IsFinished => false;

    public float m_attackTime = 1f; // Time to wait before attacking again
    private float _attackTimer = 0f; // Timer to track attack cooldown

    private IAnimator _animator;
    private IMovementContext _movementContext;

    private Transform _target;

    public WeaponController m_weaponController;

    public float m_innaccuracyRange = 6f; // Range for random inaccuracy

    private void Awake()
    {
        _movementContext = GetComponent<IMovementContext>();
        if (_movementContext == null)
            Debug.LogError("IMovementContext not found on " + gameObject.name);
        _animator = GetComponent<IAnimator>();
    }


    public void Enter(Transform target)
    {
        _attackTimer = 0f;
        _target = target;

        if (_target == null)
        {
            _animator.SetShoot(false);
            return;
        }

        _movementContext.StopMoving();
        _animator.SetShoot(true);
    }

    public void Exit()
    {
        _attackTimer = 0f;
        _movementContext.StartMoving();
        _target = null;
        _animator.SetShoot(false);
        if (m_weaponController != null && m_weaponController.m_firePoint != null)
        {
            m_weaponController.m_firePoint.transform.rotation = Quaternion.identity;
        }
    }

    public void Tick()
    {
        if (_target == null || m_weaponController == null || m_weaponController.m_firePoint == null)
        {
            return;
        }

        Vector3 target = _target.transform.position + new Vector3(0, 1f, 0);
        m_weaponController.m_firePoint.transform.LookAt(target);
        // Add a small random offset to the aim to simulate inaccuracy
        Vector3 randomOffset = new Vector3(Random.Range(-m_innaccuracyRange, m_innaccuracyRange), Random.Range(-m_innaccuracyRange, m_innaccuracyRange), Random.Range(-m_innaccuracyRange, m_innaccuracyRange));
        m_weaponController.m_firePoint.transform.rotation *= Quaternion.Euler(randomOffset);

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f; // Elimina la componente vertical

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 120f);
    }
}