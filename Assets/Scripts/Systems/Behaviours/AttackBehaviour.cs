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
            _movementContext.ResetPath(); ;
        }
        _animator.SetShoot(true);
    }

    public void Exit()
    {
        _attackTimer = 0f;
        _movementContext.ResetPath();
        _target = null;
        _animator.SetShoot(false);
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
    }
}