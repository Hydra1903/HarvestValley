using System.Collections.Generic;
using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    [Header("General Info")]
    public int penId;
    public Barn barnReference;
    public HayCellManager penHayCellManager_Level1;
    public HayCellManager penHayCellManager_Level2;

    public AnimalPenUIManager uiManager;
    public InfoPanelUI penInfoPanel;
    public ItemData hayItemData;
    [Header("Spawn Settings")]
    public Transform spawnPointType1;
    public Transform spawnPointType2;
    public Transform[] wanderPoints;
    public int maxAnimals = 5;

    [Range(1, 2)]
    public int currentLevel = 1;

    private List<(GameObject animal, AnimalData data)> spawnedAnimals = new();
    private HashSet<string> allowedTags = new();

    public List<AnimalPenSaveInfo> savedAnimals = new();
    [System.Serializable]

    public class AnimalPenSaveInfo
    {
        public string animalID;
        public AnimalFedding.FeedingAnimalType animalType;
        public string variant;
        public int daysFed;
        public bool canHarvest;
    }
    public HayCellManager penHayCellManager
    {
        get
        {
            // Ưu tiên cấp 1 nếu có, nếu không thì dùng cấp 2
            return penHayCellManager_Level1 != null ? penHayCellManager_Level1 : penHayCellManager_Level2;
        }
    }
    private void Start()
    {
        if (InfoPanelManager.instance != null)
            InfoPanelManager.instance.RegisterPanel(penId, null);
        UpdateActiveHayManager();
    }

    private void OnDestroy()
    {
        if (InfoPanelManager.instance != null)
            InfoPanelManager.instance.UnregisterPanel(penId);
    }
    public void SwitchLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 2);
        UpdateActiveHayManager();
    }
    private void UpdateActiveHayManager()
    {
        if (penHayCellManager_Level1 != null)
            penHayCellManager_Level1.gameObject.SetActive(currentLevel == 1);
        if (penHayCellManager_Level2 != null)
            penHayCellManager_Level2.gameObject.SetActive(currentLevel == 2);

        uiManager?.UpdateUIForLevel(currentLevel);
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

        var feeding = animal.GetComponent<AnimalFedding>();
        if (feeding != null)
        {
            feeding.barn = barnReference;
            feeding.hayCellManager = currentLevel == 1 ? penHayCellManager_Level1 : penHayCellManager_Level2;

            var info = new AnimalPenSaveInfo
            {
                animalID = animal.name,
                animalType = feeding.animalTypes,
                daysFed = feeding.daysFed,
                canHarvest = feeding.CanHarvest(),
                variant = animal.name.Contains("Black") ? "Black" :
                          animal.name.Contains("Cream") ? "Cream" : "White"
            };
            savedAnimals.Add(info);
        }

        var infos = animal.GetComponent<AnimalInfo>();
        if (infos != null && penInfoPanel != null)
            infos.InjectPanel(penInfoPanel);

        return true;
    }

    public void RemoveAnimal(GameObject animal)
    {
        int index = spawnedAnimals.FindIndex(a => a.animal == animal);
        if (index >= 0)
        {
            if (index < savedAnimals.Count)
                savedAnimals.RemoveAt(index);
            spawnedAnimals.RemoveAt(index);
        }

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

            if (feeding.animalTypes == AnimalFedding.FeedingAnimalType.Sheep)
            {
                if (feeding.GetMealsToday() < 1)
                    return false;
            }
            else if (feeding.animalTypes == AnimalFedding.FeedingAnimalType.Goat)
            {
                if (feeding.GetMealsToday() < 1)
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

        if (cellIndex < savedAnimals.Count)
            savedAnimals.RemoveAt(cellIndex);

        spawnedAnimals.RemoveAt(cellIndex);

        if (spawnedAnimals.Count == 0)
            allowedTags.Clear();

        uiManager?.RefreshUI();
        //SaveLoadSystem.SaveFarm(FindAnyObjectByType<PensManager>().allPens, FindAnyObjectByType<PensManager>().allHayManagers);
    }
    public void UpdateSavedAnimalData(GameObject animal)
    {
        var feed = animal.GetComponent<AnimalFedding>();
        if (feed == null) return;

        int index = savedAnimals.FindIndex(a => a.animalID == animal.name);
        if (index >= 0)
        {
            savedAnimals[index].daysFed = feed.GetDaysFed();
            savedAnimals[index].canHarvest = feed.CanHarvest();
        }
    }
    public void UpdateAnimalFeedStatusUI()
    {
        uiManager?.UpdateFeedStatus();
    }
    public bool HasAssignedType() => allowedTags.Count > 0;
    public bool IsAllowedTag(string tag) => allowedTags.Contains(tag);

    public int SpawnedAnimalCount => spawnedAnimals.Count;
    public int MaxAnimals => maxAnimals;
    public List<(GameObject, AnimalData)> GetSpawnedAnimals() => spawnedAnimals;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager?.ShowPenInfo(true);
            uiManager?.ShowInventory(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager?.ShowPenInfo(false);
            uiManager?.ShowInventory(false);
        }
    }
}
