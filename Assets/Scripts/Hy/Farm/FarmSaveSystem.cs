using System.Collections.Generic;
using System;
using UnityEngine;

public class FarmSaveSystem : MonoBehaviour
{
    private FarmManager farm;

    public void Initialize(FarmManager f) => farm = f;

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
            areas = new List<AreaSave>(SoilManager.Instance.GetAreas()),
            plants = new List<PlantSave>(GetPlants())
        };
    }

    //Hàm Load khởi tạo dựng lại từ dữ liệu
    public void LoadFromSave(FarmGridSave data)
    {
        // apply size + origin
        farm.AllocateTiles(data.width, data.height);
        farm.cellSize = data.cellSize;
        farm.origin = data.origin;

        // soil
        SoilManager.Instance.ClearAreas();
        foreach (var a in data.areas)
            SoilManager.Instance.AddAreaFromSave(a);

        // plants
        PlantManager.Instance.ClearPlants();
        foreach (var p in data.plants)
        PlantManager.Instance.AddPlantFromSave(p);
    }

    // ===== Helpers =====
    private List<PlantSave> GetPlants() => PlantManager.Instance.GetPlants();

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

