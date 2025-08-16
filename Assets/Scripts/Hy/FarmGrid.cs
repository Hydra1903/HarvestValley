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
    public PlantType currentPlantType = PlantType.Carrot;
    public PlantDatabase plantDatabase;

    [Header("Input Filter")]
    [SerializeField] private LayerMask gridMask; 

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

    private List<AreaSave> _areaSaves = new();
    private List<PlantSave> _plantSaves = new();


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

        var ghostManagerObj = new GameObject($"SimpleGhostManager_{gridId}");
        simpleGhostManager = ghostManagerObj.AddComponent<SimpleGhostManager>();
        simpleGhostManager.Initialize(ghostMaterial);
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
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = ToolType.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = ToolType.Shovel;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentTool = ToolType.Seed;
        
        if (currentTool == ToolType.Seed)
        {
            if (Input.GetKeyDown(KeyCode.Q)) currentPlantType = PlantType.Carrot;  // 1x1
            if (Input.GetKeyDown(KeyCode.W)) currentPlantType = PlantType.Tomato;  // 2x2
            if (Input.GetKeyDown(KeyCode.E)) currentPlantType = PlantType.Apple;   // 3x3
        }
    }

    void HandleMouseInput()
    {
        // 1) Raycast chỉ vào layer FarmGround
        var cam = Camera.main;
        if (!cam) { HideGhosts(); return; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, gridMask))
        {
            HideGhosts();
            return;
        }

        // 2) Chỉ xử lý nếu hit thuộc FarmGrid này
        var hitGrid = hit.collider.GetComponentInParent<FarmGrid>();
        if (hitGrid != this)
        {
            HideGhosts();
            return;
        }

        //check thêm biên theo origin/size để chắc chắn
        if (!IsWorldPointInsideThisGrid(hit.point))
        {
            HideGhosts();
            return;
        }

        // 3) Từ đây là đang hover đúng grid này → hiện ghost + cho click
        Vector3 worldPos = hit.point;
        Vector2Int gridPos = WorldToGrid(worldPos);

        if (currentTool == ToolType.Seed)
        {
            HandlePlantingInput(gridPos); // logic cũ
        }
        else
        {
            // Đào luống/hố — logic cũ
            ToolInfo toolInfo = GetCurrentToolInfo();
            Vector2Int startPos = CalculateStartPosition(gridPos, toolInfo.size);

            if (CanPlaceSoil(startPos.x, startPos.y, toolInfo.size))
            {
                ShowGhostPreview(startPos, toolInfo);
                if (Input.GetMouseButtonDown(0))
                    PlaceArea(startPos.x, startPos.y, toolInfo.size, toolInfo.prefab);
            }
            else
            {
                HideGhosts();
            }
        }
    }

    //kiểm tra click
    bool IsWorldPointInsideThisGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - origin; // dùng origin của từng grid
        return local.x >= 0 && local.z >= 0 &&
               local.x < gridWidth * cellSize &&
               local.z < gridHeight * cellSize;
    }

    //ẩn các ghost của đất và cây
    void HideGhosts()
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

        float dugYOffset = 0.28f; 
        Vector3 pos = origin + new Vector3((x + 0.5f) * cellSize, dugYOffset, (y + 0.5f) * cellSize);
        tileObjects[x, y] = Instantiate(dugSoilPrefab, pos, Quaternion.identity);
    }

    // Hàm làm ướt đất 
    public void WetTile(GameObject waterMatPrefab)
    {
        waterPrefab.SetActive(true);
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
        float dugYOffset = 0.24f;
        float offsetX = (size == 5) ? 5f : 0.8f; // 5x5 luống hoặc 3x3 hố
        float offsetZ = (size == 5) ? -0.3f : 2.7f;
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
        
        //hiển thị ghost
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

        GameObject stagePrefab = null;
        if (plantData.growthPrefabs != null && plantData.growthPrefabs.Length > 0)
            stagePrefab = plantData.growthPrefabs[0];
        else
            stagePrefab = plantData.prefab;

        if (stagePrefab != null && IsInGrid(centerX, centerY))
        {
            if (tiles[centerX, centerY].plantObject != null)
                Destroy(tiles[centerX, centerY].plantObject);

            tiles[centerX, centerY].plantObject = Instantiate(stagePrefab, plantPos, Quaternion.identity);
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

    //hàm cập nhật giai đoạn phát triển
    public void AdvanceDay()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = tiles[x, y];
                if (tile.plantInstance != null && tile.plantObject != null)
                {
                    PlantInstance plant = tile.plantInstance;
                    int prevStage = plant.currentStage;


                    plant.AdvanceDay();

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
        foreach (var a in data.areas)
        {
            for (int dx = 0; dx < a.size; dx++)
                for (int dy = 0; dy < a.size; dy++)
                {
                    tiles[a.startX + dx, a.startY + dy].state = SoilState.Dug;
                    tiles[a.startX + dx, a.startY + dy].soilType = a.soilType;
                }

            GameObject prefab = (a.size == 5) ? dugSoilPrefab : holePrefab;

            float yOffset = 0.28f;
            float offsetX = (a.size == 5) ? 5.1f : 0.8f; // 5x5 luống hoặc 3x3 hố
            float offsetZ = (a.size == 5) ? -0.5f : 2.7f;
            Vector3 pos = origin + new Vector3(
                (a.startX + offsetX) * cellSize,
                yOffset,
                (a.startY + offsetZ) * cellSize
            );
            if (prefab != null) Instantiate(prefab, pos, Quaternion.identity);
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
                daysInCurrentStage = p.daysInStage
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
            GameObject stagePrefab = null;
            if (plantData.growthPrefabs != null && plantData.growthPrefabs.Length > p.stage)
                stagePrefab = plantData.growthPrefabs[p.stage];
            else
                stagePrefab = plantData.prefab;

            if (stagePrefab != null && IsInGrid(p.centerX, p.centerY))
            {
                Vector3 pos = origin + new Vector3(
                    (startX + size * 0.5f) * cellSize,
                    0.45f,
                    (startY + size * 0.5f) * cellSize
                );

                if (tiles[p.centerX, p.centerY].plantObject != null)
                    Destroy(tiles[p.centerX, p.centerY].plantObject);

                tiles[p.centerX, p.centerY].plantObject = Instantiate(stagePrefab, pos, Quaternion.identity);
            }
        }
    }
}

