using Unity.Cinemachine;
using UnityEngine;

public class TestingSwitchCamera : MonoBehaviour
{
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;

    private bool isFirstPerson = true;

    void Start()
    {
        SetCamera(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
            SetCamera(isFirstPerson);
        }
    }

    void SetCamera(bool firstPerson)
    {
        if (firstPerson)
        {
            firstPersonCam.Priority = 10;
            thirdPersonCam.Priority = 0;
        }
        else
        {
            firstPersonCam.Priority = 0;
            thirdPersonCam.Priority = 10;
        }
    }
}
