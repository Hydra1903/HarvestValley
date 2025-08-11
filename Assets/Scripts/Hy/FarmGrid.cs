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
    public PlantType currentPlantType = PlantType.Carrot;
    public PlantDatabase plantDatabase;

    [Header("Ghost system")]
    public Material ghostMaterial;
    private SimpleGhostManager simpleGhostManager; 

    private Tile[,] tiles;
    private GameObject[,] tileObjects;

    public GameObject ghostPlotPrefab;
    private GameObject ghostPlotInstance;
    public GameObject ghostHolePrefab;
    private GameObject ghostHoleInstance;
    
    public ToolType currentTool = ToolType.Hoe;

    void Start()
    {
        tiles = new Tile[gridWidth, gridHeight];
        tileObjects = new GameObject[gridWidth, gridHeight];

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
        
        // Initialize simple ghost manager
        GameObject ghostManagerObj = new GameObject("SimpleGhostManager");
        simpleGhostManager = ghostManagerObj.AddComponent<SimpleGhostManager>();
        simpleGhostManager.Initialize(ghostMaterial);

    }

    void OnEnable()
    {
        SaveLoadManager.RegisterGrid(this);
    }

    void OnDisable()
    {
        SaveLoadManager.UnregisterGrid(this);
    }

    void Update()
    {
        HandleToolSwitching();
        HandleMouseInput();

        if (Input.GetKeyDown(KeyCode.N)) 
        {
            AdvanceDay();
        }
    }

    void HandleToolSwitching()
    {
        // Chuyển công cụ
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = ToolType.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = ToolType.Shovel;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentTool = ToolType.Seed;
        
        // Chuyển loại cây khi đang dùng seed
        if (currentTool == ToolType.Seed)
        {
            if (Input.GetKeyDown(KeyCode.Q)) currentPlantType = PlantType.Carrot;  // 1x1
            if (Input.GetKeyDown(KeyCode.W)) currentPlantType = PlantType.Tomato;  // 2x2
            if (Input.GetKeyDown(KeyCode.E)) currentPlantType = PlantType.Apple;   // 3x3
        }
    }

    void HandleMouseInput()
    {
        ghostPlotInstance.SetActive(false);
        ghostHoleInstance.SetActive(false);
        if (simpleGhostManager != null)
        {
            simpleGhostManager.HideGhost();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPos = hit.point;
            Vector2Int gridPos = WorldToGrid(worldPos);

            if (currentTool == ToolType.Seed)
            {
                HandlePlantingInput(gridPos);
            }
            else
            {
                // Logic đào luống 
                ToolInfo toolInfo = GetCurrentToolInfo();
                Vector2Int startPos = CalculateStartPosition(gridPos, toolInfo.size);

                if (CanPlaceSoil(startPos.x, startPos.y, toolInfo.size))
                {
                    ShowGhostPreview(startPos, toolInfo);

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceArea(startPos.x, startPos.y, toolInfo.size, toolInfo.prefab);
                    }
                }
            }
        }
    }

    struct ToolInfo
    {
        public int size;
        public GameObject prefab;
        public GameObject ghost;
        public float offsetX;
        public float offsetZ;
    }

    ToolInfo GetCurrentToolInfo()
    {
        ToolInfo info = new ToolInfo();
        
        if (currentTool == ToolType.Hoe)
        {
            info.size = 5;
            info.prefab = dugSoilPrefab;
            info.ghost = ghostPlotInstance;
            info.offsetX = 0f;
            info.offsetZ = 5f;
        }
        else // Shovel
        {
            info.size = 3;
            info.prefab = holePrefab;
            info.ghost = ghostHoleInstance;
            info.offsetX = 0.8f;
            info.offsetZ = 2.7f;
        }
        
        return info;
    }

    Vector2Int CalculateStartPosition(Vector2Int gridPos, int size)
    {
        // Tìm góc trên-trái của vùng
        int startX = gridPos.x - (size / 2);
        int startY = gridPos.y - (size / 2);

        // Đảm bảo vùng không vượt ra ngoài lưới
        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;
        if (startX + size > gridWidth) startX = gridWidth - size;
        if (startY + size > gridHeight) startY = gridHeight - size;

        return new Vector2Int(startX, startY);
    }

    void ShowGhostPreview(Vector2Int startPos, ToolInfo toolInfo)
    {
        Vector3 ghostPos = origin + new Vector3(
            (startPos.x + toolInfo.offsetX) * cellSize,
            0.28f,
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

    public void DigTile(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return;
        if (tiles[x, y].state != SoilState.Normal) return;

        tiles[x, y].state = SoilState.Dug;

        // Hiển thị luống đất đã đào
        float dugYOffset = 0.28f; // cao hơn mặt đất một chút
        Vector3 pos = origin + new Vector3((x + 0.5f) * cellSize, dugYOffset, (y + 0.5f) * cellSize);
        tileObjects[x, y] = Instantiate(dugSoilPrefab, pos, Quaternion.identity);
    }

    // Hàm làm ướt đất (ví dụ gọi khi tưới nước)
    public void WetTile(GameObject waterMatPrefab)
    {
        waterPrefab.SetActive(true);
    }

    void OnDrawGizmos()
    {
        float gizmoYOffset = 0.7f; // cao hơn mặt đất một chút
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


    bool IsInGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
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

    // Hàm đặt đất tổng quát
    void PlaceArea(int startX, int startY, int size, GameObject prefab)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tiles[startX + x, startY + y].state = SoilState.Dug;
                tiles[startX + x, startY + y].soilType = (size == 5) ? SoilType.Plot : SoilType.Hole;
            }
        }
        float dugYOffset = 0.28f;
        // Điều chỉnh offset riêng cho X và Z
        float offsetX = (size == 5) ? 5f : 0.8f; // 5x5 luống hoặc 3x3 hố
        float offsetZ = (size == 5) ? -0.5f : 2.7f;
        
        Vector3 pos = origin + new Vector3(
            (startX + offsetX) * cellSize,
            dugYOffset,
            (startY + offsetZ) * cellSize
        );
        Instantiate(prefab, pos, Quaternion.identity);
    }
    
    // ===== PLANT SYSTEM =====
    
    void HandlePlantingInput(Vector2Int gridPos)
    {
        if (plantDatabase == null) return;
        
        PlantData plantData = plantDatabase.GetPlantData(currentPlantType);
        if (plantData == null) return;
        
        int size = plantData.GetSizeInt();
        Vector2Int startPos = CalculateStartPosition(gridPos, size);
        
        if (CanPlantAt(startPos, size, plantData))
        {
            ShowPlantGhostPreview(startPos, plantData);
            
            if (Input.GetMouseButtonDown(0))
            {
                PlantSeed(startPos, plantData);
            }
        }
    }
    
    bool CanPlantAt(Vector2Int startPos, int size, PlantData plantData)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int checkX = startPos.x + x;
                int checkY = startPos.y + y;
                
                if (!IsInGrid(checkX, checkY))
                    return false;
                    
                Tile tile = tiles[checkX, checkY];
                
                // Kiểm tra tile có thể trồng cây không
                if (tile.state != SoilState.Dug || tile.plantInstance != null)
                    return false;
                    
                // Sử dụng logic từ PlantData để kiểm tra
                bool isHole = IsHoleArea(new Vector2Int(checkX, checkY), 1);
                if (!plantData.CanPlantOn(tile.state, isHole))
                    return false;
            }
        }
        return true;
    }

    bool IsHoleArea(Vector2Int pos, int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int checkX = pos.x + x;
                int checkY = pos.y + y;

                if (!IsInGrid(checkX, checkY))
                    return false;

                if (tiles[checkX, checkY].soilType != SoilType.Hole)
                    return false;
            }
        }
        return true;
    }


    void ShowPlantGhostPreview(Vector2Int startPos, PlantData plantData)
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
        
        // Sử dụng SimpleGhostManager để hiển thị ghost
        simpleGhostManager.ShowGhost(plantData, ghostPos);
    }

    void PlantSeed(Vector2Int startPos, PlantData plantData)
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

        // Tính ô trung tâm (nơi đặt GameObject của cây)
        int centerX = startPos.x + (size / 2);
        int centerY = startPos.y + (size / 2);

        Vector3 plantPos = origin + new Vector3(
            (startPos.x + (size * 0.5f)) * cellSize,
            0.45f,
            (startPos.y + (size * 0.5f)) * cellSize
        );

        // Chọn prefab stage 0 nếu có, fallback về plantData.prefab
        GameObject stagePrefab = null;
        if (plantData.growthPrefabs != null && plantData.growthPrefabs.Length > 0)
            stagePrefab = plantData.growthPrefabs[0];
        else
            stagePrefab = plantData.prefab;

        if (stagePrefab != null && IsInGrid(centerX, centerY))
        {
            // Xóa nếu đã có object trước đó ở center (an toàn)
            if (tiles[centerX, centerY].plantObject != null)
                Destroy(tiles[centerX, centerY].plantObject);

            tiles[centerX, centerY].plantObject = Instantiate(stagePrefab, plantPos, Quaternion.identity);
        }

        Debug.Log($"Đã trồng {plantData.plantName} ({plantData.plantType}) tại ({startPos.x}, {startPos.y}) size {size}");
    }

    //hàm cập nhật giai đoạn phát triển
    public void AdvanceDay()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = tiles[x, y];
                // Chỉ cập nhật khi ô này là ô chứa plantObject (ô trung tâm)
                if (tile.plantInstance != null && tile.plantObject != null)
                {
                    PlantInstance plant = tile.plantInstance;
                    int prevStage = plant.currentStage;


                    plant.AdvanceDay();

                    // Nếu stage thay đổi thì update GameObject
                    if (plant.currentStage != prevStage)
                    {
                        Vector3 pos = tile.plantObject.transform.position;
                        Destroy(tile.plantObject);

                        GameObject stagePrefab = null;
                        if (plant.plantData.growthPrefabs != null && plant.plantData.growthPrefabs.Length > plant.currentStage)
                            stagePrefab = plant.plantData.growthPrefabs[plant.currentStage];
                        else
                            stagePrefab = plant.plantData.prefab;

                        if (stagePrefab != null)
                        {
                            tile.plantObject = Instantiate(stagePrefab, pos, Quaternion.identity);
                        }
                        else
                        {
                            tile.plantObject = null;
                        }
                    }
                }
            }
        }

        Debug.Log("Qua ngày: Tất cả cây trung tâm đã được cập nhật.");
    }

    // ---------- Save/Load helpers (uses classes inside SaveLoadManager) ----------
    public SaveLoadManager.FarmGridSaveData ToSaveData()
    {
        var fsd = new SaveLoadManager.FarmGridSaveData();
        fsd.gridId = gridId;
        fsd.originX = origin.x;
        fsd.originZ = origin.z;
        fsd.width = gridWidth;
        fsd.height = gridHeight;
        fsd.cellSize = cellSize;

        fsd.tiles = new SaveLoadManager.TileSaveData[gridWidth * gridHeight];
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
            {
                int idx = y * gridWidth + x;
                fsd.tiles[idx] = new SaveLoadManager.TileSaveData
                {
                    state = tiles[x, y].state,
                    soilType = tiles[x, y].soilType
                };
            }

        // Lưu cây dựa trên ô center (tile.plantObject != null)
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
            {
                var t = tiles[x, y];
                if (t.plantInstance != null && t.plantObject != null)
                {
                    int size = t.plantInstance.plantData.GetSizeInt();
                    int startX = x - (size / 2);
                    int startY = y - (size / 2);

                    var ps = new SaveLoadManager.PlantSaveData
                    {
                        startX = startX,
                        startY = startY,
                        size = size,
                        plantType = t.plantInstance.plantData.plantType,
                        currentStage = t.plantInstance.currentStage,
                        daysInCurrentStage = t.plantInstance.daysInCurrentStage,
                        harvestCount = t.plantInstance.harvestCount
                    };

                    fsd.plants.Add(ps);
                }
            }

        return fsd;
    }

    public void LoadFromSaveData(SaveLoadManager.FarmGridSaveData fsd)
    {
        if (fsd.width != gridWidth || fsd.height != gridHeight || Mathf.Abs(fsd.cellSize - cellSize) > 0.001f)
        {
            Debug.LogWarning($"Saved grid ({fsd.gridId}) size differs from current FarmGrid. Saved: {fsd.width}x{fsd.height}@{fsd.cellSize} - Current: {gridWidth}x{gridHeight}@{cellSize}");
        }

        // Clear existing plantInstances & objects
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
            {
                tiles[x, y].plantInstance = null;
                if (tiles[x, y].plantObject != null)
                {
                    Destroy(tiles[x, y].plantObject);
                    tiles[x, y].plantObject = null;
                }
            }

        // Load tiles
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
            {
                int idx = y * gridWidth + x;
                if (idx < fsd.tiles.Length)
                {
                    tiles[x, y].state = fsd.tiles[idx].state;
                    tiles[x, y].soilType = fsd.tiles[idx].soilType;
                }
                else
                {
                    tiles[x, y].state = SoilState.Normal;
                    tiles[x, y].soilType = SoilType.None;
                }
            }

        // Restore plants
        foreach (var ps in fsd.plants)
        {
            PlantData data = plantDatabase != null ? plantDatabase.GetPlantData(ps.plantType) : null;
            if (data == null)
            {
                Debug.LogWarning($"Missing PlantData for {ps.plantType} when loading grid {gridId}");
                continue;
            }

            PlantInstance inst = new PlantInstance(data);
            inst.currentStage = ps.currentStage;
            inst.daysInCurrentStage = ps.daysInCurrentStage;
            inst.harvestCount = ps.harvestCount;

            // assign instance to all tiles in region
            for (int dx = 0; dx < ps.size; dx++)
                for (int dy = 0; dy < ps.size; dy++)
                {
                    int tx = ps.startX + dx;
                    int ty = ps.startY + dy;
                    if (IsInGrid(tx, ty))
                    {
                        tiles[tx, ty].plantInstance = inst;
                        tiles[tx, ty].state = SoilState.Planted;
                    }
                }

            // instantiate center prefab
            int centerX = ps.startX + (ps.size / 2);
            int centerY = ps.startY + (ps.size / 2);
            if (IsInGrid(centerX, centerY))
            {
                Vector3 pos = origin + new Vector3((ps.startX + ps.size * 0.5f) * cellSize, 0.45f, (ps.startY + ps.size * 0.5f) * cellSize);
                GameObject prefab = (data.growthPrefabs != null && data.growthPrefabs.Length > inst.currentStage) ? data.growthPrefabs[inst.currentStage] : data.prefab;
                if (prefab != null)
                {
                    tiles[centerX, centerY].plantObject = Instantiate(prefab, pos, Quaternion.identity);
                }
            }
        }
    }
}

