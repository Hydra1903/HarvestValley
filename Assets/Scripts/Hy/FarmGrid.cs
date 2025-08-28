using nTools.PrefabPainter;
using System.Collections.Generic;
using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    [Header("Save/ID")]
    public string gridId = "";

    [Header("Kích thước")]
    public int gridWidth = 30; // 6*5
    public int gridHeight = 20; // 4*5
    public float cellSize = 1f;
    public Vector3 origin = Vector3.zero;

    [Header("Dirt System")]
    public GameObject dugSoilPrefab; // Luống
    public GameObject holePrefab; //Hố
    public GameObject waterPrefab; // Thảm nước

    [Header("Plant system")]
    public PlantDatabase plantDatabase;
    public InventoryItem currentItem;


    [Header("Input Filter")]
    [SerializeField] private LayerMask gridMask;

    [Header("Ghost system")]
    public Material ghostMaterial;
    private SimpleGhostManager simpleGhostManager;

    private Tile[,] tiles;

    public GameObject ghostPlotPrefab;
    private GameObject ghostPlotInstance;
    public GameObject ghostHolePrefab;
    private GameObject ghostHoleInstance;

    private List<AreaSave> _areaSaves = new();
    private List<PlantSave> _plantSaves = new();
    private List<GameObject> _areaObjects = new();

    [SerializeField] private HotBarUI _hotbarUI;

    void Start()
    {
        tiles = new Tile[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                tiles[x, y] = new Tile();
            }
        }

        ghostPlotInstance = Instantiate(ghostPlotPrefab, Vector3.zero, Quaternion.identity);
        ghostPlotInstance.SetActive(false);
        ghostHoleInstance = Instantiate(ghostHolePrefab, Vector3.zero, Quaternion.identity);
        ghostHoleInstance.SetActive(false);

        var ghostManagerObj = new GameObject($"SimpleGhostManager_{gridId}");
        simpleGhostManager = ghostManagerObj.AddComponent<SimpleGhostManager>();
        simpleGhostManager.Initialize(ghostMaterial);
    }

    void Update()
    {
        // LẤY MỚI currentItem NGAY ĐẦU FRAME
        currentItem = (_hotbarUI != null) ? _hotbarUI.currentItem : null;

        HandleMouseInput();

        _hotbarUI.UpdateAllSlots();

        if (Input.GetKeyDown(KeyCode.N))
        {
            AdvanceDay();
        }
    }

    private void Awake()
    {
        if (_hotbarUI == null)
        {
            _hotbarUI = FindFirstObjectByType<HotBarUI>();
            if (_hotbarUI == null)
            {
                Debug.LogError("[FarmGrid] Không tìm thấy HotBarUI. Hãy kéo thả vào Inspector hoặc đảm bảo có HotBarUI trong scene.");
            }
        }
    }

    void HandleMouseInput()
    {
        var item = currentItem;

        // 1) Raycast chỉ vào layer FarmGround
        var cam = Camera.main;
        if (!cam) { HideAllGhosts(); return; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, gridMask))
        {
            HideAllGhosts();
            return;
        }

        // 2) Chỉ xử lý nếu hit thuộc FarmGrid này
        var hitGrid = hit.collider.GetComponentInParent<FarmGrid>();
        if (hitGrid != this || !IsWorldPointInsideThisGrid(hit.point))
        {
            HideAllGhosts();
            return;
        }

        // 3) Hover đúng grid
        Vector3 worldPos = hit.point;
        Vector2Int gridPos = WorldToGrid(worldPos);

        // 1) Nếu current item là SEED
        if (item != null && item.itemData != null && item.itemData.itemType == ItemType.Seed && item.quantity > 0)
        {
            HideAllGhosts();

            PlantData pd = (plantDatabase != null) ? plantDatabase.GetPlantData(item.itemData.plantType) : null;
            if (pd != null)
            {
                int size = pd.GetSizeInt();

                Vector2Int seedStartPos;
                if (size == 3)
                {
                    // ✨ BẮT BUỘC snap vào block hố 3×3
                    if (!TrySnapStartToHole3x3(gridPos, out seedStartPos))
                    {
                        HideAllGhosts(); // không đứng trong hố 3×3 -> không hiển thị ghost
                        return;
                    }
                }
                else
                {
                    seedStartPos = CalculateStartPosition(gridPos, size);
                }

                if (CanPlantAt(seedStartPos, size, pd))
                {
                    ShowPlantGhostPreview(seedStartPos, pd);

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlantSeed(seedStartPos, pd);
                        if (_hotbarUI != null && _hotbarUI.hotbar != null)
                        {
                            _hotbarUI.hotbar.UseAndRemoveItem(_hotbarUI.valueScroll, 1);
                            _hotbarUI.UpdateAllSlots();
                        }
                    }
                }
            }
            return; 
        }

        // Nếu không có item/không phải seed mà lại null -> dừng
        if (item == null || item.itemData == null)
        {
            HideAllGhosts();
            return;
        }

        // 2) Hoe/Shovel (Đào đất)
        if (item.itemData.toolType == ToolType.Hoe || item.itemData.toolType == ToolType.Shovel)
        {
            HideAllGhosts();

            ToolInfo toolInfo = GetCurrentToolInfo();
            Vector2Int startPos = CalculateStartPosition(gridPos, toolInfo.size);

            if (CanPlaceSoil(startPos.x, startPos.y, toolInfo.size))
                ShowGhostPreview(startPos, toolInfo);
            else
                HideAllGhosts();

            if (Input.GetMouseButtonDown(0))
            {
                var expectedType = (toolInfo.size == 5) ? SoilType.Plot : SoilType.Hole;

                if (TryFindAreaContaining(gridPos.x, gridPos.y, out int idx))
                {
                    var a = _areaSaves[idx];
                    if (a.soilType == expectedType)
                    {
                        int digCost = (toolInfo.size == 5) ? 10 : 6;
                        if (Mp.Instance != null && Mp.Instance.mp < digCost)
                        {
                            Notification.Instance?.ShowNotification("Hết năng lượng!");
                            return;
                        }

                        if (FlattenAreaAt(gridPos)) // thành công thật sự
                        {
                            if (digCost > 0) Mp.Instance?.UseMp(digCost); // giờ mới trừ
                        }
                        return;
                    }
                }

                if (CanPlaceSoil(startPos.x, startPos.y, toolInfo.size))
                {
                    int digCost = (toolInfo.size == 5) ? 10 : 6;

                    // Chỉ kiểm tra đủ NL
                    if (Mp.Instance != null && Mp.Instance.mp < digCost)
                    {
                        Notification.Instance?.ShowNotification("Hết năng lượng!");
                        return;
                    }

                    PlaceArea(startPos.x, startPos.y, toolInfo.size, toolInfo.prefab);

                    // Thành công rồi mới trừ
                    if (digCost > 0) Mp.Instance?.UseMp(digCost);
                }
            }
            return;
        }


        // 3) Harvest
        if (item.itemData.toolType == ToolType.Harvest)
        {
            HideAllGhosts();
            if (Input.GetMouseButtonDown(0))
                HarvestAt(gridPos);
            return;
        }
    }

    // Tìm area chứa điểm (x,y) với loại đất chỉ định
    bool TryFindAreaContaining(int x, int y, SoilType requiredType, out AreaSave area, out int idx)
    {
        for (int i = 0; i < _areaSaves.Count; i++)
        {
            var a = _areaSaves[i];
            if (a.soilType != requiredType) continue;
            if (x >= a.startX && x < a.startX + a.size &&
                y >= a.startY && y < a.startY + a.size)
            {
                area = a; idx = i; return true;
            }
        }
        area = default; idx = -1; return false;
    }

    // Kiểm tra block bắt đầu tại (startX,startY) có trùng khít một area mong muốn không
    bool IsExactAreaMatch(int startX, int startY, int size, SoilType type)
    {
        for (int i = 0; i < _areaSaves.Count; i++)
        {
            var a = _areaSaves[i];
            if (a.soilType == type && a.size == size && a.startX == startX && a.startY == startY)
                return true;
        }
        return false;
    }

    // Snap startPos về đầu block hố 3×3 chứa con trỏ (nếu có)
    bool TrySnapStartToHole3x3(Vector2Int hoverPos, out Vector2Int snappedStart)
    {
        if (TryFindAreaContaining(hoverPos.x, hoverPos.y, SoilType.Hole, out var a, out _)
            && a.size == 3)
        {
            snappedStart = new Vector2Int(a.startX, a.startY);
            return true;
        }
        snappedStart = default;
        return false;
    }



    //////// CƠ CHẾ ĐẤT ///////

    //kiểm tra click
    private bool IsWorldPointInsideThisGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - origin; // dùng origin của từng grid
        return local.x >= 0 && local.z >= 0 &&
               local.x < gridWidth * cellSize &&
               local.z < gridHeight * cellSize;
    }

    //ẩn các ghost của đất và cây
    private void HideAllGhosts()
    {
        if (ghostPlotInstance) ghostPlotInstance.SetActive(false);
        if (ghostHoleInstance) ghostHoleInstance.SetActive(false);
        if (simpleGhostManager != null) simpleGhostManager.HideGhost();
    }   
    
    struct ToolInfo
    {
        public int size;
        public GameObject prefab;
        public GameObject ghost;
        public float offsetY;
        public float offsetX;
        public float offsetZ;
    }

    private ToolInfo GetCurrentToolInfo()
    {
        ToolInfo info = new ToolInfo();

        var item = currentItem;
        var toolType = (item != null && item.itemData != null) ? item.itemData.toolType : ToolType.None;

        if (toolType == ToolType.Hoe)
        {
            info.size = 5;
            info.prefab = dugSoilPrefab;
            info.ghost = ghostPlotInstance;
            info.offsetY = 0.235f;
            info.offsetX = -0.2f;
            info.offsetZ = 5f;
        }
        else // Shovel mặc định
        {
            info.size = 3;
            info.prefab = holePrefab;
            info.ghost = ghostHoleInstance;
            info.offsetY = 0.45f;
            info.offsetX = 1.5f;
            info.offsetZ = 1.5f;
        }

        return info;
    }

    Vector2Int CalculateStartPosition(Vector2Int gridPos, int size)
    {
        int startX = gridPos.x - (size / 2);
        int startY = gridPos.y - (size / 2);

        // Đảm bảo vùng không vượt ra ngoài lưới
        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;
        if (startX + size > gridWidth) startX = gridWidth - size;
        if (startY + size > gridHeight) startY = gridHeight - size;

        return new Vector2Int(startX, startY);
    }

    private void ShowGhostPreview(Vector2Int startPos, ToolInfo toolInfo)
    {
        if (toolInfo.ghost == null) return;

        Vector3 ghostPos = origin + new Vector3(
            (startPos.x + toolInfo.offsetX) * cellSize,
            toolInfo.offsetY,
            (startPos.y + toolInfo.offsetZ) * cellSize
        );

        toolInfo.ghost.transform.position = ghostPos;
        toolInfo.ghost.SetActive(true);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize);
        return new Vector2Int(x, y);
    }

    // Hàm làm ướt đất 
    public void WetTile(GameObject waterMatPrefab)
    {
        waterPrefab.SetActive(true);
    }

    bool IsInGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    //Hàm kiểm tra click ở vùng đã đào chưa
    bool TryFindAreaContaining(int x, int y, out int index)
    {
        for (int i = 0; i < _areaSaves.Count; i++)
        {
            var a = _areaSaves[i];
            if (x >= a.startX && x < a.startX + a.size &&
                y >= a.startY && y < a.startY + a.size)
            { index = i; return true; }
        }
        index = -1; return false;
    }

    // Hàm kiểm tra vùng hợp lệ
    bool CanPlaceSoil(int startX, int startY, int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int checkX = startX + x;
                int checkY = startY + y;
                // Nếu bất kỳ ô nào đã có luống hoặc hole (state == Dug) thì không cho đặt
                if (!IsInGrid(checkX, checkY) || tiles[checkX, checkY].state == SoilState.Dug)
                    return false;
            }
        }
        return true;
    }

    // Hàm đặt đất
    void PlaceArea(int startX, int startY, int size, GameObject prefab)
    {
        // 1) Đánh dấu tiles
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tiles[startX + x, startY + y].state = SoilState.Dug;
                tiles[startX + x, startY + y].soilType = (size == 5) ? SoilType.Plot : SoilType.Hole;
            }
        }

        // 2) Ghi record save vùng
        _areaSaves.Add(new AreaSave
        {
            startX = startX,
            startY = startY,
            size = size,
            soilType = (size == 5) ? SoilType.Plot : SoilType.Hole
        });

        // 3) Đặt prefab ở tâm vùng (ổn định với mọi cellSize/origin)
        float dugYOffset = (size == 5) ? 0.235f : 0.45f; // độ cao 5 là luống 3 là hố
        float offsetX = (size == 5) ? 5f : 1.5f; // trục x 5 là luống 3 là hố
        float offsetZ = (size == 5) ? -0.2f : 1.5f; // trục z 5 là luống 3 là hố
        Vector3 pos = origin + new Vector3(
            (startX + offsetX) * cellSize,
            dugYOffset,
            (startY + offsetZ) * cellSize
        );

        GameObject dirt;

        if (_hotbarUI.currentItem.itemData.toolType == ToolType.Shovel)
        {
            dirt = Instantiate(prefab, pos, RandomizeRotation());
        }
        else
        {
            dirt = Instantiate(prefab, pos, Quaternion.identity);
        }

        _areaObjects.Add(dirt);
    }

    //Hủy đất
    bool FlattenAreaAt(Vector2Int gridPos)
    {
        int x = gridPos.x, y = gridPos.y;
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight) return false;

        if (!TryFindAreaContaining(x, y, out int idx))
        {
            Debug.Log("Không có luống/hố để hủy tại đây."); return false;
        }

        var a = _areaSaves[idx];

        // 1) Chặn nếu có cây trong vùng
        for (int dx = 0; dx < a.size; dx++)
            for (int dy = 0; dy < a.size; dy++)
            {
                var t = tiles[a.startX + dx, a.startY + dy];
                if (t.plantInstance != null)
                {
                    Debug.LogWarning("Không thể hủy vì vùng đang có cây!");
                    return false;
                }
            }

        // 2) Reset về đất thường
        for (int dx = 0; dx < a.size; dx++)
            for (int dy = 0; dy < a.size; dy++)
            {
                var t = tiles[a.startX + dx, a.startY + dy];

                // Nếu bạn có overlay per-tile (nước/cỏ/đá) -> tắt ở đây (nếu dùng):
                // if (tileObjects[a.startX+dx, a.startY+dy]) tileObjects[a.startX+dx, a.startY+dy].SetActive(false);

                t.state = SoilState.Normal;
                t.soilType = SoilType.None;
            }

        // 3) Hủy prefab vùng
        if (idx >= 0 && idx < _areaObjects.Count && _areaObjects[idx] != null)
            Destroy(_areaObjects[idx]);

        // 4) Xóa record
        _areaSaves.RemoveAt(idx);
        if (idx >= 0 && idx < _areaObjects.Count)
            _areaObjects.RemoveAt(idx);

        Debug.Log($"Đã hủy {(a.size == 5 ? "luống 5x5" : "hố 3x3")} tại ({a.startX},{a.startY}).");
        return true;
    }


    // ===== PLANT SYSTEM =====

    private bool CanPlantAt(Vector2Int startPos, int size, PlantData plantData)
    {
        // ✨ Nếu là cây 3×3, yêu cầu phải trùng khít một block hố 3×3
        if (size == 3)
        {
            if (!IsExactAreaMatch(startPos.x, startPos.y, 3, SoilType.Hole))
                return false;
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int checkX = startPos.x + x;
                int checkY = startPos.y + y;

                if (!IsInGrid(checkX, checkY))
                    return false;

                Tile tile = tiles[checkX, checkY];

                // Ô phải đã đào và chưa có cây
                if (tile.state != SoilState.Dug || tile.plantInstance != null)
                    return false;

                // Logic dựa trên PlantData (giữ nguyên của bạn)
                bool isHole = (tiles[checkX, checkY].soilType == SoilType.Hole);
                if (!plantData.CanPlantOn(tile.state, isHole))
                    return false;
            }
        }
        return true;
    }

    private void ShowPlantGhostPreview(Vector2Int startPos, PlantData plantData)
    {
        if (simpleGhostManager == null || plantData == null) return;

        // Tính toán offset dựa trên kích thước
        float offsetX = plantData.GetSizeInt() * 0.5f;
        float offsetZ = plantData.GetSizeInt() * 0.5f;

        Vector3 ghostPos = origin + new Vector3(
            (startPos.x + offsetX) * cellSize,
            0.45f,
            (startPos.y + offsetZ) * cellSize
        );

        //hiển thị ghost
        simpleGhostManager.ShowGhost(plantData, ghostPos);
    }


    //Xoay prefab ngẫu nhiên theo trục Y 
    Quaternion RandomizeRotation()
    {
        float randomY = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, randomY, 0f);
    }


    //hàm trồng cây
    private void PlantSeed(Vector2Int startPos, PlantData plantData)
    {
        if (plantData == null)
        {
            Debug.LogWarning("PlantData null");
            return;
        }

        int size = plantData.GetSizeInt();

        // Tạo plant instance
        PlantInstance newPlantInstance = new PlantInstance(plantData);
        newPlantInstance.currentStage = 0;
        newPlantInstance.daysInCurrentStage = 0;

        // Gán plantInstance cho tất cả ô trong vùng
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int tileX = startPos.x + x;
                int tileY = startPos.y + y;
                if (IsInGrid(tileX, tileY))
                {
                    tiles[tileX, tileY].state = SoilState.Planted;
                    tiles[tileX, tileY].plantInstance = newPlantInstance;
                }
            }
        }

        GameObject stagePrefab = GetPrefabFor(newPlantInstance, 0);


        // Tính ô trung tâm (nơi đặt GameObject của cây)
        float prefabY = stagePrefab.transform.position.y;
        int centerX = startPos.x + (size / 2);
        int centerY = startPos.y + (size / 2);

        if (stagePrefab != null && IsInGrid(centerX, centerY))
        {
            if (tiles[centerX, centerY].plantObject != null)
                Destroy(tiles[centerX, centerY].plantObject);

            Vector3 plantPos = origin + new Vector3(
                (startPos.x + (size * 0.5f)) * cellSize,
                prefabY,
                (startPos.y + (size * 0.5f)) * cellSize
            );

            tiles[centerX, centerY].plantObject = Instantiate(stagePrefab, plantPos, RandomizeRotation());
        }


        Debug.Log($"Đã trồng {plantData.plantName} ({plantData.plantType}) tại ({startPos.x}, {startPos.y}) size {size}");

        // LƯU RECORD 
        _plantSaves.Add(new PlantSave
        {
            type = plantData.plantType,
            size = size,
            stage = 0,
            daysInStage = 0,
            centerX = centerX,
            centerY = centerY
        });
    }

    //Kiểm tra số lần thu hoạch
    bool HasMoreHarvests(PlantInstance inst)
    {
        int max = inst.plantData.maxHarvest;
        if (max < 0) return true;                // vô hạn
        return inst.harvestCount < max;          // còn lượt
    }

    //Kiểm tra đã trưởng thành chưa
    bool IsMature(PlantInstance inst)
    {
        return inst.currentStage >= GetLastStageIndexFor(inst);
    }

    bool TryGetPlantCenterFrom(int x, int y, out int cx, out int cy)
    {
        cx = cy = -1;
        var inst = tiles[x, y].plantInstance;
        if (inst == null) return false;

        int sx = Mathf.Max(0, x - 3), ex = Mathf.Min(gridWidth - 1, x + 3);
        int sy = Mathf.Max(0, y - 3), ey = Mathf.Min(gridHeight - 1, y + 3);
        for (int i = sx; i <= ex; i++)
            for (int j = sy; j <= ey; j++)
            {
                var t = tiles[i, j];
                if (t.plantInstance == inst && t.plantObject != null)
                {
                    cx = i; cy = j; return true;
                }
            }
        return false;
    }

    //hàm thu hoạch
    private void HarvestAt(Vector2Int gridPos)
    {
        int x = gridPos.x, y = gridPos.y;
        if (!IsInGrid(x, y)) return;

        var t = tiles[x, y];
        if (t.plantInstance == null) { Debug.Log("Không có cây."); return; }

        if (!TryGetPlantCenterFrom(x, y, out int cx, out int cy))
        {
            Debug.LogWarning("Không tìm được ô tâm."); return;
        }

        var inst = tiles[cx, cy].plantInstance;
        if (inst == null || inst.plantData == null) { Debug.LogWarning("Thiếu dữ liệu cây."); return; }
        if (!IsMature(inst)) { Debug.Log("Chưa chín."); return; }

        int cost = Mathf.Max(0, inst.plantData.energyHarvest);
        // 1) CHỈ kiểm tra đủ NL (không trừ)
        if (Mp.Instance != null && Mp.Instance.mp < cost)
        {
            Notification.Instance?.ShowNotification("Hết năng lượng!");
            return;
        }

        // 2) Thử add vào túi (một lần duy nhất)
        int yield = Mathf.Max(0, inst.plantData.harvestValue);
        if (yield <= 0 || inst.plantData.harvestItem == null)
        {
            Debug.LogWarning("[Harvest] Dữ liệu harvest không hợp lệ.");
            return;
        }
        if (!TryAddHarvestToInventory(inst.plantData, yield))
        {
            // Túi đầy -> KHÔNG trừ NL, KHÔNG đổi trạng thái cây
            Debug.LogWarning($"[Harvest] Túi đầy, không thu {yield} x {inst.plantData.harvestItem.itemName}");
            return;
        }

        // 3) AddItem đã thành công → giờ mới TRỪ NL (không đụng Inventory)
        if (cost > 0) Mp.Instance?.UseMp(cost);

        // 4) Cập nhật trạng thái cây + XP
        inst.harvestCount++;
        if (Xp.Instance != null) Xp.Instance.AddXp(Mathf.Max(0, inst.plantData.xpHarvest));
        Debug.Log(
            $"Thu hoạch {inst.plantData.plantName} (+{yield})  +{inst.plantData.xpHarvest} XP. " +
            $"Lần {inst.harvestCount}/{(inst.plantData.maxHarvest < 0 ? "∞" : inst.plantData.maxHarvest.ToString())}"
        );

        // 5) Regrow hay xóa
        if (HasMoreHarvests(inst))
        {
            if (UseMatureChain(inst))
            {
                inst.currentStage = 0;
                inst.daysInCurrentStage = 0;
                inst.daysUntilNextHarvest = 0;
                ReplacePlantMeshAtCenter(cx, cy, inst);
            }
            return;
        }

        RemovePlantAtCenter(cx, cy);
        int idx = _plantSaves.FindIndex(p => p.centerX == cx && p.centerY == cy);
        if (idx >= 0) _plantSaves.RemoveAt(idx);
    }

    //thêm vào túi đồ
    bool TryAddHarvestToInventory(PlantData pd, int amount)
    {
        if (pd == null || pd.harvestItem == null || amount <= 0) return false;
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("[Harvest] Inventory.Instance = null");
            return false;
        }

        // Thử thêm vào túi
        bool ok = Inventory.Instance.AddItem(pd.harvestItem, amount);
        if (!ok)
        {
            // Túi đầy => KHÔNG thu hoạch
            Debug.LogWarning($"[Harvest] Túi đầy, không thể thu {amount} x {pd.harvestItem.itemName}");
            return false;
        }

        // Thêm thành công => ok
        return true;
    }

    // đổi mesh theo inst.currentStage tại ô tâm
    void ReplacePlantMeshAtCenter(int cx, int cy, PlantInstance inst)
    {
        var tile = tiles[cx, cy];
        if (tile.plantObject != null)
        {
            // Giữ nguyên XZ, chỉ thay Y theo prefab stage mới
            Vector3 basePos = tile.plantObject.transform.position;
            float yRot = tile.plantObject.transform.eulerAngles.y;

            Destroy(tile.plantObject);

            GameObject stagePrefab = GetPrefabFor(inst, inst.currentStage);
            if (stagePrefab != null)
            {
                float prefabY = stagePrefab.transform.position.y;

                Vector3 newPos = new Vector3(
                    basePos.x,
                    prefabY,
                    basePos.z
                );

                tile.plantObject = Instantiate(stagePrefab, newPos, Quaternion.Euler(0f, yRot, 0f));
            }
        }
    }

    //xóa cây ở giữa tile
    void RemovePlantAtCenter(int cx, int cy)
    {
        var inst = tiles[cx, cy].plantInstance;
        if (inst == null) return;

        int size = inst.plantData.GetSizeInt();
        int startX = cx - size / 2;
        int startY = cy - size / 2;

        for (int dx = 0; dx < size; dx++)
            for (int dy = 0; dy < size; dy++)
            {
                int tx = startX + dx, ty = startY + dy;
                if (!IsInGrid(tx, ty)) continue;

                if (tiles[tx, ty].state == SoilState.Planted)
                    tiles[tx, ty].state = SoilState.Dug;
                if (tiles[tx, ty].plantInstance == inst)
                    tiles[tx, ty].plantInstance = null;
            }

        if (tiles[cx, cy].plantObject != null)
        {
            Destroy(tiles[cx, cy].plantObject);
            tiles[cx, cy].plantObject = null;
        }
    }

    //hàm cập nhật giai đoạn phát triển
    public void AdvanceDay()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = tiles[x, y];
                if (tile.plantInstance == null || tile.plantObject == null) continue;

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
                }
                // Đang ở stage cuối -> chờ thu hoạch
            }
        }

        Debug.Log("Qua ngày: tăng trưởng theo growth (lần đầu) hoặc mature-regrow (từ lần 2), không dùng cooldown.");
    }

    // Dùng mature chain nếu cây có matureRegrowPrefabs và đã thu ít nhất 1 lần
    private bool UseMatureChain(PlantInstance inst)
    {
        return inst.plantData.HasMatureRegrowChain() && inst.harvestCount >= 1;
    }

    int GetLastStageIndexFor(PlantInstance inst)
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

    private bool UseEnergy(int amount)
    {
        if (amount <= 0) return true;
        if (Mp.Instance.mp < amount)
        {
            Notification.Instance.ShowNotification("Hết năng lượng!");
            return false;
        }
        
        Mp.Instance.UseMp(amount); 
        return true;
    }

    /// ======SAVE DATA======

    void SyncPlantSavesFromWorld()
    {
        // Đồng bộ stage/daysInStage từ runtime về _plantSaves
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
            {
                var tile = tiles[x, y];
                if (tile.plantInstance != null && tile.plantObject != null)
                {
                    var inst = tile.plantInstance;
                    var rec = _plantSaves.Find(p => p.centerX == x && p.centerY == y);
                    if (rec != null)
                    {
                        rec.stage = inst.currentStage;
                        rec.daysInStage = inst.daysInCurrentStage;
                        rec.size = inst.plantData.GetSizeInt();
                        rec.type = inst.plantData.plantType;
                        rec.harvestCount = inst.harvestCount;
                    }
                }
            }
    }

    public FarmGridSave BuildSave()
    {
        // Đảm bảo stage mới nhất được ghi vào _plantSaves
        SyncPlantSavesFromWorld();

        return new FarmGridSave
        {
            gridId = gridId,
            width = gridWidth,
            height = gridHeight,
            cellSize = cellSize,
            origin = origin,
            areas = new List<AreaSave>(_areaSaves),
            plants = new List<PlantSave>(_plantSaves)
        };
    }

    public void LoadFromSave(FarmGridSave data)
    {
        //Apply dữ liệu
        gridWidth = data.width;
        gridHeight = data.height;
        cellSize = data.cellSize;
        origin = data.origin;

        //Reset lưới
        tiles = new Tile[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                tiles[x, y] = new Tile();

        //Dựng lại đất 
        _areaObjects = new List<GameObject>(data.areas.Count);
        foreach (var a in data.areas)
        {
            for (int dx = 0; dx < a.size; dx++)
                for (int dy = 0; dy < a.size; dy++)
                {
                    tiles[a.startX + dx, a.startY + dy].state = SoilState.Dug;
                    tiles[a.startX + dx, a.startY + dy].soilType = a.soilType;
                }

            GameObject prefab = (a.size == 5) ? dugSoilPrefab : holePrefab;

            float yOffset = (a.size == 5) ? 0.235f : 0.45f;
            float offsetX = (a.size == 5) ? 5f : 1.5f;
            float offsetZ = (a.size == 5) ? -0.2f : 1.5f;
            Vector3 pos = origin + new Vector3(
                (a.startX + offsetX) * cellSize,
                yOffset,
                (a.startY + offsetZ) * cellSize
            );
            GameObject go = null;
            if (prefab != null) go = Instantiate(prefab, pos, Quaternion.identity);
            _areaObjects.Add(go);
        }
        _areaSaves = new List<AreaSave>(data.areas);

        //Dựng lại cây
        _plantSaves = new List<PlantSave>(data.plants);
        foreach (var p in _plantSaves)
        {
            var plantData = plantDatabase != null ? plantDatabase.GetPlantData(p.type) : null;
            if (plantData == null) continue;

            int size = p.size;
            int startX = p.centerX - (size / 2);
            int startY = p.centerY - (size / 2);

            // Gắn PlantInstance cho cả vùng
            var inst = new PlantInstance(plantData)
            {
                currentStage = p.stage,
                daysInCurrentStage = p.daysInStage,
                harvestCount = p.harvestCount,
            };

            for (int dx = 0; dx < size; dx++)
                for (int dy = 0; dy < size; dy++)
                {
                    int tx = startX + dx;
                    int ty = startY + dy;
                    if (tx < 0 || ty < 0 || tx >= gridWidth || ty >= gridHeight) continue;
                    tiles[tx, ty].state = SoilState.Planted;
                    tiles[tx, ty].plantInstance = inst;
                }

            // Instantiate prefab đúng stage tại tâm
            GameObject stagePrefab = GetPrefabFor(inst, p.stage);

            if (stagePrefab != null && IsInGrid(p.centerX, p.centerY))
            {
                Vector3 pos = origin + new Vector3(
                    (startX + size * 0.5f) * cellSize,
                    0.45f,
                    (startY + size * 0.5f) * cellSize
                );
                if (tiles[p.centerX, p.centerY].plantObject != null)
                    Destroy(tiles[p.centerX, p.centerY].plantObject);

                tiles[p.centerX, p.centerY].plantObject = Instantiate(stagePrefab, pos, RandomizeRotation());
            }
        }
    }

    void OnDrawGizmos()
    {
        float gizmoYOffset = 0.3f;
        Gizmos.color = Color.red;
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 from = origin + new Vector3(x * cellSize, gizmoYOffset, 0);
            Vector3 to = origin + new Vector3(x * cellSize, gizmoYOffset, gridHeight * cellSize);
            Gizmos.DrawLine(from, to);
        }
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 from = origin + new Vector3(0, gizmoYOffset, y * cellSize);
            Vector3 to = origin + new Vector3(gridWidth * cellSize, gizmoYOffset, y * cellSize);
            Gizmos.DrawLine(from, to);
        }

        // Vẽ outline cho các vùng luống 5x5 và hố 3x3
        if (tiles != null)
        {
            // Đánh dấu các vùng đã vẽ để không vẽ trùng
            bool[,] visited = new bool[gridWidth, gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (!visited[x, y] && tiles[x, y].state == SoilState.Dug)
                    {
                        // Kiểm tra vùng 5x5
                        bool isPlot = true;
                        if (x + 4 < gridWidth && y + 4 < gridHeight)
                        {
                            for (int dx = 0; dx < 5; dx++)
                                for (int dy = 0; dy < 5; dy++)
                                    if (tiles[x + dx, y + dy].state != SoilState.Dug)
                                        isPlot = false;
                        }
                        else isPlot = false;

                        // Kiểm tra vùng 3x3 
                        bool isHole = false;
                        if (!isPlot && x + 2 < gridWidth && y + 2 < gridHeight)
                        {
                            isHole = true;
                            for (int dx = 0; dx < 3; dx++)
                                for (int dy = 0; dy < 3; dy++)
                                    if (tiles[x + dx, y + dy].state != SoilState.Dug)
                                        isHole = false;
                        }

                        if (isPlot)
                        {
                            Gizmos.color = Color.green;
                            Vector3 p1 = origin + new Vector3(x * cellSize, gizmoYOffset + 0.01f, y * cellSize);
                            Vector3 p2 = origin + new Vector3((x + 5) * cellSize, gizmoYOffset + 0.01f, y * cellSize);
                            Vector3 p3 = origin + new Vector3((x + 5) * cellSize, gizmoYOffset + 0.01f, (y + 5) * cellSize);
                            Vector3 p4 = origin + new Vector3(x * cellSize, gizmoYOffset + 0.01f, (y + 5) * cellSize);
                            Gizmos.DrawLine(p1, p2);
                            Gizmos.DrawLine(p2, p3);
                            Gizmos.DrawLine(p3, p4);
                            Gizmos.DrawLine(p4, p1);
                            // Đánh dấu đã vẽ vùng này
                            for (int dx = 0; dx < 5; dx++)
                                for (int dy = 0; dy < 5; dy++)
                                    visited[x + dx, y + dy] = true;
                        }
                        else if (isHole)
                        {
                            Gizmos.color = Color.blue;
                            Vector3 p1 = origin + new Vector3(x * cellSize, gizmoYOffset + 0.02f, y * cellSize);
                            Vector3 p2 = origin + new Vector3((x + 3) * cellSize, gizmoYOffset + 0.02f, y * cellSize);
                            Vector3 p3 = origin + new Vector3((x + 3) * cellSize, gizmoYOffset + 0.02f, (y + 3) * cellSize);
                            Vector3 p4 = origin + new Vector3(x * cellSize, gizmoYOffset + 0.02f, (y + 3) * cellSize);
                            Gizmos.DrawLine(p1, p2);
                            Gizmos.DrawLine(p2, p3);
                            Gizmos.DrawLine(p3, p4);
                            Gizmos.DrawLine(p4, p1);
                            // Đánh dấu đã vẽ vùng này
                            for (int dx = 0; dx < 3; dx++)
                                for (int dy = 0; dy < 3; dy++)
                                    visited[x + dx, y + dy] = true;
                        }
                    }
                }
            }
        }
    }

}

