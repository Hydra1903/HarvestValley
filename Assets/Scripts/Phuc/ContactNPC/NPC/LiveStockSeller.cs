using Unity.Cinemachine;
using UnityEngine;
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

        // Nút Yes xác nh?n mua
        yesButton.onClick.AddListener(() => ConfirmPurchase());
        // Nút No quay l?i b?ng mua
        noButton.onClick.AddListener(() => BackToBuyMenu());

        // Nút ch?n chu?ng ? sau khi ch?n th? m? b?ng confirm
        pen1Button.onClick.AddListener(() => SelectPen(pen1));
        pen2Button.onClick.AddListener(() => SelectPen(pen2));

        // Nút ch?n lo?i ð?ng v?t ? m? b?ng ch?n chu?ng
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
            Debug.LogError("Pen or Animal havent been choosen");
            return;
        }

        if (!selectedPen.CanSpawnMore())
        {
            Debug.LogWarning("Pen now full cant spawn.");
            confirmPanel.SetActive(false);
            return;
        }

        GameObject prefab = AnimalFactory.GetPrefab(selectedType);
        if (prefab == null)
        {
            Debug.LogError("cant found Prefab");
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

        if (!selectedPen.IsAllowedTag(obj.tag) && selectedPen.HasAssignedType())
        {
            Debug.LogWarning($"Pen not accepted the animal tag '{obj.tag}'");
            Destroy(obj);
            confirmPanel.SetActive(false);
            return;
        }

        selectedPen.RegisterAnimal(obj);

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
