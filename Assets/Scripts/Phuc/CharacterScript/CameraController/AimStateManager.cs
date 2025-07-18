using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering;
public enum SwitchCamera
{
    FirstCamera, 
    SecondCamera
}
public class AimStateManager : MonoBehaviour
{
    public Camera firstPersonCamera;       
    public Camera thirdPersonCamera;

    public GameObject firstPersonCamObj;   
    public GameObject thirdPersonCamObj;

    //cai nay la gameobject khung xuong va image cua nhan vat -> testing tat no di
    public GameObject Skinny;

    private CinemachineCamera firstCam;
    private CinemachineCamera thirdCam;
    public SwitchCamera currentState;
    private bool isFirstPerson = true;

    void Start()
    {
        firstCam = firstPersonCamObj.GetComponent<CinemachineCamera>();
        thirdCam = thirdPersonCamObj.GetComponent<CinemachineCamera>();
        currentState = SwitchCamera.FirstCamera;
        ActivateFirstPerson();
    }

    void Update()
    {

        HandleChange();
    }
    public void ChangeState(SwitchCamera newState)
    {
        currentState = newState;
    }
    //ham lay input V de chuyen doi camera
    void HandleChange()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isFirstPerson)
            {
                ActivateThirdPerson();
                ChangeState(SwitchCamera.SecondCamera);
            }
            else
            {
                ActivateFirstPerson();
                ChangeState(SwitchCamera.FirstCamera);
            }
        }
    }
    //chuyen doi sang goc nhin thu nhat
    void ActivateFirstPerson()
    {
        firstPersonCamObj.SetActive(true);
        thirdPersonCamObj.SetActive(false);

        Skinny.SetActive(false);      

        firstPersonCamera.targetDisplay = 0;
        thirdPersonCamera.targetDisplay = 1;
        firstCam.ForceCameraPosition(firstCam.Follow.position, firstCam.Follow.rotation);

        firstCam.enabled = false;
        firstCam.enabled = true;
        isFirstPerson = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    //Chuyen doi sang goc nhin 3 
    void ActivateThirdPerson()
    {
        thirdPersonCamObj.SetActive(true);
        firstPersonCamObj.SetActive(false);
        Skinny.SetActive(true);
        thirdPersonCamera.targetDisplay = 0;
        firstPersonCamera.targetDisplay = 1;

        thirdCam.ForceCameraPosition(thirdCam.Follow.position, thirdCam.Follow.rotation);
        thirdCam.enabled = false;
        thirdCam.enabled = true;

        isFirstPerson = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
