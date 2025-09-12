using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class AnimalPen : MonoBehaviour
{
    public CinemachineInputAxisController playerAxisController;
    public FirstCameraTesting firstCameraTesting;

    [Header("SpawnPoint and Random WP")]
    public Transform spawnPointType1;
    public Transform spawnPointType2;
    public Transform[] wanderPoints;
    public int maxAnimals;

    private List<(GameObject animal, AnimalData data)> spawnedAnimals
        = new List<(GameObject, AnimalData)>(); private HashSet<string> allowedTag = new HashSet<string>();
    public Barn barnReference; 

    [Header("UI")]
    public TMP_Text animalCountText;
    public GameObject animalInfoPanelPrefab;
    public InfoPanelUI sharedInfoPanel;
    public GameObject inventoryPanels;
    public GameObject penInfoPanel;
    public TMP_Text penInfoText;

    [Header("Animal List UI")]
    public Transform animalListParent;       
    public AnimalListUI listUI;
    private List<AnimalInfo> animals = new List<AnimalInfo>();
    public AnimalCellUI[] cells;
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
            Debug.LogWarning($"Tag '{tag}' no allowed to spawn into this pen!");
            return false;
        }

        spawnedAnimals.Add((animal, data));
        UpdateAnimalCountUI();
        // tim cell trong
        for (int i = 0; i < cells.Length; i++)
        {
            if (!cells[i].gameObject.activeSelf || cells[i].IsEmpty())
            {
                cells[i].gameObject.SetActive(true);
                cells[i].Setup(animal, data, i + 1, this);
                break;
            }
        }
        UpdateAnimalCells();
        UpdateAnimalCountUI();
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
        // tat het cell
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Clear();
            cells[i].gameObject.SetActive(false);
        }

        // bat cell dung voi so luong maxAnimal
        for (int i = 0; i < spawnedAnimals.Count && i < maxAnimals; i++)
        {
            var (animal, data) = spawnedAnimals[i];
            cells[i].gameObject.SetActive(true);
            cells[i].Setup(animal, data, i + 1, this);
        }
    }
    public void UpdateAnimalCountUI()
    {
        string countText = $"{spawnedAnimals.Count} / {maxAnimals}";

        if (animalCountText != null)
            animalCountText.text = countText;

        if (penInfoText != null)
            penInfoText.text = "" + countText;
    }
    public bool IsAllowedTag(string tag)
    {
        return allowedTag.Contains(tag);
    }
    public bool HasAssignedType()
    {
        return allowedTag.Count > 0;
    }
    public void ShowPenInfo(bool show)
    {
        if (penInfoPanel != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (playerAxisController != null)
            {
                playerAxisController.enabled = false;
            }
            if (firstCameraTesting != null)
            {
                firstCameraTesting.allowMouseLook = false;
            }
            penInfoPanel.SetActive(show);
            inventoryPanels.SetActive(show);
            if (show)
            {
                UpdateAnimalCountUI();
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

    public void SellAnimal(GameObject animal)
    {
        var index = spawnedAnimals.FindIndex(x => x.animal == animal);
       if (index >= 0)
    {
        var tuple = spawnedAnimals[index];
        spawnedAnimals.RemoveAt(index);
        Destroy(tuple.animal);
        Debug.Log("Selled " + tuple.animal.name);
        UpdateAnimalCountUI();
        UpdateAnimalCells();
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
