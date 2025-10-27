using UnityEngine;

public class PlantOutline : MonoBehaviour
{
    private Outline outline;
    private static PlantOutline currentHover; 

    [SerializeField] private Color myOutlineColor = Color.white;  
    [SerializeField, Range(0, 10)] private float defaultWidth = 0f; 

    private void Awake()
    {
        outline = GetComponentInChildren<Outline>();
        if (!outline) return;

        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = myOutlineColor;  
        outline.OutlineWidth = defaultWidth;    
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
