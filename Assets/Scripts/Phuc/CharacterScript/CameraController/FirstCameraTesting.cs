using UnityEngine;

public class FirstCameraTesting : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // G�n camera (FPS camera)
    private CharacterController controller;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // L?y input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Di chuy?n theo h�?ng camera (b? tr?c Y �? kh�ng �i l�n tr?i)
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * vertical + right * horizontal).normalized;

        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // �p d?ng gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleRotation()
    {
        // Xoay nh�n v?t theo h�?ng camera (tr?c Y)
        Vector3 lookDir = cameraTransform.forward;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
