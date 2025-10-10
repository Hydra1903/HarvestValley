using UnityEngine;

public class CursorController : MonoBehaviour
{
    private bool isCursorVisible = false;

    void Start()
    {
        // Ban đầu, ẩn và khóa con trỏ chuột khi vào game
        HideCursor();
    }

    void Update()
    {
        // Ví dụ: Nhấn phím Escape để Hiện/Ẩn con trỏ chuột
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorVisible = !isCursorVisible;
            if (isCursorVisible)
            {
                ShowCursor();
            }
            else
            {
                HideCursor();
            }
        }
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}