using System.Collections.Generic;
using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    [Header("General Info")]
    public int penId;
    public Barn barnReference;
    public AnimalPenUIManager uiManager;
    public InfoPanelUI penInfoPanel;
    [Header("Spawn Settings")]
    public Transform spawnPointType1;
    public Transform spawnPointType2;
    public Transform[] wanderPoints;
    public int maxAnimals = 5;

    private List<(GameObject animal, AnimalData data)> spawnedAnimals = new();
    private HashSet<string> allowedTags = new();

    private void Start()
    {
        if (InfoPanelManager.instance != null)
            InfoPanelManager.instance.RegisterPanel(penId, null);
    }

    private void OnDestroy()
    {
        if (InfoPanelManager.instance != null)
            InfoPanelManager.instance.UnregisterPanel(penId);
    }

    public Vector3 GetRandomSpawnPosition()
    {
        Transform basePoint = Random.value < 0.5f ? spawnPointType1 : spawnPointType2;
        Vector2 offset = Random.insideUnitCircle * 1.5f;
        return basePoint.position + new Vector3(offset.x, 0f, offset.y);
    }

    public bool CanSpawnMore() => spawnedAnimals.Count < maxAnimals;

    public bool RegisterAnimal(GameObject animal, AnimalData data)
    {
        string tag = animal.tag;

        if (allowedTags.Count == 0)
        {
            allowedTags.Add(tag);
        }
        else if (!allowedTags.Contains(tag))
        {
            Destroy(animal);
            return false;
        }

        spawnedAnimals.Add((animal, data));
        var info = animal.GetComponent<AnimalInfo>();
        if (info != null && penInfoPanel != null)
            info.InjectPanel(penInfoPanel);
        return true;
    }

    public void RemoveAnimal(GameObject animal)
    {
        int index = spawnedAnimals.FindIndex(a => a.animal == animal);
        if (index >= 0)
            spawnedAnimals.RemoveAt(index);

        if (spawnedAnimals.Count == 0)
            allowedTags.Clear();
    }
    public bool AreAllAnimalsFed()
    {
        if (spawnedAnimals.Count == 0)
            return false;

        foreach (var (animal, _) in spawnedAnimals)
        {
            if (animal == null) continue;
            var feeding = animal.GetComponent<AnimalFedding>();
            if (feeding == null) continue;

            if (feeding.animalType == AnimalFedding.AnimalType.Sheep)
            {
                if (feeding.GetMealsToday() < 1)
                    return false;
            }
            else if (feeding.animalType == AnimalFedding.AnimalType.Goat)
            {
                if (feeding.GetMealsToday() < 2)
                    return false;
            }
        }

        return true;
    }

    public void SellAnimal(int cellIndex)
    {
        if (cellIndex < 0 || cellIndex >= spawnedAnimals.Count)
            return;

        var (animal, _) = spawnedAnimals[cellIndex];
        if (animal != null)
            Destroy(animal);

        spawnedAnimals.RemoveAt(cellIndex);

        if (spawnedAnimals.Count == 0)
            allowedTags.Clear();

        uiManager?.RefreshUI();
    }
    public void UpdateAnimalFeedStatusUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateFeedStatus();
        }
    }
    public bool HasAssignedType() => allowedTags.Count > 0;
    public bool IsAllowedTag(string tag) => allowedTags.Contains(tag);

    public int SpawnedAnimalCount => spawnedAnimals.Count;
    public int MaxAnimals => maxAnimals;
    public List<(GameObject, AnimalData)> GetSpawnedAnimals() => spawnedAnimals;
}