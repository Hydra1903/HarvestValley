using UnityEngine;

/// ScriptableObject chứa dữ liệu cây trồng 
[CreateAssetMenu(fileName = "New Plant Data", menuName = "Farm System/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("Basic Info")]
    public string plantName = "New Plant";
    public PlantType plantType = PlantType.Carrot;
    public PlantSize size = PlantSize.Small;
    
    [Header("Visual")]
    public GameObject prefab;
    public Sprite icon;

    [Header("Growth Stages")]
    public GameObject[] growthPrefabs; // Prefab cho từng giai đoạn
    public int[] daysPerStage;         // Số ngày ở mỗi giai đoạn

    [Header("Growth Settings")]
    public float growthTime = 10f; // thời gian phát triển (giây)
    public int maxHarvest = 1; // số lần thu hoạch tối đa

    [Header("Requirements")]
    public bool needsWater = true;
    public bool needsFertilizer = false;
    
    [Header("Economic")]
    public int seedCost = 10;
    public int harvestValue = 25;
    
    [Header("Description")]
    [TextArea(3, 5)]
    public string description = "Mô tả về loại cây này...";
    

    /// Kiểm tra xem cây có thể trồng trên loại đất này không
    public bool CanPlantOn(SoilState soilState, bool isHole)
    {
        if (soilState != SoilState.Dug) return false;
        
        // Cây 3x3 chỉ trồng trên hố, cây nhỏ hơn trồng trên luống
        if (size == PlantSize.Large)
            return isHole;
        else
            return !isHole;
    }
    
    public int GetSizeInt()
    {
        return (int)size;
    }
}

/// Instance của cây đã trồng - chứa trạng thái runtime
[System.Serializable]
public class PlantInstance
{
    public PlantData plantData;
    public float currentGrowth = 0f;
    public int harvestCount = 0;
    public bool needsWater = false;
    public bool needsFertilizer = false;
    public int currentStage = 0;
    public int daysInCurrentStage = 0;

    public PlantInstance(PlantData data)
    {
        plantData = data;
        needsWater = data.needsWater;
        needsFertilizer = data.needsFertilizer;
    }
    
    public void UpdateGrowth(float deltaTime)
    {
        if (!IsFullyGrown())
        {
            currentGrowth += deltaTime;
            if (currentGrowth >= plantData.growthTime)
            {
                currentGrowth = plantData.growthTime;
            }
        }
    }
    
    public float GetGrowthProgress()
    {
        return currentGrowth / plantData.growthTime;
    }
    
    public bool IsFullyGrown()
    {
        return currentGrowth >= plantData.growthTime;
    }
    
    public bool CanHarvest()
    {
        return IsFullyGrown() && harvestCount < plantData.maxHarvest;
    }
    
    public void Harvest()
    {
        if (CanHarvest())
        {
            harvestCount++;
        }
    }

    public void AdvanceDay()
    {
        // Nếu không có growthPrefabs thì không làm gì
        if (plantData == null || plantData.growthPrefabs == null || plantData.growthPrefabs.Length == 0)
            return;

        // Nếu đã ở stage cuối thì không tăng nữa
        if (currentStage >= plantData.growthPrefabs.Length - 1) return;

        // Lấy số ngày cần cho stage hiện tại (fallback = 1)
        int requiredDays = 1;
        if (plantData.daysPerStage != null && plantData.daysPerStage.Length > currentStage)
            requiredDays = Mathf.Max(1, plantData.daysPerStage[currentStage]);

        daysInCurrentStage++;

        if (daysInCurrentStage >= requiredDays)
        {
            currentStage++;
            daysInCurrentStage = 0;
        }
    }
}