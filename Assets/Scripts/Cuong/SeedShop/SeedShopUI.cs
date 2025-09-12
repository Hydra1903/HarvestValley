using UnityEngine;
using UnityEngine.UI;

public class SeedShopUI : MonoBehaviour
{
    public static SeedShopUI Instance;

    public SeedShop seedShop;
    public SeedShopItemUI[] seedShopItemUI;
    public ScrollRect scrollViewSeedShop;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void ResetUI()
    {
        scrollViewSeedShop.verticalNormalizedPosition = 1f;
        for (int i = 0; i < seedShop.amount.Length; i++)
        {
            seedShop.amount[i] = 1;
            seedShopItemUI[i].CalculatePrice();
            seedShopItemUI[i].UpdateUI();
            seedShopItemUI[i].UpdateUnlock();
        }
    }
}
