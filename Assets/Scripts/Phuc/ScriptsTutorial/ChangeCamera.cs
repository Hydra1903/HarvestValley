using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public Animator cameraAnimator; // animator �? play animation chuy?n
    public Camera cameraA; // camera ban �?u
    public Camera cameraB; // camera ��ch
    public float transitionTime = 2f; // th?i gian c?a animation

    private bool isSwitched = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !isSwitched)
        {
            StartCoroutine(SwitchCamera());
        }
    }

    System.Collections.IEnumerator SwitchCamera()
    {
        isSwitched = true;

        // Play animation
        cameraAnimator.Play("Anim1");

        // Ch? animation k?t th�c
        yield return new WaitForSeconds(transitionTime);

        // T?t camera A, b?t camera B
        cameraA.enabled = false;
        cameraB.enabled = true;
    }
}
