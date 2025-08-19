using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotBarSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    public HotBar hotbar;
    public HotBarUI hotbarUI;
    public int location;
    public void SetSlot(int lcn, HotBar hb, HotBarUI ui)
    {
        location = lcn;
        hotbar = hb;
        hotbarUI = ui;
    }

    public void UpdateSlotUI()
    {
        var slot = hotbar.slots[location];

        if (slot.IsEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = slot.item.itemData.icon;
            quantityText.text = slot.item.quantity > 0 ? slot.item.quantity.ToString() : "";
        }
    }
}
