using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    public CinemachineInputAxisController playerAxisController;
    public FirstCameraTesting firstCameraTesting;

    [Header("SpawnPoint and Random WP")]
    public Transform spawnPointType1;
    public Transform spawnPointType2;
    public Transform[] wanderPoints;
    public int maxAnimals;

    private List<GameObject> spawnedAnimals = new List<GameObject>();
    private HashSet<string> allowedTag = new HashSet<string>();
    public Barn barnReference; 
    [Header("UI")]
    public TMP_Text animalCountText;
    public GameObject inventoryPanels;
    public GameObject penInfoPanel;
    public TMP_Text penInfoText;

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

    public bool RegisterAnimal(GameObject animal)
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

        spawnedAnimals.Add(animal);
        UpdateAnimalCountUI();
        return true;
    }
    public void RemoveAnimal(GameObject animal)
    {
        if (spawnedAnimals.Contains(animal))
        {
            spawnedAnimals.Remove(animal);
            UpdateAnimalCountUI();
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
