using UnityEngine;
using UnityEngine.InputSystem;

/// ScriptableObject chứa dữ liệu cây trồng 
[CreateAssetMenu(fileName = "New Plant Data", menuName = "Farm System/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("Basic Info")]
    public string plantName = "New Plant";
    public PlantType plantType = PlantType.Carrot;
    public PlantSize size = PlantSize.Small;

    [Header("Growth Stages")]
    public GameObject[] growthPrefabs;  // Prefab cho từng giai đoạn
    public int[] daysPerStage;          // Số ngày ở mỗi giai đoạn

    [Header("Growth Settings")]
    public ItemData harvestItem;
    public int maxHarvest = 1;          // số lần thu hoạch tối đa // -1 vô hạn
    public int harvestValue = 25;       //sản lượng
    public int xpHarvest = 5;       // XP nhận được khi thu hoạch cây này
    public int energyHarvest = 5;  // Năng lượng tiêu hao khi thu hoạch

    [Header("Second Growth Stages")]
    public GameObject[] matureRegrowPrefabs;     // Giai đoạn phát triển cho cây đã trưởng thành
    public int[] matureRegrowDaysPerStage;       // Số ngày mỗi giai đoạn
    public int useMatureRegrowAfterHarvestCount = 1; //Được sử dụng khi maxHarvest lớn hơn 1

    [Header("Requirements")]
    public bool needsWater = true;      //nước

    [Header("Seasons (arrays)")]
    public SeasonState[] growthSeasons = new SeasonState[] { SeasonState.Spring };
    public SeasonState[] harvestSeasons = new SeasonState[] { SeasonState.Summer };

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

    public bool HasMatureRegrowChain()
    {
        return matureRegrowPrefabs != null && matureRegrowPrefabs.Length > 0;
    }

    //dùng giai đoạn 2 nếu thu hoạch trên 1 lần 
    public int GetRequiredDaysForStage(bool useMatureChain, int stageIndex)
    {
        if (!useMatureChain)
        {
            if (daysPerStage != null && daysPerStage.Length > stageIndex)
                return Mathf.Max(1, daysPerStage[stageIndex]);
            return 1;
        }
        else
        {
            if (matureRegrowDaysPerStage != null && matureRegrowDaysPerStage.Length > stageIndex)
                return Mathf.Max(1, matureRegrowDaysPerStage[stageIndex]);
            return 1;
        }
    }

    public bool CanGrowInSeason(SeasonState s) => ContainsSeason(growthSeasons, s);
    public bool CanHarvestInSeason(SeasonState s) => ContainsSeason(harvestSeasons, s);

    // helper nội bộ, không cần Linq để gọn & tối ưu
    private static bool ContainsSeason(SeasonState[] arr, SeasonState s)
    {
        if (arr == null) return false;
        for (int i = 0; i < arr.Length; i++)
            if (arr[i] == s) return true;
        return false;
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
}