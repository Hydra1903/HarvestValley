using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarnUI2 : MonoBehaviour
{
    public Barn2 barn;
    public RectTransform slotsParent; // parent ch?a các BarnSlot UI
    public GameObject barnSlotPrefab; // prefab slot (gi?ng c?p 1)
    public TextMeshProUGUI[] hayTexts; // m?ng hi?n th? s? c? m?i slot

    private void Start()
    {
        CreateSlotsUI();
        UpdateUI();
    }

    public void CreateSlotsUI()
    {
        for (int i = 0; i < barn.columns; i++)
        {
            GameObject slot = Instantiate(barnSlotPrefab, slotsParent);
            barn.slots[i].slotObj = slot;

            TextMeshProUGUI countText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (hayTexts == null || hayTexts.Length != barn.columns)
                hayTexts = new TextMeshProUGUI[barn.columns];
            hayTexts[i] = countText;
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < barn.columns; i++)
        {
            if (hayTexts[i] != null)
                hayTexts[i].text = barn.GetHayAmount(i) + "/" + barn.limitCapacity;
        }
    }
}
