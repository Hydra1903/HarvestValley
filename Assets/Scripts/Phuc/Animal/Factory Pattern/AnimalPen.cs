using System;
using System.Collections.Generic;
using UnityEngine;


public class AnimalPen : MonoBehaviour
{
    [Header("General Info")]
    public int penId;
    public Barn barnReference;

    [Header("Hay Managers theo cấp chuồng")]
    public HayCellManager penHayCellManager_Level1;
    public HayCellManager penHayCellManager_Level2;

    [Header("UI References")]
    public AnimalPenUIManager uiManager;
    public InfoPanelUI penInfoPanel;
    public ItemData hayItemData;
    public GameObject penVisual_Level1;
    public GameObject penVisual_Level2;

    [Header("Spawn Settings")]
    public Transform spawnPointType1;
    public Transform spawnPointType2;
    public Transform[] wanderPoints;

    [SerializeField] private int baseMaxAnimals = 4;
    private int maxAnimals; 

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
        public GameObject prefab;
        public bool canHarvest;
        public AnimalData data;
    }

    public HayCellManager penHayCellManager
    {
        get
        {
            int level = GetCurrentPenLevel();
            return level == 1 ? penHayCellManager_Level1 : penHayCellManager_Level2;
        }
    }

    private void Start()
    {
        if (InfoPanelManager.instance != null)
            InfoPanelManager.instance.RegisterPanel(penId, null);

        UpdateMaxAnimals();
        UpdateActiveHayManager();
    }

    private void OnDestroy()
    {
        if (InfoPanelManager.instance != null)
            InfoPanelManager.instance.UnregisterPanel(penId);
    }

    private int GetCurrentPenLevel()
    {
        if (Builder.Instance == null) return 1;
        return penId == 1 ? Builder.Instance.currentlevelPen1 : Builder.Instance.currentlevelPen2;
    }

    public void UpdateMaxAnimals()
    {
        int level = GetCurrentPenLevel();

        if (penId == 1 || penId == 2)
        {
            if (level == 1)
            {
                maxAnimals = 4;
                baseMaxAnimals = 4;
            }
            else if (level == 2)
            {
                maxAnimals = 7;
                baseMaxAnimals = 7;
            }
        }
        else
        {
            maxAnimals = baseMaxAnimals;
        }
    }


    public void UpdateActiveHayManager()
    {
        int level = GetCurrentPenLevel();
        UpdateMaxAnimals();

        if (penHayCellManager_Level1 != null)
            penHayCellManager_Level1.gameObject.SetActive(level == 1);
        if (penHayCellManager_Level2 != null)
            penHayCellManager_Level2.gameObject.SetActive(level == 2);
        if (penVisual_Level1 != null)
            penVisual_Level1.SetActive(level == 1);
        if (penVisual_Level2 != null)
            penVisual_Level2.SetActive(level == 2);

        uiManager?.UpdateUIPen();
    }


    public Vector3 GetRandomSpawnPosition()
    {
        Transform basePoint = UnityEngine.Random.value < 0.5f ? spawnPointType1 : spawnPointType2;
        Vector2 offset = UnityEngine.Random.insideUnitCircle * 1.5f;
        return basePoint.position + new Vector3(offset.x, 0f, offset.y);
    }

    public bool RegisterAnimal(GameObject animal, AnimalData data)
    {
        if (animal == null) return false;

        // Tạo ID duy nhất dựa trên tên prefab + thời gian hiện tại
        string uniqueID = animal.name + "_" + DateTime.Now.Ticks;
        animal.name = uniqueID;

        string tag = animal.tag;

        // Kiểm tra tag cho phép
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
            feeding.hayCellManager = penHayCellManager;

            var info = new AnimalPenSaveInfo
            {
                animalID = uniqueID,
                animalType = feeding.animalTypes,
                daysFed = feeding.daysFed,
                canHarvest = feeding.CanHarvest(),
                variant = animal.name.Contains("Black") ? "Black" :
                          animal.name.Contains("Cream") ? "Cream" : "White"
            };
            savedAnimals.Add(info);
        }

        var infoComp = animal.GetComponent<AnimalInfo>();
        if (infoComp != null)
        {
            infoComp.panelUI = penInfoPanel;
            infoComp.data = data;
        }

        return true;
    }


    public void RemoveAnimal(GameObject animal)
    {
        int index = spawnedAnimals.FindIndex(a => a.animal == animal);
        if (index >= 0)
        {
            savedAnimals.RemoveAll(a => a.animalID == animal.name);
            spawnedAnimals.RemoveAt(index);
        }

        if (spawnedAnimals.Count == 0)
            allowedTags.Clear();
    }

    public bool IsAnyAnimalFed()
    {
        if (spawnedAnimals.Count == 0)
            return false;

        foreach (var (animal, _) in spawnedAnimals)
        {
            if (animal == null) continue;
            var feeding = animal.GetComponent<AnimalFedding>();
            if (feeding == null) continue;

            if (feeding.GetMealsToday() > 0)
                return true;
        }
        return false;
    }

    public void SellAnimal(int cellIndex)
    {
        if (cellIndex < 0 || cellIndex >= spawnedAnimals.Count)
            return;

        var (animal, _) = spawnedAnimals[cellIndex];
        if (animal != null)
        {
            Destroy(animal);
        }

        savedAnimals.RemoveAll(a => a.animalID == animal.name);
        spawnedAnimals.RemoveAt(cellIndex);

        if (spawnedAnimals.Count == 0)
        {
            allowedTags.Clear();
        }

        uiManager?.RefreshUI();
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
    public bool CanSpawnMore() => spawnedAnimals.Count < maxAnimals;
    public int SpawnedAnimalCount => spawnedAnimals.Count;
    public int MaxAnimals => maxAnimals;
    public List<(GameObject, AnimalData)> GetSpawnedAnimals() => spawnedAnimals;
   public void LoadAnimals(List<AnimalPenSaveInfo> savedList)
{
    foreach (var info in savedList)
    {
        // Lấy prefab dựa trên animalType và variant, không dùng info.data.prefab
        GameObject prefab = SaveLoadSystem.GetPrefabFromFeedingType(info.animalType, info.variant);
        if (prefab == null) continue;

        GameObject obj = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity);

        var feed = obj.GetComponent<AnimalFedding>();
        if (feed != null)
        {
            feed.SetSavedState(info.daysFed, info.canHarvest, false, 0, false, false);
        }

        var infoComp = obj.GetComponent<AnimalInfo>();
        if (infoComp != null)
        {
            infoComp.panelUI = penInfoPanel;
            infoComp.data = info.data; // vẫn giữ nếu bạn muốn tham chiếu AnimalData
        }

        // Thêm vào list và tag
        spawnedAnimals.Add((obj, info.data));
        allowedTags.Add(obj.tag);
    }

    uiManager?.RefreshUI();
}


    public List<AnimalPenSaveInfo> GetCurrentSavedAnimals()
    {
        List<AnimalPenSaveInfo> list = new List<AnimalPenSaveInfo>();
        foreach (var (animal, data) in spawnedAnimals)
        {
            var feed = animal.GetComponent<AnimalFedding>();
            if (feed == null) continue;

            list.Add(new AnimalPenSaveInfo
            {
                animalID = animal.name,
                animalType = feed.animalTypes,
                variant = data.variant,
                daysFed = feed.GetDaysFed(),
                canHarvest = feed.CanHarvest()
            });
        }
        return list;
    }
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
