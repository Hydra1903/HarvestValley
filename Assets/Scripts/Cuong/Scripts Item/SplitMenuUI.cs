using UnityEngine;
using UnityEngine.UI;

public class SplitMenuUI : MonoBehaviour
{
    public static SplitMenuUI Instance;
    public GameObject panel;
    public Button splitButton;
    InventorySlotUI currentSlot;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        splitButton.onClick.AddListener(SplitItem);
    }

    public void Show(InventorySlotUI slot)
    {
        currentSlot = slot;
        panel.SetActive(true);
        panel.GetComponent<RectTransform>().position = Input.mousePosition + new Vector3(200,-100,0);
        Debug.Log(Input.mousePosition);
    }

    void SplitItem()
    {
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
