using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveStockSeller : MonoBehaviour
{
    public GameObject buyCanvas;
    public GameObject confirmPanel;
    public GameObject selectPenPanel;
    public TMP_Text pen1CountText;
    public TMP_Text pen2CountText;

    private bool playerInRange = false;
    private AnimalType selectedType = AnimalType.None;
    private AnimalPen selectedPen = null;

    [Header("UI Button Access")]
    public Button WhiteSheepButton;
    public Button BlackSheepButton;
    public Button CreamSheepButton;
    public Button WhiteGoatButton;
    public Button BlackGoatButton;
    public Button yesButton;
    public Button noButton;
    public Button pen2Button;
    public Button pen1Button;
    public GameObject pen1LockPanel;
    public GameObject pen2LockPanel;

    [Header("Spawn Point And Moving Random Point")]
    public AnimalPen pen1;
    public AnimalPen pen2;

    [System.Serializable]
    public class AnimalLevelRequirement
    {
        public AnimalType animalType;
        public int requiredLevel;
        [Header("Giá mua bằng vàng")]
        public int price;

        [Header("UI Overlay (Optional)")]
        public GameObject lockOverlay;
        public Button buyButton;
    }

    [Header("Animal Level Requirements")]
    public List<AnimalLevelRequirement> animalLevelRequirements = new List<AnimalLevelRequirement>();

    private void Start()
    {
        buyCanvas.SetActive(false);
        confirmPanel.SetActive(false);
        selectPenPanel.SetActive(false);

        yesButton.onClick.AddListener(ConfirmPurchase);
        noButton.onClick.AddListener(BackToBuyMenu);

        pen1Button.onClick.AddListener(() => SelectPen(pen1));
        pen2Button.onClick.AddListener(() => SelectPen(pen2));

        WhiteGoatButton.onClick.AddListener(() => ShowSelectPen(AnimalType.WhiteGoat));
        BlackGoatButton.onClick.AddListener(() => ShowSelectPen(AnimalType.BlackGoat));
        WhiteSheepButton.onClick.AddListener(() => ShowSelectPen(AnimalType.WhiteSheep));
        CreamSheepButton.onClick.AddListener(() => ShowSelectPen(AnimalType.CreamSheep));
        BlackSheepButton.onClick.AddListener(() => ShowSelectPen(AnimalType.BlackSheep));

        UpdateAnimalButtons();
    }

    void ShowSelectPen(AnimalType type)
    {
        selectedType = type;
        selectPenPanel.SetActive(true);
        UpdatePenCountsUI();
    }

    void SelectPen(AnimalPen pen)
    {
        selectedPen = pen;
        selectPenPanel.SetActive(false);
        confirmPanel.SetActive(true);
    }

    void ConfirmPurchase()
    {
        if (selectedPen == null || selectedType == AnimalType.None)
            return;

        int requiredLevel = GetRequiredLevel(selectedType);
        int price = GetAnimalPrice(selectedType);
        int currentLevel = LevelManager.Instance.currentLevel;

        if (currentLevel < requiredLevel)
        {
            Notification.Instance.ShowNotification("Cấp độ chưa đủ để mua!");
            confirmPanel.SetActive(false);
            return;
        }

        if (Gold.Instance.gold < price)
        {
            Notification.Instance.ShowNotification($"Không đủ vàng! Cần vàng để mua");
            confirmPanel.SetActive(false);
            return;
        }

        if (!selectedPen.CanSpawnMore())
        {
            Notification.Instance.ShowNotification("Chuồng nuôi đã đầy!");
            confirmPanel.SetActive(false);
            return;
        }

        GameObject prefab = AnimalFactory.GetPrefab(selectedType);
        if (prefab == null)
            return;

        bool spent = Gold.Instance.SpendGold(price);
        if (!spent)
        {
            Notification.Instance.ShowNotification("Không đủ vàng để mua!");
            return;
        }

        GameObject obj = Instantiate(prefab, selectedPen.GetRandomSpawnPosition(), Quaternion.identity);

        // Gán wander points cho AI
        SimpleAI ai = obj.GetComponent<SimpleAI>();
        if (ai != null)
            ai.wanderPoints = selectedPen.wanderPoints;

        // Gán barn reference cho AnimalFedding
        AnimalFedding feeding = obj.GetComponent<AnimalFedding>();
        if (feeding != null)
            feeding.barn = selectedPen.barnReference;

        // Gán panel UI cho AnimalInfo
        AnimalInfo info = obj.GetComponent<AnimalInfo>();
        if (info != null)
        {
            var panel = InfoPanelManager.instance.GetPanel(selectedPen.penId);
            if (panel != null)
                info.panelUI = panel;
        }

        AnimalData data = info?.data;
        bool success = selectedPen.RegisterAnimal(obj, data);

        string displayName = data != null
            ? AnimalHelpers.GetDisplayNameFromData(data.animalType, data.variant)
            : AnimalLocalization.GetLocalizedName(selectedType);

        if (success)
            Notification.Instance.ShowNotification($"Đã mua {displayName} với giá {price} vàng!");
        else
            Notification.Instance.ShowNotification($"Không thể thêm {displayName} vào chuồng!");

        confirmPanel.SetActive(false);
        selectedType = AnimalType.None;
        selectedPen = null;
        CloseAllUI();
        UpdatePenCountsUI();
    }

    int GetRequiredLevel(AnimalType type)
    {
        foreach (var req in animalLevelRequirements)
            if (req.animalType == type)
                return req.requiredLevel;
        return 1;
    }

    int GetAnimalPrice(AnimalType type)
    {
        foreach (var req in animalLevelRequirements)
            if (req.animalType == type)
                return req.price;
        return 0;
    }

    void UpdateAnimalButtons()
    {
        int currentLevel = LevelManager.Instance.currentLevel;

        foreach (var req in animalLevelRequirements)
        {
            bool unlocked = currentLevel >= req.requiredLevel;

            if (req.buyButton != null)
                req.buyButton.interactable = unlocked;

            if (req.lockOverlay != null)
                req.lockOverlay.SetActive(!unlocked);
        }
        UpdatePenUnlockOverlays();
    }

    void UpdatePenUnlockOverlays()
    {
        bool pen1Unlocked = Builder.Instance != null && Builder.Instance.isUnlockPen1;
        bool pen2Unlocked = Builder.Instance != null && Builder.Instance.isUnlockPen2;

        if (pen1LockPanel != null)
            pen1LockPanel.SetActive(!pen1Unlocked);

        if (pen2LockPanel != null)
            pen2LockPanel.SetActive(!pen2Unlocked);
    }

    void UpdatePenCountsUI()
    {
        if (pen1 != null && pen1CountText != null)
            pen1CountText.text = $"{pen1.SpawnedAnimalCount} / {pen1.MaxAnimals}";

        if (pen2 != null && pen2CountText != null)
            pen2CountText.text = $"{pen2.SpawnedAnimalCount} / {pen2.MaxAnimals}";
    }

    void BackToBuyMenu()
    {
        confirmPanel.SetActive(false);
        buyCanvas.SetActive(true);
        selectedPen = null;
        selectedType = AnimalType.None;
        UpdateAnimalButtons();
    }

    void CloseAllUI()
    {
        buyCanvas.SetActive(false);
        selectPenPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            bool isActive = buyCanvas.activeSelf;
            buyCanvas.SetActive(!isActive);

            if (!isActive)
                UpdateAnimalButtons();
            else
                CloseAllUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            CloseAllUI();
            selectedType = AnimalType.None;
            selectedPen = null;
        }
    }
}
