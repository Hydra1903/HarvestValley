using UnityEngine;
using UnityEngine.UI;

public class CharacterStateMachine : MonoBehaviour
{
    public static CharacterStateMachine Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public ECharacter currentCharacter;
    [HideInInspector] public ICharacterState currentState;
    public Animator animator;
    public Transform transformCharacter;
    public CharacterController characterController;
    public Character character = new Character("Cuong",4,6);
    [HideInInspector] public IdleState idleState = new IdleState();
    [HideInInspector] public WalkState walkState = new WalkState();
    [HideInInspector] public RunState runState = new RunState();
    [HideInInspector] public HoeState hoeState = new HoeState();
    [HideInInspector] public DigHoleState digHoleState = new DigHoleState();
    [HideInInspector] public HarvestLowState harvestLowState = new HarvestLowState();
    [HideInInspector] public HarvestHighState harvestHighState = new HarvestHighState();
    [HideInInspector] public WateringState wateringState = new WateringState();
    [HideInInspector] public MowingState mowingState = new MowingState();
    public float horizontal;
    public float vertical;
    public float mouseX;
    public float mouseY;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;

    public float mouseSensitivity;
    public Transform characterBody;
    public Transform cameraCharacter;
    float xRotation = 0f;

    public MainUIScreen mainUIScreen;

    public PlantManager plantManager;
    public SoilManager soilManager;
    public FarmInput farmInput;
    public void ChangeState(ICharacterState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;

        if (currentState != null)
            currentState.Enter(this);
        ToolCharacter.Instance.SetToolCharacter();
    }
    void Start()
    {
        currentState = idleState;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (UIStateMachine.Instance.currentState == UIStateMachine.Instance.mainScreenState)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }
        else
        {
            horizontal = 0; vertical = 0; mouseX = 0; mouseY = 0;
        }

        if (currentState != null)
            currentState.Update(this);

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void PlayerMovement(float speed)
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    public void CameraController()
    {
        characterBody.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        cameraCharacter.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    public void ExitState()
    {
        ChangeState(idleState);
    }  
}
