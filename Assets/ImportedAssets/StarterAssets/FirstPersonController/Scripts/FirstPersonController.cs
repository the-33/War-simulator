using Interfaces;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        [Header("Peek Settings")]
        public float PeekSideOffset = 0.3f;
        public float PeekTiltAngle = 15f;
        public float PeekLerpSpeed = 10f;

        [Header("Fall Damage")]
        public float minFallHeight = 3f;
        public float fatalFallHeight = 10f;

        private Vector3 _peekTargetPosition;
        private Quaternion _peekTargetRotation;
        private Vector3 _initialCameraLocalPosition;
        private Quaternion _initialCameraLocalRotation;

        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private float _fallStartHeight = -1f;

        public bool canRun = true;
        public bool playerLocked;

        private PlayerShooting _shooting;
        private PlayerHealth _health;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private IDamageable _playerDamageable;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _initialCameraLocalPosition = CinemachineCameraTarget.transform.localPosition;
            _initialCameraLocalRotation = CinemachineCameraTarget.transform.localRotation;

            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _shooting = GetComponent<PlayerShooting>();
            _health = GetComponent<PlayerHealth>();
            _playerDamageable = GetComponent<IDamageable>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError("Starter Assets package is missing dependencies.");
#endif

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (playerLocked) return;

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
            HandlePeek();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void HandlePeek()
        {
            bool peekingRight = _input.peekRight && (!_input.sprint || (_input.sprint && _input.aim)) && !_shooting.reloading && !_health.healing;
            bool peekingLeft = _input.peekLeft && (!_input.sprint || (_input.sprint && _input.aim)) && !_shooting.reloading && !_health.healing;

            if (peekingRight)
            {
                _peekTargetPosition = _initialCameraLocalPosition + Vector3.right * PeekSideOffset;
                _peekTargetRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, -PeekTiltAngle);
            }
            else if (peekingLeft)
            {
                _peekTargetPosition = _initialCameraLocalPosition + Vector3.left * PeekSideOffset;
                _peekTargetRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, PeekTiltAngle);
            }
            else
            {
                _peekTargetPosition = _initialCameraLocalPosition;
                _peekTargetRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, 0f);
                _input.peekLeft = false;
                _input.peekRight = false;
            }

            CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(
                CinemachineCameraTarget.transform.localPosition,
                _peekTargetPosition,
                Time.deltaTime * PeekLerpSpeed
            );

            CinemachineCameraTarget.transform.localRotation = Quaternion.Lerp(
                CinemachineCameraTarget.transform.localRotation,
                _peekTargetRotation,
                Time.deltaTime * PeekLerpSpeed
            );
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(
                    _cinemachineTargetPitch, 0.0f,
                    CinemachineCameraTarget.transform.localRotation.eulerAngles.z
                );

                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = (_input.sprint && !_shooting.reloading && !_shooting.aiming && !_health.healing && canRun) ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                if (_fallStartHeight != -1f)
                {
                    float fallDistance = _fallStartHeight - transform.position.y;
                    if (fallDistance > minFallHeight)
                    {
                        FallDamage(fallDistance);
                    }
                    _fallStartHeight = -1f;
                }

                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                if (_fallStartHeight == -1f)
                {
                    _fallStartHeight = transform.position.y;
                }

                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void FallDamage(float fallDistance)
        {
            print(fallDistance);
            if (fallDistance >= fatalFallHeight)
            {
                Debug.Log("Daño letal por caída");
                _playerDamageable.TakeDamage(100);
            }
            else
            {
                float t = Mathf.InverseLerp(minFallHeight, fatalFallHeight, fallDistance);
                float damage = Mathf.Lerp(1f, 4f, t);
                int finalDamage = Mathf.RoundToInt(damage);

                Debug.Log($"Fall damage desde altura {fallDistance:F2}: {finalDamage}");
                _playerDamageable.TakeDamage(finalDamage);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = Grounded ? transparentGreen : transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
