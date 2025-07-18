using Unity.Cinemachine;
using UnityEngine;

public class FirstCameraTesting : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseAll = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        xRotation -= mouseAll.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localEulerAngles = Vector3.right * xRotation;
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseAll * mouseSensitivity);
        }
    }
}
