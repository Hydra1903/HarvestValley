using UnityEngine;

public class SoilOutline : MonoBehaviour
{
    private Outline outline;

    [SerializeField] private Color outlineColor;
    [SerializeField, Range(0, 10)] private float defaultWidth = 0f;

    private void Awake()
    {
        outline = GetComponentInChildren<Outline>();

        // Nếu chưa có Outline component, tự động thêm vào
        if (!outline)
        {
            var renderer = GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                outline = renderer.gameObject.AddComponent<Outline>();
            }
        }

        if (outline)
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = defaultWidth; 
        }
    }
    public void EnableOutline(float width)
    {
        if (outline)
            outline.OutlineWidth = width;
    }
    public void DisableOutline()
    {
        if (outline)
            outline.OutlineWidth = 0f;
    }
    public Outline GetOutline()
    {
        return outline;
    }
}