using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class AnimalPen : MonoBehaviour
{
    public int penId;
    public CinemachineInputAxisController playerAxisController;
    public FirstCameraTesting firstCameraTesting;

    [Header("SpawnPoint and Random WP")]
    public Transform spawnPointType1;
    public Transform spawnPointType2;
    public Transform[] wanderPoints;
    public int maxAnimals;

    private List<(GameObject animal, AnimalData data)> spawnedAnimals
        = new List<(GameObject, AnimalData)>();
    private HashSet<string> allowedTag = new HashSet<string>();
    public Barn barnReference;

    [Header("UI")]
    public TMP_Text animalCountText;
    public GameObject animalInfoPanelPrefab;
    public InfoPanelUI sharedInfoPanel;
    public GameObject inventoryPanels;
    public GameObject penInfoPanel;
    public TMP_Text penInfoText;
    public TMP_Text penQuality;
    public GameObject confirmSellPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Animal List UI")]
    public Transform animalListParent;
    public AnimalListUI listUI;
    private List<AnimalInfo> animals = new List<AnimalInfo>();
    public AnimalCellUI[] cells;
    private int pendingSellIndex = -1;

    private void Start()
    {
        UpdateAnimalCountUI();
        if (penInfoPanel != null)
        {
            penInfoPanel.SetActive(false);
            inventoryPanels.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (playerAxisController != null)
            {
                playerAxisController.enabled = true;
            }
            if (firstCameraTesting != null)
            {
                firstCameraTesting.allowMouseLook = true;
            }
        }
        if (sharedInfoPanel != null)
        {
            InfoPanelManager.instance.RegisterPanel(penId, sharedInfoPanel);
        }
    }

    private void OnDestroy()
    {
        if (InfoPanelManager.instance != null)
        {
            InfoPanelManager.instance.UnregisterPanel(penId);
        }
    }

    public Vector3 GetRandomSpawnPosition()
    {
        Transform basePoint = Random.value < 0.5f ? spawnPointType1 : spawnPointType2;
        Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
        Vector3 spawnPos = basePoint.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
        return spawnPos;
    }

    public bool CanSpawnMore() => spawnedAnimals.Count < maxAnimals;

    public bool RegisterAnimal(GameObject animal, AnimalData data)
    {
        string tag = animal.tag;
        if (allowedTag.Count == 0)
        {
            allowedTag.Add(tag);
        }
        else if (!allowedTag.Contains(tag))
        {
            Notification.Instance.ShowNotification($"Chu?ng Nuôi {penId} ch? ch?p nh?n {string.Join(",", allowedTag)}, không th? thêm {tag}!");
            Destroy(animal);
            return false;
        }
        spawnedAnimals.Add((animal, data));
        UpdateAnimalCells();
        UpdateAnimalCountUI();
        for (int i = 0; i < cells.Length; i++)
        {
            if (!cells[i].gameObject.activeSelf)
            {
                cells[i].gameObject.SetActive(true);
                cells[i].Setup(animal, data, i, this);
                break;
            }
        }
        return true;
    }

    public void RemoveAnimal(GameObject animal)
    {
        var index = spawnedAnimals.FindIndex(x => x.animal == animal);
        if (index >= 0)
        {
            spawnedAnimals.RemoveAt(index);
            UpdateAnimalCountUI();
            UpdateAnimalCells();
        }
    }

    public void UpdateAnimalCells()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Clear();
            cells[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < spawnedAnimals.Count && i < maxAnimals; i++)
        {
            var (animal, data) = spawnedAnimals[i];
            cells[i].gameObject.SetActive(true);
            cells[i].Setup(animal, data, i, this);
            cells[i].SetIndexNumber(i + 1);
        }
    }

    public void UpdateAnimalCountUI()
    {
        string countText = $"{spawnedAnimals.Count} / {maxAnimals}";

        if (animalCountText != null)
            animalCountText.text = countText;

        if (penInfoText != null)
            penInfoText.text = countText;

        // ð?ng b? t?nh tr?ng ãn u?ng
        UpdateAnimalFeedStatusUI();
    }

    public void UpdateAnimalFeedStatusUI()
    {
        bool allGood = true;

        foreach (var (animal, data) in spawnedAnimals)
        {
            if (animal == null) continue;
            var feeding = animal.GetComponent<AnimalFedding>();
            if (feeding == null) continue;

            if (feeding.animalType == AnimalFedding.AnimalType.Sheep)
            {
                if (!feeding.HasEatenToday())
                {
                    allGood = false;
                    break;
                }
            }
            else if (feeding.animalType == AnimalFedding.AnimalType.Goat)
            {
                if (feeding.GetMealsToday() < 2)
                {
                    allGood = false;
                    break;
                }
            }
        }

        string statusText = allGood && spawnedAnimals.Count > 0 ? "Good" : "Bad";
        Color statusColor = allGood && spawnedAnimals.Count > 0 ? Color.green : Color.red;
        if (penQuality != null)
        {
            penQuality.text = statusText;
            penQuality.color = statusColor;

        }
    }

    public bool IsAllowedTag(string tag) => allowedTag.Contains(tag);

    public bool HasAssignedType() => allowedTag.Count > 0;

    public void ShowPenInfo(bool show)
    {
        if (penInfoPanel != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerAxisController != null)
                playerAxisController.enabled = false;
            if (firstCameraTesting != null)
                firstCameraTesting.allowMouseLook = false;

            penInfoPanel.SetActive(show);
            inventoryPanels.SetActive(show);

            if (show)
            {
                UpdateAnimalCountUI();
                UpdateAnimalCells();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (playerAxisController != null)
                    playerAxisController.enabled = true;

                if (firstCameraTesting != null)
                    firstCameraTesting.allowMouseLook = true;
            }
        }
    }

    public void ShowConfirmSell(int cellIndex, string animalName)
    {
        if (confirmSellPanel == null) return;
        pendingSellIndex = cellIndex;
        confirmSellPanel.SetActive(true);
        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() =>
            {
                SellAnimal(pendingSellIndex);
                confirmSellPanel.SetActive(false);
                pendingSellIndex = -1;
            });
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(() =>
            {
                confirmSellPanel.SetActive(false);
                pendingSellIndex = -1;
            });
        }
    }

    public void SellAnimal(int cellIndex)
    {
        if (cellIndex < 0 || cellIndex >= spawnedAnimals.Count) return;

        var (animal, data) = spawnedAnimals[cellIndex];
        if (animal != null) Destroy(animal);

        spawnedAnimals.RemoveAt(cellIndex);

        Notification.Instance.ShowNotification($"Chu?ng Nuôi {penId} ð? bán 1 ð?ng v?t!");
        UpdateAnimalCountUI();
        UpdateAnimalCells();

        if (spawnedAnimals.Count == 0)
        {
            allowedTag.Clear();
        }
    }

    public void AddAnimal(AnimalInfo animal)
    {
        animals.Add(animal);
        listUI.Refresh(animals);
    }

    public void RemoveAnimal(AnimalInfo animal)
    {
        animals.Remove(animal);
        listUI.Refresh(animals);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            penInfoPanel.SetActive(false);
            inventoryPanels.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerAxisController != null)
                playerAxisController.enabled = true;

            if (firstCameraTesting != null)
                firstCameraTesting.allowMouseLook = true;
        }
    }
}
