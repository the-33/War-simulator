using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimations : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private PlayerShooting _playerShooting;
    private Animator _animator;

    public ParticleSystem shellEjection;
    public ParticleSystem shootingParticles;

    public Image crosshair;

    public CinemachineVirtualCamera mainCamera;
    public Camera weaponCamera;

    public int normalFov = 50;
    public int aimingFov = 48;
    public float fovLerpSpeed = 10f;

    private float currentFov;

    public void shoot()
    {
        shootingParticles.Play();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _playerShooting = GetComponent<PlayerShooting>();
        _animator = GetComponent<Animator>();

        crosshair.enabled = true;
        currentFov = normalFov;
        mainCamera.m_Lens.FieldOfView = currentFov;
        weaponCamera.fieldOfView = currentFov;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerShooting.shooting)
        {
            _animator.SetBool("Shooting", true);
            shellEjection.Play();
        }
        else
        {
            _animator.SetBool("Shooting", false);
            shellEjection.Stop();
        }

        if (_playerShooting.aiming)
        {
            _animator.SetBool("Aiming", true);
            crosshair.enabled = false;
        }
        else
        {
            _animator.SetBool("Aiming", false);
            crosshair.enabled = true;
        }

        float targetFov = _playerShooting.aiming ? aimingFov : normalFov;
        currentFov = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * fovLerpSpeed);
        mainCamera.m_Lens.FieldOfView = currentFov;
        weaponCamera.fieldOfView = currentFov;

        if (_playerShooting.reloading) _animator.SetBool("Reload", true);
        else _animator.SetBool("Reload", false);

            
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
