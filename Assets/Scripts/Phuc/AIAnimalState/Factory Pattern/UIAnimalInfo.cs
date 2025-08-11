using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimalInfo : MonoBehaviour
{
    public static UIAnimalInfo Instance;

    [Header("UI Elements")]
    public Text nameText;
    public Text statusText;
    public Text productText;
    public Text harvestText;
    public GameObject panel;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void SetInfo(string name, string status, string product, bool canHarvest)
    {
        nameText.text = name;
        statusText.text = status;
        productText.text = product;
        harvestText.text = canHarvest ? "<color=green>�? s?n s�ng thu ho?ch</color>" : "<color=red>Ch�a th? thu ho?ch</color>";
        panel.SetActive(true);
    }
}
