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
    public GameObject[] growthPrefabs;  // Prefab cho từng giai đoạn
    public int[] daysPerStage;          // Số ngày ở mỗi giai đoạn

    [Header("Growth Settings")]
    public int maxHarvest = 1;          // số lần thu hoạch tối đa // -1 vô hạn
    public int harvestValue = 25;       //sản lượng
    public int regrowDays = 3;          // số ngày chờ để ra quả tiếp (cho cây nhiều lần)
    public int regrowStageIndex = -1;   // nếu >=0: stage dùng trong thời gian chờ hồi quả; -1 = giữ stage cuối
    [Header("Requirements")]
    public bool needsWater = true;      //nước
    
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
    public int currentStage = 0;
    public int daysInCurrentStage = 0;
    public int daysUntilNextHarvest = 0; 

    public PlantInstance(PlantData data)
    {
        plantData = data;
        needsWater = data.needsWater;
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