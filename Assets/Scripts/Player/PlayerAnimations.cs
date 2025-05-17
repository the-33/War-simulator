using StarterAssets;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.fire) _animator.SetBool("Shooting", true);
        else _animator.SetBool("Shooting", false);

        if (_input.aim) _animator.SetBool("Aiming", true);
        else _animator.SetBool("Aiming", false);

        if (_input.reload)
        {
            _animator.SetBool("Reload", true);
            _input.ReloadInput(false);
        }
        else
        {
            _animator.SetBool("Reload", false);
        }

            
        if (_input.move != Vector2.zero)
        {
            if (!_input.sprint)
            {
                _animator.SetBool("Walking", true);
                _animator.SetBool("Running", false);
            }
            else
            {
                _animator.SetBool("Walking", false);
                _animator.SetBool("Running", true);
            }
        }
        else
        {
            _animator.SetBool("Walking", false);
            _animator.SetBool("Running", false);
        }
    }
}
