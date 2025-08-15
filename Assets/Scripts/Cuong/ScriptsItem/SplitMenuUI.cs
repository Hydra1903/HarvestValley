using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class SplitMenuUI : MonoBehaviour
{
    public static SplitMenuUI Instance;
    public GameObject panel;
    InventorySlotUI currentSlot;
    public Vector3 deviation;

    public GameObject backgroundBlocker;
    public TMP_InputField inputfieldQuantity;
    public int splitQuantity;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(InventorySlotUI slot, RectTransform item)
    {
        currentSlot = slot;
        panel.SetActive(true);
        backgroundBlocker.SetActive(true);
        panel.GetComponent<RectTransform>().position = item.position + deviation;
    }

    public void SplitInHalf()
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
        inputfieldQuantity.text = "1";
        panel.SetActive(false);
    }
    public void TakePartialItem()
    {
        backgroundBlocker.SetActive(false);

        splitQuantity = int.Parse(inputfieldQuantity.text);
        var slotData = currentSlot.inventory.slots[currentSlot.row, currentSlot.column];
        if (splitQuantity < currentSlot.inventory.slots[currentSlot.row, currentSlot.column].item.quantity)
        {           
            if (slotData.item != null && slotData.item.quantity > 1)
            {
                currentSlot.inventory.slots[currentSlot.row, currentSlot.column].item.quantity -= splitQuantity;

                currentSlot.inventory.AddItem2(slotData.item.itemData, splitQuantity);

                currentSlot.inventoryUI.UpdateAllSlots();
            }
        }
        else
        {
            Notification.Instance.ShowNotification("Chia quá giới hạn");
        }
        inputfieldQuantity.text = "1";
        panel.SetActive(false);
    }
    public void Hide()
    {
        panel.SetActive(false);
    }

    public void Plus()
    {
        splitQuantity = int.Parse(inputfieldQuantity.text);
        if (splitQuantity < 99)
        {
            splitQuantity++;
            inputfieldQuantity.text = splitQuantity.ToString();
        }
    }
    public void Minus()
    {
        splitQuantity = int.Parse(inputfieldQuantity.text);
        if (splitQuantity > 1)
        {
            splitQuantity--;
            inputfieldQuantity.text = splitQuantity.ToString();
        }
    }


}
