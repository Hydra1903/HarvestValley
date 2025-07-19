using UnityEngine;
using Unity.Cinemachine;

public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}

public class AimStateManager : MonoBehaviour
{
    public GameObject firstPersonCamObj;
    public GameObject thirdPersonCamObj;

    public GameObject skinnyModel;

    private CinemachineCamera firstCam;
    private CinemachineCamera thirdCam;

    public CameraMode currentMode = CameraMode.FirstPerson;

    [SerializeField] KeyCode switchKey = KeyCode.V;

    void Start()
    {
        firstCam = firstPersonCamObj.GetComponent<CinemachineCamera>();
        thirdCam = thirdPersonCamObj.GetComponent<CinemachineCamera>();

        ApplyCameraState(currentMode);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            currentMode = currentMode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
            ApplyCameraState(currentMode);
        }

    }

    void ApplyCameraState(CameraMode mode)
    {
        bool isFirst = mode == CameraMode.FirstPerson;

        firstPersonCamObj.SetActive(isFirst);
        thirdPersonCamObj.SetActive(!isFirst);

        if (skinnyModel != null)
            skinnyModel.SetActive(!isFirst);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
