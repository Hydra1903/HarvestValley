using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEditor.Rendering.LookDev;
using NUnit.Framework.Interfaces;
using Unity.IO.LowLevel.Unsafe;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3;
    public float rotationSpeed = 700f;
    public bool IsGrounded;
    [HideInInspector]public Vector3 dir;
    public float hzInput;
    public float vInput;

    [SerializeField]float groundYOffSet;
    [SerializeField]public LayerMask groundMask;
    public Vector3 spherePos;
   [SerializeField] float gravity = -9.81f;
    public Vector3 velocity;
    public float jumpForce = 5f;
    public CharacterController controller;
    public bool isEndJump;
    public CinemachineOrbitalFollow orbitalTransposer;
    public Transform orientation;
    public Camera Camera;
    public Transform cameraTransform;
    private float xRotation = 0f; 
    public float mouseSensitivity = 100f;


    // KHỞI TẠO TRẠNG THÁI
    public MovementBaseState currentState;
    public Idle IdleState = new Idle();
    public Running RunState = new Running();
    public Walk WalkState = new Walk();
    public Fall FallState = new Fall();
    public AimStateManager aimManager;
    //public ChangeCamera ChangeCameraState= new ChangeCamera();
    //private Transform cameraTarget;
    //private Transform characterBody;

    public Animator animator;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        SwitchState(IdleState);
    }
    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    private void Update()
    {
        IsGrounded = isGrounded();
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        Gravity();
        Move(1f);
        Rotate();
        currentState.UpdateState(this);

        if (aimManager != null && aimManager.currentMode == CameraMode.FirstPerson)
        {
            FirstPersonLook();
        }
    }
    public void Move(float Speed)
    {
        //Vector3 forward = cameraTransform.forward;
        //Vector3 right = cameraTransform.right;

        //forward.y = 0f;
        //right.y = 0f;

        //forward.Normalize();
        //right.Normalize();

        //dir = (forward * vInput + right * hzInput).normalized * moveSpeed * Speed;
        //controller.Move(dir * Time.deltaTime);
        Vector3 forward, right;

        if (aimManager != null && aimManager.currentMode == CameraMode.FirstPerson)
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }
        else
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        // Đặt hướng di chuyển
        dir = (forward * vInput + right * hzInput).normalized * moveSpeed * Speed;

        // Sau khi có dir, mới gọi Move()
        controller.Move(dir * Time.deltaTime);
    }
    void Rotate()
    {
        if (aimManager != null && aimManager.currentMode == CameraMode.FirstPerson)
            return; // Góc nhìn thứ nhất đã xử lý bằng chuột rồi

        if (dir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
  
    void FirstPersonLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public bool isGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffSet, transform.position.z);
        if (Physics.CheckSphere(spherePos,controller.radius - 0.05f, groundMask)) return true;
        return false;
    }
    void Gravity()
    {
        if (!isGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2f;
        controller.Move(velocity * Time.deltaTime);
    }
    void OFFJump()
    {
        isEndJump = true;
    }
    void ONJump()
    {
        isEndJump = false;
    }
}