using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    public Transform spawnPointType1;
    public Transform spawnPointType2;
    public Transform[] wanderPoints;
    public int maxAnimals;

    private List<GameObject> spawnedAnimals = new List<GameObject>();

    [Header("UI")]
    public TMP_Text animalCountText;
    public GameObject penInfoPanel;
    public TMP_Text penInfoText;    

    private void Start()
    {
        UpdateAnimalCountUI();
        if (penInfoPanel != null)
            penInfoPanel.SetActive(false);
    }

    public Transform GetRandomSpawnPoint()
    {
        return Random.value < 0.5f ? spawnPointType1 : spawnPointType2;
    }
    public bool CanSpawnMore() => spawnedAnimals.Count < maxAnimals;

    public void RegisterAnimal(GameObject animal)
    {
        spawnedAnimals.Add(animal);
        UpdateAnimalCountUI();
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
    public void ShowPenInfo(bool show)
    {
        if (penInfoPanel != null)
        {
            penInfoPanel.SetActive(show);
            if (show)
                UpdateAnimalCountUI(); 
        }
    }
}
