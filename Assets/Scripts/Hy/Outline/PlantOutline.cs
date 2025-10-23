using UnityEngine;

public class PlantOutline : MonoBehaviour
{
    private Outline outline;
    private static PlantOutline currentHover; // cây hiện tại đang được trỏ

    [SerializeField] private Color myOutlineColor = Color.white;  // <— bạn tự gán
    [SerializeField, Range(0, 10)] private float defaultWidth = 0f; // nên để 0 để ẩn lúc spawn


    private void Awake()
    {
        outline = GetComponentInChildren<Outline>();
        if (!outline) return;

        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = myOutlineColor;  // gán màu 1 lần lúc spawn
        outline.OutlineWidth = defaultWidth;    // 0 = ẩn, FarmInput sẽ bật khi hover
    }

    private void OnMouseEnter()
    {
        if (currentHover && currentHover != this)
            currentHover.DisableOutline();

        currentHover = this;
        EnableOutline();
    }

    private void OnMouseExit()
    {
        if (currentHover == this)
        {
            DisableOutline();
            currentHover = null;
        }
    }

    private void EnableOutline()
    {
        if (outline)
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 8f;
        }
    }

    private void DisableOutline()
    {
        if (outline)
            outline.OutlineWidth = 0f;
    }


}
