using UnityEngine;

public class PlantHoverOutline : MonoBehaviour
{
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false; // tắt viền khi khởi động
    }

    void OnMouseEnter()
    {
        if (outline != null)
            outline.enabled = true; // bật khi trỏ chuột vào
    }

    void OnMouseExit()
    {
        if (outline != null)
            outline.enabled = false; // tắt khi rời chuột ra
    }
}
