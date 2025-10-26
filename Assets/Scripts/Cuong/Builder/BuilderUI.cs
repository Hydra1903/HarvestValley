using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BuilderUI : MonoBehaviour
{
    public static BuilderUI Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public Button UpdateBarnLv2;
    public Button UpdateBarnLv3;
    public Button UpdateHomeLv2;
    public Button UnlockFarmland2;
    public Button UnlockFarmland3;
    public Button UnlockGrassland;
    public Button UnlockPen1;
    public Button UnlockPen2;
    public Button UpdatePen1Lv2;
    public Button UpdatePen2Lv2;
    public Button UnlockGreenhouse1;
    public Button UnlockGreenhouse2;

    public TextMeshProUGUI textUpdateBarnLv2;
    public TextMeshProUGUI textUpdateBarnLv3;
    public TextMeshProUGUI textUpdateHomeLv2;
    public TextMeshProUGUI textUnlockFarmland2;
    public TextMeshProUGUI textUnlockFarmland3;
    public TextMeshProUGUI textUnlockGrassland;
    public TextMeshProUGUI textUnlockPen1;
    public TextMeshProUGUI textUnlockPen2;
    public TextMeshProUGUI textUpdatePen1Lv2;
    public TextMeshProUGUI textUpdatePen2Lv2;
    public TextMeshProUGUI textUnlockGreenhouse1;
    public TextMeshProUGUI textUnlockGreenhouse2;

    public Button Yes;
    public Button No;
    public GameObject panelConfirm;
    private System.Action onYes;

    public TextMeshProUGUI gold;
    public ScrollRect scrollViewBuilder;
    public void OnPanelConfirm(System.Action callback)
    {
        panelConfirm.SetActive(true);

        Yes.onClick.RemoveAllListeners();
        No.onClick.RemoveAllListeners();

        onYes = callback;
        Yes.onClick.AddListener(() =>
        {
            onYes?.Invoke();
            panelConfirm.SetActive(false);
            UpdateUI();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        });

        No.onClick.AddListener(() =>
        {
            panelConfirm.SetActive(false);
        });
    }
    void Start()
    {
        //LoadUI();
    }
    public void LoadUI()
    {
        if (Builder.Instance.isBuilding[0])
        {
            UpdateButton_UpdateBarnLv2();
        }
        if (Builder.Instance.isBuilding[1])
        {
            UpdateButton_UpdateBarnLv3();
        }
        if (Builder.Instance.isBuilding[2])
        {
            UpdateButton_UpdateHomeLv2();
        }
        if (Builder.Instance.isBuilding[3])
        {
            UpdateButton_UnlockFarmland2();
        }
        if (Builder.Instance.isBuilding[4])
        {
            UpdateButton_UnlockFarmland3();
        }
        if (Builder.Instance.isBuilding[5])
        {
            UpdateButton_UnlockGrassland();
        }
        if (Builder.Instance.isBuilding[6])
        {
            UpdateButton_UnlockPen1();
        }
        if (Builder.Instance.isBuilding[7])
        {
            UpdateButton_UpdatePen1Lv2();
        }
        if (Builder.Instance.isBuilding[8])
        {
            UpdateButton_UnlockPen2();
        }
        if (Builder.Instance.isBuilding[9])
        {
            UpdateButton_UpdatePen2Lv2();
        }
        if (Builder.Instance.isBuilding[10])
        {
            UpdateButton_UnlockGreenhouse1();
        }
        if (Builder.Instance.isBuilding[11])
        {
            UpdateButton_UnlockGreenhouse2();
        }

    }
    public void UpdateUI()
    {
        gold.text = Gold.Instance.gold.ToString("N0", new CultureInfo("de-DE"));
    }
    public void ResetUI()
    {
        scrollViewBuilder.verticalNormalizedPosition = 1f;
        panelConfirm.SetActive(false);
    }
    public void UpdateButton_UpdateBarnLv2()
    {
        UpdateBarnLv2.interactable = false;
        textUpdateBarnLv2.text = "Đã Nâng Cấp";
    }
    public void UpdateButton_UpdateBarnLv3()
    {
        UpdateBarnLv3.interactable = false;
        textUpdateBarnLv3.text = "Đã Nâng Cấp";
    }
    public void UpdateButton_UpdateHomeLv2()
    {
        UpdateHomeLv2.interactable = false;
        textUpdateHomeLv2.text = "Đã Nâng Cấp";
    }
    public void UpdateButton_UnlockFarmland2()
    {
        UnlockFarmland2.interactable = false;
        textUnlockFarmland2.text = "Đã Mở Khóa";
    }
    public void UpdateButton_UnlockFarmland3()
    {
        UnlockFarmland3.interactable = false;
        textUnlockFarmland3.text = "Đã Mở Khóa";
    }
    public void UpdateButton_UnlockGrassland()
    {
        UnlockGrassland.interactable = false;
        textUnlockGrassland.text = "Đã Mở Khóa";
    }
    public void UpdateButton_UnlockPen1()
    {
        UnlockPen1.interactable = false;
        textUnlockPen1.text = "Đã Mở Khóa";
    }
    public void UpdateButton_UnlockPen2()
    {
        UnlockPen2.interactable = false;
        textUnlockPen2.text = "Đã Mở Khóa";
    }
    public void UpdateButton_UpdatePen1Lv2()
    {
        UpdatePen1Lv2.interactable = false;
        textUpdatePen1Lv2.text = "Đã Nâng Cấp";
    }
    public void UpdateButton_UpdatePen2Lv2()
    {
        UpdatePen2Lv2.interactable = false;
        textUpdatePen2Lv2.text = "Đã Nâng Cấp";
    }
    public void UpdateButton_UnlockGreenhouse1()
    {
        UnlockGreenhouse1.interactable = false;
        textUnlockGreenhouse1.text = "Đã Mở Khóa";
    }
    public void UpdateButton_UnlockGreenhouse2()
    {
        UnlockGreenhouse2.interactable = false;
        textUnlockGreenhouse2.text = "Đã Mở Khóa";
    }
}
