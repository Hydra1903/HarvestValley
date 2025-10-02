using UnityEngine;
using System.Collections;
public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public Camera cameraFPS;
    public Camera cameraTPS;
    public Animator animatorCameraTPS;
    public void SwitchToActionView()
    {
        cameraFPS.enabled = false;
        cameraTPS.enabled = true;
        cameraTPS.transform.position = cameraFPS.transform.position;
        cameraTPS.transform.rotation = cameraFPS.transform.rotation;
        animatorCameraTPS.Play("MoveOn");
    }

    public IEnumerator SwitchToMainView()
    {
        animatorCameraTPS.Play("MoveOff");
        yield return new WaitForSeconds(0.5f);
        OnAnimationEnd();
    }

    void OnAnimationEnd()
    {
        cameraFPS.enabled = true;
        cameraTPS.enabled = false;
    }
}
