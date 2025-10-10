using UnityEngine;
using Unity.Cinemachine;

public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}

public class AimStateManager : MonoBehaviour
{
    [Header("Camera Object")]
    public GameObject firstPersonCamObj;
    public GameObject thirdPersonCamObj;

    [Header("Avatar and GameObject Player")]
    public GameObject skinnyModel;
    public GameObject playerObject;

    [Header("Switch Camera")]
    [SerializeField] KeyCode switchKey = KeyCode.V;
    public CameraMode currentMode = CameraMode.FirstPerson;

    void Start()
    {
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

        if (playerObject != null)
        {
            var firstPersonScript = playerObject.GetComponentInChildren<FirstCameraTesting>();
            if (firstPersonScript != null) firstPersonScript.enabled = isFirst;
            var thirdPersonScript = playerObject.GetComponentInChildren<PlayerController>();
            if (thirdPersonScript != null) thirdPersonScript.enabled = !isFirst;
        }
        if (skinnyModel != null)
            skinnyModel.SetActive(!isFirst);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
