using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// Database chứa tất cả dữ liệu cây trồng trong game
[CreateAssetMenu(fileName = "Plant Database", menuName = "Farm System/Plant Database")]
public class PlantDatabase : ScriptableObject
{
    [Header("All Plants")]
    public List<PlantData> allPlants = new List<PlantData>();
    
    [Header("Quick Access")]
    public PlantData defaultPlant;
    
    /// Tìm PlantData theo PlantType
    public PlantData GetPlantData(PlantType plantType)
    {
        return allPlants.FirstOrDefault(plant => plant.plantType == plantType);
    }
    
    /// Lấy tất cả cây theo kích thước
    public List<PlantData> GetPlantsBySize(PlantSize size)
    {
        return allPlants.Where(plant => plant.size == size).ToList();
    }
    
    /// Lấy tất cả cây có thể trồng trên loại đất này
    public List<PlantData> GetPlantsForSoil(SoilState soilState, bool isHole)
    {
        return allPlants.Where(plant => plant.CanPlantOn(soilState, isHole)).ToList();
    }
    
    /// Kiểm tra xem có PlantData cho PlantType này không
    public bool HasPlantData(PlantType plantType)
    {
        return GetPlantData(plantType) != null;
    }
    
    /// Thêm PlantData mới vào database
    public void AddPlant(PlantData plantData)
    {
        if (plantData != null && !allPlants.Contains(plantData))
        {
            allPlants.Add(plantData);
        }
    }

    /// Xóa PlantData khỏi database
    public void RemovePlant(PlantData plantData)
    {
        if (allPlants.Contains(plantData))
        {
            allPlants.Remove(plantData);
        }
    }
    
    /// Lấy tất cả PlantType có trong database
    public List<PlantType> GetAllPlantTypes()
    {
        return allPlants.Select(plant => plant.plantType).ToList();
    }
    
    /// Validate database - kiểm tra tính hợp lệ
    [ContextMenu("Validate Database")]
    public void ValidateDatabase()
    {
        Debug.Log($"Plant Database contains {allPlants.Count} plants:");
        
        foreach (var plant in allPlants)
        {
            if (plant == null)
            {
                Debug.LogWarning("Found null plant in database!");
                continue;
            }
            
            if (plant.prefab == null)
            {
                Debug.LogWarning($"Plant {plant.plantName} ({plant.plantType}) missing prefab!");
            }
            
            Debug.Log($"- {plant.plantName} ({plant.plantType}) - Size: {plant.size} - Growth: {plant.growthTime}s");
        }
        
        // Kiểm tra duplicate PlantType
        var duplicates = allPlants.GroupBy(p => p.plantType)
                                 .Where(g => g.Count() > 1)
                                 .Select(g => g.Key);
        
        foreach (var duplicate in duplicates)
        {
            Debug.LogWarning($"Duplicate PlantType found: {duplicate}");
        }
    }
}