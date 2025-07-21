using UnityEngine;
using UnityEngine.UI;

public class SplitMenuUI : MonoBehaviour
{
    public static SplitMenuUI Instance;
    public GameObject panel;
    public Button splitButton;
    InventorySlotUI currentSlot;
    public Vector3 deviation;
    public GameObject backgroundBlocker;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        splitButton.onClick.AddListener(SplitItem);
    }

    public void Show(InventorySlotUI slot, RectTransform item)
    {
        currentSlot = slot;
        panel.SetActive(true);
        backgroundBlocker.SetActive(true);
        panel.GetComponent<RectTransform>().position = item.position + deviation;
    }

    void SplitItem()
    {
        backgroundBlocker.SetActive(false);
        var slotData = currentSlot.inventory.slots[currentSlot.row, currentSlot.column];
        if (slotData.item != null && slotData.item.quantity > 1)
        {
            int half = slotData.item.quantity / 2;

            currentSlot.inventory.slots[currentSlot.row, currentSlot.column].item.quantity -= half;

            currentSlot.inventory.AddItem2(slotData.item.itemData, half);

            currentSlot.inventoryUI.UpdateAllSlots();
        }

        panel.SetActive(false);
    }


    public void Hide()
    {
        panel.SetActive(false);
    }
}
