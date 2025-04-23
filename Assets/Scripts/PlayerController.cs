using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement properties")]
    public float m_terminalSpeed = 5f;
    public float m_acceleration = 5f;

    [Header("Testing properties")]
    public float m_mouseSensitivity = 1f;

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
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }

    private void FixedUpdate()
    {
        HandleMovement();
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
        if (m_rigidbody.linearVelocity.magnitude > m_terminalSpeed)
        {
            m_rigidbody.linearVelocity = Vector3.ClampMagnitude(m_rigidbody.linearVelocity, m_terminalSpeed);
        }

        if (m_lookInput.sqrMagnitude > 0.01f)
        {
            float mouseX = m_lookInput.x * m_mouseSensitivity;
            float mouseY = m_lookInput.y * m_mouseSensitivity;

            // Invertimos Y si hace falta
            m_xRotation -= mouseY;
            m_xRotation = Mathf.Clamp(m_xRotation, -90f, 90f);

            m_cameraTransform.localRotation = Quaternion.Euler(m_xRotation, 0f, 0f);
            m_playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
