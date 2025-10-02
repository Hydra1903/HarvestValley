using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [Header("Ghost cây")]
    public Material ghostMaterial;
    private SimpleGhostManager simpleGhostManager;

    private FarmManager farmManager;
    private SoilManager soilManager;

    private readonly List<PlantSave> _plantSaves = new();
    public List<PlantSave> GetPlants() => _plantSaves;

    public void Initialize(FarmManager f, SoilManager s)
    {
        farmManager = f; soilManager = s;

        var ghostManagerObj = new GameObject($"SimpleGhostManager_{farmManager.gridId}");
        simpleGhostManager = ghostManagerObj.AddComponent<SimpleGhostManager>();
        simpleGhostManager.Initialize(ghostMaterial);
    }

    // ===== Hover SEED =====

    //Lấy dữ liệu 
    public bool HandleSeedHover(Vector2Int gridPos, InventoryItem seedItem)
    {
        if (seedItem == null || seedItem.itemData == null || seedItem.itemData.itemType != ItemType.Seed)
        { HideGhost(); return false; }

        var pd = farmManager.plantDatabase ? farmManager.plantDatabase.GetPlantData(seedItem.itemData.plantType) : null;
        if (pd == null) { HideGhost(); return false; }

        int size = pd.GetSizeInt();
        Vector2Int start;

        // Với cây 3x3: bắt buộc snap vào đúng 1 hố 3x3
        if (size == 3)
        {
            if (!soilManager.TrySnapStartToHole3x3(gridPos, out start)) { HideGhost(); return false; }
            if (!soilManager.IsExactAreaMatch(start.x, start.y, 3, SoilType.Hole)) { HideGhost(); return false; }
        }
        else
        {
            start = farmManager.CalculateStartPosition(gridPos, size);
        }

        if (!AreaIsDug(start, size))
        {
            HideGhost();
            return false;
        }


        if (!CanPlantAt(start, size, pd)) { HideGhost(); return false; }

        float offset = size * 0.5f;
        Vector3 ghostPos = farmManager.origin + new Vector3(
            (start.x + offset) * farmManager.cellSize,
            0.45f,
            (start.y + offset) * farmManager.cellSize
        );
        simpleGhostManager.ShowGhost(pd, ghostPos);
        return true;
    }

    public void HideGhost() => simpleGhostManager?.HideGhost();

    // ===== Trồng =====
    public bool TryPlant(Vector2Int gridPos, InventoryItem seedItem)
    {
        if (seedItem == null || seedItem.itemData == null || seedItem.itemData.itemType != ItemType.Seed) return false;

        var pd = farmManager.plantDatabase ? farmManager.plantDatabase.GetPlantData(seedItem.itemData.plantType) : null;
        if (pd == null) return false;

        int size = pd.GetSizeInt();
        Vector2Int start;
        if (size == 3)
        {
            if (!soilManager.TrySnapStartToHole3x3(gridPos, out start)) return false;
            if (!soilManager.IsExactAreaMatch(start.x, start.y, 3, SoilType.Hole)) return false;
        }
        else
        {
            start = farmManager.CalculateStartPosition(gridPos, size);
        }

        if (!CanPlantAt(start, size, pd)) return false;

        PlantSeed(start, pd);
        return true;
    }

    // ===== Thu hoạch =====
    public bool TryHarvest(Vector2Int gridPos)
    {
        int x = gridPos.x, y = gridPos.y;
        if (!farmManager.IsInGrid(x, y)) return false;

        var t = farmManager.Tiles[x, y];
        if (t.plantInstance == null) { Debug.Log("Không có cây."); return false; }

        if (!TryGetPlantCenterFrom(x, y, out int cx, out int cy))
        { Debug.LogWarning("Không tìm được ô tâm."); return false; }

        var inst = farmManager.Tiles[cx, cy].plantInstance;
        if (inst == null || inst.plantData == null) { Debug.LogWarning("Thiếu dữ liệu cây."); return false; }
        if (!IsMature(inst)) { Debug.Log("Chưa chín."); return false; }

        int cost = Mathf.Max(0, inst.plantData.energyHarvest);
        // chỉ kiểm tra đủ NL
        if (Mp.Instance != null && Mp.Instance.mp < cost)
        { Notification.Instance?.ShowNotification("Hết năng lượng!"); return false; }

        int yield = Mathf.Max(0, inst.plantData.harvestValue);
        if (yield <= 0 || inst.plantData.harvestItem == null)
        { Debug.LogWarning("[Harvest] Dữ liệu harvest không hợp lệ."); return false; }

        // Thêm vào túi (1 lần). Nếu thất bại -> không trừ NL, không đổi state.
        if (Inventory.Instance == null || !Inventory.Instance.AddItem(inst.plantData.harvestItem, yield))
        {
            Debug.LogWarning($"[Harvest] Túi đầy, không thể thu {yield} x {inst.plantData.harvestItem.itemName}");
            return false;
        }

        // Add thành công -> trừ NL
        if (cost > 0) Mp.Instance?.UseMp(cost);

        // cập nhật trạng thái + XP
        inst.harvestCount++;
        if (Xp.Instance != null) Xp.Instance.AddXp(Mathf.Max(0, inst.plantData.xpHarvest));
        Debug.Log($"Thu hoạch {inst.plantData.plantName} (+{yield}) +{inst.plantData.xpHarvest} XP | Lần {inst.harvestCount}/{(inst.plantData.maxHarvest < 0 ? "∞" : inst.plantData.maxHarvest.ToString())}");

        // Regrow?
        if (HasMoreHarvests(inst))
        {
            if (UseMatureChain(inst))
            {
                inst.currentStage = 0;
                inst.daysInCurrentStage = 0;
                inst.daysUntilNextHarvest = 0;
                ReplacePlantMeshAtCenter(cx, cy, inst);
            }
            return true;
        }

        // Hết lượt -> xóa cây + save
        RemovePlantAtCenter(cx, cy);
        int idx = _plantSaves.FindIndex(p => p.centerX == cx && p.centerY == cy);
        if (idx >= 0) _plantSaves.RemoveAt(idx);
        return true;
    }

    // ===== Qua ngày =====
    public void AdvanceDay()
    {
        //Cập nhật UI
        //GameTime.Instance.NextDay();

        // Thời tiết hiện tại
        bool isRainy = Weather.Instance != null && Weather.Instance.currentWeather == WeatherState.Rainy;

        soilManager.ResetDailyWater();      // 1) Xóa trạng thái tưới hôm trước

        if (isRainy) soilManager.WaterAllAreas();  // 2) Mưa hôm nay -> tưới toàn bộ

        soilManager.WaterBySprinklers();    // 3) Máy tưới hoạt động trong ngày

        //tăng trưởng khi được tưới
        for (int x = 0; x < farmManager.gridWidth; x++)
        {
            for (int y = 0; y < farmManager.gridHeight; y++)
            {
                Tile tile = farmManager.Tiles[x, y];
                if (tile.plantInstance == null || tile.plantObject == null) continue;


                //Không tưới & không mưa KHÔNG phát triển
                bool watered = isRainy || soilManager.IsTileWatered(x, y);
                if (!watered) continue;

                var inst = tile.plantInstance;
                int last = GetLastStageIndexFor(inst);

                if (inst.currentStage < last)
                {
                    inst.daysInCurrentStage++;
                    int need = GetRequiredDaysForCurrentStage(inst);
                    if (inst.daysInCurrentStage >= need)
                    {
                        inst.currentStage++;
                        inst.daysInCurrentStage = 0;
                        ReplacePlantMeshAtCenter(x, y, inst);
                    }
                    Debug.Log("Qua ngày: tăng trưởng.");
                }
            }
        }
    }

    // ===== PLANTING =====

    //Có thể trồng không?
    public bool CanPlantAt(Vector2Int startPos, int size, PlantData plantData)
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                int cx = startPos.x + x, cy = startPos.y + y;
                if (!farmManager.IsInGrid(cx, cy)) return false;

                var t = farmManager.Tiles[cx, cy];
                if (t.state != SoilState.Dug || t.plantInstance != null) return false;

                bool isHole = (t.soilType == SoilType.Hole);
                if (!plantData.CanPlantOn(t.state, isHole)) return false;
            }
        return true;
    }

    //Hàm trồng cây
    private void PlantSeed(Vector2Int startPos, PlantData plantData)
    {
        int size = plantData.GetSizeInt();

        var inst = new PlantInstance(plantData) { currentStage = 0, daysInCurrentStage = 0 };

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                int tx = startPos.x + x, ty = startPos.y + y;
                if (!farmManager.IsInGrid(tx, ty)) continue;
                farmManager.Tiles[tx, ty].state = SoilState.Planted;
                farmManager.Tiles[tx, ty].plantInstance = inst;
            }

        GameObject stagePrefab = GetPrefabFor(inst, 0);

        int centerX = startPos.x + (size / 2);
        int centerY = startPos.y + (size / 2);

        if (stagePrefab && farmManager.IsInGrid(centerX, centerY))
        {
            if (farmManager.Tiles[centerX, centerY].plantObject)
                Destroy(farmManager.Tiles[centerX, centerY].plantObject);

            Vector3 plantPos = farmManager.origin + new Vector3(
                (startPos.x + (size * 0.5f)) * farmManager.cellSize,
                stagePrefab.transform.position.y,
                (startPos.y + (size * 0.5f)) * farmManager.cellSize
            );
            farmManager.Tiles[centerX, centerY].plantObject = Instantiate(stagePrefab, plantPos, RandomizeRotation());
            var clickable = farmManager.Tiles[centerX, centerY].plantObject.GetComponentInChildren<PlantClickable>();
            if (clickable != null)
                clickable.Init(farmManager, centerX, centerY);
        }

        _plantSaves.Add(new PlantSave
        {
            type = plantData.plantType,
            size = size,
            stage = 0,
            daysInStage = 0,
            centerX = centerX,
            centerY = centerY,
            harvestCount = 0
        });

        Debug.Log($"Đã trồng {plantData.plantName} tại ({startPos.x},{startPos.y}) size {size}");
    }

    // ===== Tile / Prefab helpers =====

    private bool TryGetPlantCenterFrom(int x, int y, out int cx, out int cy)
    {
        cx = cy = -1;
        var inst = farmManager.Tiles[x, y].plantInstance;
        if (inst == null) return false;

        int sx = Mathf.Max(0, x - 3), ex = Mathf.Min(farmManager.gridWidth - 1, x + 3);
        int sy = Mathf.Max(0, y - 3), ey = Mathf.Min(farmManager.gridHeight - 1, y + 3);
        for (int i = sx; i <= ex; i++)
            for (int j = sy; j <= ey; j++)
            {
                var t = farmManager.Tiles[i, j];
                if (t.plantInstance == inst && t.plantObject != null)
                { cx = i; cy = j; return true; }
            }
        return false;
    }

    private void ReplacePlantMeshAtCenter(int cx, int cy, PlantInstance inst)
    {
        var tile = farmManager.Tiles[cx, cy];
        if (tile.plantObject)
        {
            Vector3 basePos = tile.plantObject.transform.position;
            float yRot = tile.plantObject.transform.eulerAngles.y;
            Destroy(tile.plantObject);

            GameObject stagePrefab = GetPrefabFor(inst, inst.currentStage);
            if (stagePrefab)
            {
                float prefabY = stagePrefab.transform.position.y;
                Vector3 newPos = new Vector3(basePos.x, prefabY, basePos.z);
                tile.plantObject = Instantiate(stagePrefab, newPos, Quaternion.Euler(0f, yRot, 0f));
                var clickable = tile.plantObject.GetComponentInChildren<PlantClickable>();
                if (clickable != null)
                    clickable.Init(farmManager, cx, cy);
            }
        }
    }

    private void RemovePlantAtCenter(int cx, int cy)
    {
        var inst = farmManager.Tiles[cx, cy].plantInstance;
        if (inst == null) return;

        int size = inst.plantData.GetSizeInt();
        int startX = cx - size / 2;
        int startY = cy - size / 2;

        for (int dx = 0; dx < size; dx++)
            for (int dy = 0; dy < size; dy++)
            {
                int tx = startX + dx, ty = startY + dy;
                if (!farmManager.IsInGrid(tx, ty)) continue;

                if (farmManager.Tiles[tx, ty].state == SoilState.Planted)
                    farmManager.Tiles[tx, ty].state = SoilState.Dug;
                if (farmManager.Tiles[tx, ty].plantInstance == inst)
                    farmManager.Tiles[tx, ty].plantInstance = null;
            }

        if (farmManager.Tiles[cx, cy].plantObject)
        {
            Destroy(farmManager.Tiles[cx, cy].plantObject);
            farmManager.Tiles[cx, cy].plantObject = null;
        }
    }

    private int GetLastStageIndexFor(PlantInstance inst)
    {
        var pd = inst.plantData;
        bool mature = UseMatureChain(inst);

        if (!mature)
        {
            if (pd.growthPrefabs != null && pd.growthPrefabs.Length > 0)
                return pd.growthPrefabs.Length - 1;
        }
        else
        {
            if (pd.matureRegrowPrefabs != null && pd.matureRegrowPrefabs.Length > 0)
                return pd.matureRegrowPrefabs.Length - 1;
        }
        return 0;
    }

    private GameObject GetPrefabFor(PlantInstance inst, int stage)
    {
        var pd = inst.plantData;
        bool mature = UseMatureChain(inst);

        if (!mature)
        {
            if (pd.growthPrefabs != null && pd.growthPrefabs.Length > stage)
                return pd.growthPrefabs[stage];
            return (pd.growthPrefabs != null && pd.growthPrefabs.Length > 0) ? pd.growthPrefabs[0] : null;
        }
        else
        {
            if (pd.matureRegrowPrefabs != null && pd.matureRegrowPrefabs.Length > stage)
                return pd.matureRegrowPrefabs[stage];
            return (pd.matureRegrowPrefabs != null && pd.matureRegrowPrefabs.Length > 0) ? pd.matureRegrowPrefabs[0] : null;
        }
    }

    private int GetRequiredDaysForCurrentStage(PlantInstance inst)
    {
        return inst.plantData.GetRequiredDaysForStage(UseMatureChain(inst), inst.currentStage);
    }

    private bool UseMatureChain(PlantInstance inst)
    {
        return inst.plantData.HasMatureRegrowChain() && inst.harvestCount >= 1;
    }

    private bool HasMoreHarvests(PlantInstance inst)
    {
        int max = inst.plantData.maxHarvest;
        if (max < 0) return true;
        return inst.harvestCount < max;
    }

    private bool IsMature(PlantInstance inst) => inst.currentStage >= GetLastStageIndexFor(inst);

    //Nếu là khu vực đã đào 
    private bool AreaIsDug(Vector2Int start, int size)
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                int cx = start.x + x, cy = start.y + y;
                if (!farmManager.IsInGrid(cx, cy)) return false;
                if (farmManager.Tiles[cx, cy].state != SoilState.Dug) return false;
            }
        return true;
    }

    //Random xoay 
    private Quaternion RandomizeRotation()
    {
        float y = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, y, 0f);
    }

    public void ClearPlants()
    {
        for (int x = 0; x < farmManager.gridWidth; x++)
            for (int y = 0; y < farmManager.gridHeight; y++)
            {
                var t = farmManager.Tiles[x, y];
                if (t.plantObject) { GameObject.Destroy(t.plantObject); t.plantObject = null; }
                t.plantInstance = null;
                if (t.state == SoilState.Planted) t.state = SoilState.Dug;
            }
        _plantSaves.Clear();
    }

    public void AddPlantFromSave(PlantSave p)
    {
        var pd = farmManager.plantDatabase ? farmManager.plantDatabase.GetPlantData(p.type) : null;
        if (pd == null) return;

        int size = p.size;
        int startX = p.centerX - (size / 2);
        int startY = p.centerY - (size / 2);

        var inst = new PlantInstance(pd)
        {
            currentStage = p.stage,
            daysInCurrentStage = p.daysInStage,
            harvestCount = p.harvestCount
        };

        for (int dx = 0; dx < size; dx++)
            for (int dy = 0; dy < size; dy++)
            {
                int tx = startX + dx, ty = startY + dy;
                if (!farmManager.IsInGrid(tx, ty)) continue;
                farmManager.Tiles[tx, ty].state = SoilState.Planted;
                farmManager.Tiles[tx, ty].plantInstance = inst;
            }

        var prefab = GetPrefabFor(inst, p.stage);
        if (prefab && farmManager.IsInGrid(p.centerX, p.centerY))
        {
            Vector3 pos = farmManager.origin + new Vector3(
                (startX + size * 0.5f) * farmManager.cellSize, 0.45f,
                (startY + size * 0.5f) * farmManager.cellSize
            );
            if (farmManager.Tiles[p.centerX, p.centerY].plantObject)
                GameObject.Destroy(farmManager.Tiles[p.centerX, p.centerY].plantObject);
            farmManager.Tiles[p.centerX, p.centerY].plantObject = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        }

        _plantSaves.Add(p);
    }
}
