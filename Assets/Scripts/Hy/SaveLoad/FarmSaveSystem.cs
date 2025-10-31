using System.Collections.Generic;
using System;
using UnityEngine;

public class FarmSaveSystem : MonoBehaviour
{
    private FarmManager farm;
    public FarmInput farmInput;
    public void Initialize(FarmManager f) => farm = f;
    public PlantManager plantManager;
    public SoilManager soilManager;
    //Đồng bộ đến FarmGridSave
    public FarmGridSave BuildSave()
    {
        SyncPlantSavesFromWorld();
        return new FarmGridSave
        {
            gridId = farm.gridId,
            width = farm.gridWidth,
            height = farm.gridHeight,
            cellSize = farm.cellSize,
            origin = farm.origin,
            areas = new List<AreaSave>(soilManager.GetAreas()),
            plants = new List<PlantSave>(GetPlants()),
            sprinklers = soilManager.GetSprinklerSaves(),
            wateredCenters = soilManager.GetWateredCenters(),        
        };
    }

    //Hàm Load khởi tạo dựng lại từ dữ liệu
    public void LoadFromSave(FarmGridSave data)
    {
        // ✅ Set thông số trước
        farm.cellSize = data.cellSize;
        farm.origin = data.origin;
        farm.CreateTiles(data.width, data.height);

        // ✅ Soil
        soilManager.ClearAreas();
        foreach (var a in data.areas)
            soilManager.AddAreaFromSave(a);

        // ✅ Plants
        plantManager.ClearPlants();
        foreach (var p in data.plants)
            plantManager.AddPlantFromSave(p);

        // ✅ Sprinklers
        soilManager.ClearSprinklers();
        foreach (var sp in data.sprinklers)
            soilManager.AddSprinklerFromSave(sp);

        // ✅ Watered
        soilManager.ClearWatered();
        soilManager.ApplyWateredCenters(data.wateredCenters);

        if (farmInput.soilGrid) farmInput.soilGrid.UpdateGridColors();   
        if (farmInput.plantGrid) farmInput.plantGrid.UpdateGridColors();

    }


    // ===== Helpers =====
    private List<PlantSave> GetPlants() => plantManager.GetPlants();

    //Ghi lại trạng thái, ngày, số lần thu hoạch về save
    private void SyncPlantSavesFromWorld()
    {
        var plants = GetPlants();
        for (int x = 0; x < farm.gridWidth; x++)
            for (int y = 0; y < farm.gridHeight; y++)
            {
                var tile = farm.Tiles[x, y];
                if (tile.plantInstance != null && tile.plantObject != null)
                {
                    var inst = tile.plantInstance;
                    int idx = plants.FindIndex(p => p.centerX == x && p.centerY == y);
                    if (idx >= 0)
                    {
                        var rec = plants[idx];
                        rec.stage = inst.currentStage;
                        rec.daysInStage = inst.daysInCurrentStage;
                        rec.size = inst.plantData.GetSizeInt();
                        rec.type = inst.plantData.plantType;
                        rec.harvestCount = inst.harvestCount;
                        plants[idx] = rec;
                    }
                }
            }
    }
}

