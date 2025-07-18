using UnityEngine;
using UnityEngine.UI;

public class LiveStockSeller : MonoBehaviour
{
    public GameObject buyCanvas;
    public GameObject confirmPanel;
    public GameObject selectPenPanel;

    private bool playerInRange = false;
    private AnimalType selectedType = AnimalType.None;

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

        yesButton.onClick.AddListener(() => ShowSelectPen());
        noButton.onClick.AddListener(OnCancelPurchase);

        pen1Button.onClick.AddListener(() => SpawnAnimalInPen(pen1));
        pen2Button.onClick.AddListener(() => SpawnAnimalInPen(pen2));

        WhiteGoatButton.onClick.AddListener(() => ShowConfirm(AnimalType.WhiteGoat));
        BlackGoatButton.onClick.AddListener(() => ShowConfirm(AnimalType.BlackGoat));
        WhiteSheepButton.onClick.AddListener(() => ShowConfirm(AnimalType.WhiteSheep));
        CreamSheepButton.onClick.AddListener(() => ShowConfirm(AnimalType.CreamSheep));
        BlackSheepButton.onClick.AddListener(() => ShowConfirm(AnimalType.BlackSheep));
    }

    void ShowConfirm(AnimalType type)
    {
        selectedType = type;
        confirmPanel.SetActive(true);
    }

    void ShowSelectPen()
    {
        confirmPanel.SetActive(false);
        selectPenPanel.SetActive(true);
    }

    void SpawnAnimalInPen(AnimalPen pen)
    {
        if (!pen.CanSpawnMore())
        {
            Debug.LogWarning("Pen Full Cant Spawn");
            return;
        }

        GameObject prefab = AnimalFactory.GetPrefab(selectedType);
        if (prefab == null)
        {
            Debug.LogError("Cant Find!");
            return;
        }

        Transform spawnPoint = pen.GetRandomSpawnPoint();
        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        SimpleAI ai = obj.GetComponent<SimpleAI>();
        if (ai != null)
        {
            ai.wanderPoints = pen.wanderPoints;
        }

        pen.RegisterAnimal(obj); //Sign New Animal in Pen

        selectPenPanel.SetActive(false);
        buyCanvas.SetActive(false);
        selectedType = AnimalType.None;
    }

    void ResetUI()
    {
        buyCanvas.gameObject.SetActive(false);
        confirmPanel.SetActive(false);
        selectPenPanel.SetActive(false);
        selectedType = AnimalType.None;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void OnCancelPurchase()
    {
        confirmPanel.SetActive(false);
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
            }
            else
            {
                ResetUI();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            buyCanvas.gameObject.SetActive(false);
            confirmPanel.SetActive(false);
            selectedType = AnimalType.None;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
