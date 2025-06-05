using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MovementContext))]
[RequireComponent(typeof(WeaponEvents))]
public class EnemyAnimatorController : MonoBehaviour, IAnimator
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private MovementContext _movementContext;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _movementContext = GetComponent<MovementContext>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void SetShoot(bool state)
    {
        _animator.SetBool("Shoot", state);
    }

    private void Update()
    {
        if (_animator == null) return;

        var animatorSpeed = NormalizeWithMiddle(_agent.velocity.magnitude, 0f, _movementContext.m_normalSpeed, _movementContext.m_quickSpeed);
        _animator.SetFloat("Speed", animatorSpeed);
    }

    float NormalizeWithMiddle(float value, float min, float med, float max)
    {
        if (value <= med)
        {
            return Mathf.InverseLerp(min, med, value) * 0.5f;
        }
        else
        {
            return 0.5f + Mathf.InverseLerp(med, max, value) * 0.5f;
        }
    }

    public void TriggerDeath()
    {
        _agent.enabled = false;
        _capsuleCollider.enabled = false;
        _animator.SetTrigger("Death");

        Destroy(gameObject, 10f);
    }

    public void TriggerHit()
    {
        _animator.SetTrigger("Hit");
    }
}