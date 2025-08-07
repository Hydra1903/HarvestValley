using UnityEngine;

/// <summary>
/// ScriptableObject chứa dữ liệu cây trồng - có thể tạo asset và điều chỉnh trong Inspector
/// </summary>
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
    
    /// <summary>
    /// Kiểm tra xem cây có thể trồng trên loại đất này không
    /// </summary>
    public bool CanPlantOn(SoilState soilState, bool isHole)
    {
        if (soilState != SoilState.Dug) return false;
        
        // Cây 3x3 chỉ trồng trên hố, cây nhỏ hơn trồng trên luống
        if (size == PlantSize.Large)
            return isHole;
        else
            return !isHole;
    }
    
    /// <summary>
    /// Lấy kích thước dưới dạng int
    /// </summary>
    public int GetSizeInt()
    {
        return (int)size;
    }
}

/// <summary>
/// Instance của cây đã trồng - chứa trạng thái runtime
/// </summary>
[System.Serializable]
public class PlantInstance
{
    public PlantData plantData;
    public float currentGrowth = 0f;
    public int harvestCount = 0;
    public bool needsWater = false;
    public bool needsFertilizer = false;
    
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
}