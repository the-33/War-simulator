using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement properties")]
    public float m_runSpeed = 10f;
    public float m_walkSpeed = 5f;
    public float m_acceleration = 5f;
    [Tooltip("Wont update until it's reinstantiated")]
    public float m_drag = 5f;

    [Header("Testing properties")]
    public float m_mouseSensitivity = 1f;

    [Header("Interaction")]
    public float m_interactionDistance = 5f;

    [Header("References")]
    public Transform m_playerBody;
    public Transform m_cameraTransform;
    private PlayerInput m_playerInput;
    private Rigidbody m_rigidbody;

    //Input values
    private Vector2 m_movementInput;
    private Vector2 m_lookInput;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_playerInput = GetComponent<PlayerInput>();

        //Apply properties
        m_rigidbody.freezeRotation = true;
        m_rigidbody.linearDamping = m_drag;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleCameraRotation();
        CastInteractionRay();
    }

    private void HandleInputs()
    {
        m_movementInput = m_playerInput.actions["Move"].ReadValue<Vector2>();
        m_lookInput = m_playerInput.actions["Look"].ReadValue<Vector2>();
    }

    private float m_xRotation = 0f;
    private void HandleMovement()
    {
        // Get the input direction
        Vector3 forwardForce = m_playerBody.forward * m_acceleration;
        Vector3 rightForce = m_playerBody.right * m_acceleration;
        Vector3 force = forwardForce * m_movementInput.y + rightForce * m_movementInput.x;
        m_rigidbody.AddForce(force, ForceMode.Acceleration);

        // Clamp the velocity to the terminal speed
        if (m_rigidbody.linearVelocity.magnitude > m_walkSpeed)
        {
            m_rigidbody.linearVelocity = Vector3.ClampMagnitude(m_rigidbody.linearVelocity, m_walkSpeed);
        }
    }

    private void HandleCameraRotation()
    {
        // Handle camera rotation
        if (m_lookInput.sqrMagnitude > 0.01f)
        {
            float mouseX = m_lookInput.x * m_mouseSensitivity;
            float mouseY = m_lookInput.y * m_mouseSensitivity;

            m_xRotation -= mouseY;
            m_xRotation = Mathf.Clamp(m_xRotation, -90f, 90f);

            m_cameraTransform.localRotation = Quaternion.Euler(m_xRotation, 0f, 0f);
            m_playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    private void CastInteractionRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_cameraTransform.position, m_cameraTransform.forward, out hit, m_interactionDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
    }
}
