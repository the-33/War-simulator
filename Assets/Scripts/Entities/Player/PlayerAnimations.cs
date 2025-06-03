using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimations : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private PlayerShooting _playerShooting;
    private PlayerHealth _health;
    private FirstPersonController _firstPersonController;
    private Animator _animator;

    public Transform ShellEjectionPoint;
    public GameObject ShellPrefab;
    public float shellEjectionForce;
    public ParticleSystem shootingParticles;

    public Image crosshair;

    public CinemachineCamera mainCamera;
    public Camera weaponCamera;

    public int normalFov = 50;
    public int aimingFov = 48;
    public float fovLerpSpeed = 10f;

    public float standingSize = 2f;
    public float crouchSize = 1f;
    public float sizeLerpSpeed = 10f;

    private float currentFov;

    public void shootAnimate()
    {
        shootingParticles.Play();
    }

    public void EjectShell()
    {
        var shell = Instantiate(ShellPrefab, ShellEjectionPoint.position, Quaternion.LookRotation(-ShellEjectionPoint.transform.right));
        shell.GetComponent<Rigidbody>().AddForce(ShellEjectionPoint.transform.forward * shellEjectionForce, ForceMode.VelocityChange);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _playerShooting = GetComponent<PlayerShooting>();
        _health = GetComponent<PlayerHealth>();
        _animator = GetComponent<Animator>();
        _firstPersonController = GetComponent<FirstPersonController>();

        crosshair.enabled = true;
        currentFov = normalFov;
        mainCamera.Lens.FieldOfView = currentFov;
        weaponCamera.fieldOfView = currentFov;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerShooting.shooting)
        {
            _animator.SetBool("Shooting", true);
        }
        else
        {
            _animator.SetBool("Shooting", false);
        }

        if (_playerShooting.aiming)
        {
            _animator.SetBool("Aiming", true);
        }
        else
        {
            _animator.SetBool("Aiming", false);
        }

        float targetFov = _playerShooting.aiming ? aimingFov : normalFov;
        currentFov = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * fovLerpSpeed);
        mainCamera.Lens.FieldOfView = currentFov;
        weaponCamera.fieldOfView = currentFov;

        if (_playerShooting.reloading) _animator.SetBool("Reload", true);
        else _animator.SetBool("Reload", false);

        float targetSize = (_input.crouch) ? crouchSize : standingSize;
        transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, targetSize, Time.deltaTime * sizeLerpSpeed);
            
        if (_input.move != Vector2.zero)
        {
            if (_input.sprint && !_playerShooting.aiming && !_health.healing && !_playerShooting.reloading && _firstPersonController.canRun)
            {
                _animator.SetBool("Walking", false);
                _animator.SetBool("Running", true);
            }
            else
            {
                _animator.SetBool("Walking", true);
                _animator.SetBool("Running", false);
            }
        }
        else
        {
            _animator.SetBool("Walking", false);
            _animator.SetBool("Running", false);
        }

        if (_health.healing)
        {
            _animator.SetBool("DoingAction", true);
        }
        else
        {
            _animator.SetBool("DoingAction", false);
        }

        if (_health.healing || _playerShooting.aiming) crosshair.enabled = false;
        else crosshair.enabled = true;
    }
}
