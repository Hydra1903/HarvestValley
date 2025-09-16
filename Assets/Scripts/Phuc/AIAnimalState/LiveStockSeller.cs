using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LiveStockSeller : MonoBehaviour
{
    public CinemachineInputAxisController playerAxisController;
    public FirstCameraTesting firstCameraTesting;

    public GameObject buyCanvas;
    public GameObject confirmPanel;
    public GameObject selectPenPanel;

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

    [Header("Spawn Point And Moving Random Point")]
    public AnimalPen pen1;
    public AnimalPen pen2;

    private void Start()
    {
        buyCanvas.gameObject.SetActive(false);
        confirmPanel.SetActive(false);
        selectPenPanel.SetActive(false);

        yesButton.onClick.AddListener(() => ConfirmPurchase());
        noButton.onClick.AddListener(() => BackToBuyMenu());

        pen1Button.onClick.AddListener(() => SelectPen(pen1));
        pen2Button.onClick.AddListener(() => SelectPen(pen2));

        WhiteGoatButton.onClick.AddListener(() => ShowSelectPen(AnimalType.WhiteGoat));
        BlackGoatButton.onClick.AddListener(() => ShowSelectPen(AnimalType.BlackGoat));
        WhiteSheepButton.onClick.AddListener(() => ShowSelectPen(AnimalType.WhiteSheep));
        CreamSheepButton.onClick.AddListener(() => ShowSelectPen(AnimalType.CreamSheep));
        BlackSheepButton.onClick.AddListener(() => ShowSelectPen(AnimalType.BlackSheep));
    }

    void ShowSelectPen(AnimalType type)
    {
        selectedType = type;
        selectPenPanel.SetActive(true);
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
        {
            Notification.Instance.ShowNotification("Chuồng Nuôi hoặc động vật chưa được chọn");
            return;
        }
        if (!selectedPen.CanSpawnMore())
        {
            Notification.Instance.ShowNotification("Chuồng Nuôi Đã Đầy!");
            confirmPanel.SetActive(false);
            return;
        }
        GameObject prefab = AnimalFactory.GetPrefab(selectedType);
        if (prefab == null)
        {
            Debug.LogError("Không tìm thấy prefab");
            return;
        }
        GameObject obj = Instantiate(prefab, selectedPen.GetRandomSpawnPosition(), Quaternion.identity);

        SimpleAI ai = obj.GetComponent<SimpleAI>();
        if (ai != null)
        {
            ai.wanderPoints = selectedPen.wanderPoints;
        }
        AnimalFedding feeding = obj.GetComponent<AnimalFedding>();
        if (feeding != null)
        {
            feeding.barn = selectedPen.barnReference;
        }
        var info = obj.GetComponent<AnimalInfo>();
        var panel = InfoPanelManager.instance.GetPanel(selectedPen.penId);
        if (panel != null)
        {
            info.InjectPanel(panel);
        }
        AnimalData data = obj.GetComponent<AnimalInfo>()?.data;
        bool success = selectedPen.RegisterAnimal(obj, data);
        if (success)
        {
            Notification.Instance.ShowNotification($"Đã Thêm động vật đã mua vào {selectedPen.name}");
        }
        else
        {
            Notification.Instance.ShowNotification($"động vật đã chọn không được thêm vào {selectedPen.name} (loại không hợp lệ)");
        }
        confirmPanel.SetActive(false);
        selectedType = AnimalType.None;
        selectedPen = null;
        CloseAllUI();
    }
    void BackToBuyMenu()
    {
        confirmPanel.SetActive(false);
        buyCanvas.SetActive(true);
        selectedPen = null;
        selectedType = AnimalType.None;
    }

    void CloseAllUI()
    {
        buyCanvas.SetActive(false);
        selectPenPanel.SetActive(false);
        confirmPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerAxisController != null)
            playerAxisController.enabled = true;

        firstCameraTesting.allowMouseLook = true;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            bool isActive = buyCanvas.activeSelf;
            buyCanvas.SetActive(!isActive);

            if (!isActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (playerAxisController != null)
                    playerAxisController.enabled = false;

                firstCameraTesting.allowMouseLook = false;
            }
            else
            {
                CloseAllUI();
            }
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
