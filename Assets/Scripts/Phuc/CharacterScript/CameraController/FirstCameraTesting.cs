using UnityEngine;

public class FirstCameraTesting : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public CharacterController controller;

    [Header("Movement Settings")]
    public float walkSpeed = 3.7f;
    public float sprintSpeed = 9f;
    public float gravity = -9.81f;
    private Vector3 velocity;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;
    public bool allowMouseLook = true;

    void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
        if (cameraTransform == null && Camera.main != null) 
        { 
            cameraTransform = Camera.main.transform; 
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (allowMouseLook)
        {
            HandleMouseLook();
        }
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * vertical + right * horizontal).normalized;
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
