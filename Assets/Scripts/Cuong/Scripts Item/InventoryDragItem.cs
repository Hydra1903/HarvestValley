using UnityEngine;
using UnityEngine.UI;

public class InventoryDragItem : MonoBehaviour
{
    public Image icon;

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.enabled = true;
    }

    public void Hide()
    {
        icon.enabled = false;
    }

    void Update()
    {
        if (icon.enabled)
            transform.position = Input.mousePosition;
    }
}
