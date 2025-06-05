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

    private void Awake()
    {
        _movementContext = GetComponent<IMovementContext>();
        if (_movementContext == null)
            Debug.LogError("IMovementContext not found on " + gameObject.name);
        _animator = GetComponent<IAnimator>();
    }


    public void Enter(object context = null)
    {
        _attackTimer = 0f;
        if (context is Transform)
        {
            _target = (Transform)context;
            _movementContext.StopMoving(); ;
        }
        _animator.SetShoot(true);
    }

    public void Exit()
    {
        _attackTimer = 0f;
        _movementContext.StartMoving();
        _target = null;
        _animator.SetShoot(false);
        m_weaponController.m_firePoint.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void Tick()
    {
        if (_attackTimer < m_attackTime)
        {
            _attackTimer += Time.deltaTime;
            return; // Wait for the attack cooldown
        }
        _attackTimer = 0f; // Reset the attack timer

        //Handle the attack logic here

        AimAtTarget(transform, m_weaponController.m_firePoint, _target.transform);
        Vector3 target = _target.transform.position + new Vector3(0, 1f, 0);
        m_weaponController.m_firePoint.transform.LookAt(target);
    }


    void AimAtTarget(Transform parentTransform, Transform cannonTransform, Transform target)
    {
        parentTransform.LookAt(target.position);
    }
}