using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    public int gridWidth = 30; // 6*5
    public int gridHeight = 20; // 4*5
    public float cellSize = 1f;
    public Vector3 origin = Vector3.zero;

    public GameObject dugSoilPrefab; // Luống
    public GameObject holePrefab; //Hố
    public GameObject waterPrefab; // Thảm nước
    
    // Plant system
    public PlantType currentPlantType = PlantType.Carrot;
    public PlantDatabase plantDatabase;
    
    // Optimized ghost system  
    public Material ghostMaterial;
    private SimpleGhostManager simpleGhostManager; 

    private Tile[,] tiles;
    private GameObject[,] tileObjects;

    public GameObject ghostPlotPrefab;
    private GameObject ghostPlotInstance;
    public GameObject ghostHolePrefab;
    private GameObject ghostHoleInstance;
    
    // Removed multiple ghost instances - now using PlantGhostManager


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

    void Update()
    {
        HandleToolSwitching();
        HandleMouseInput();
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
        // Ẩn tất cả ghost trước
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
                // Logic đào luống cũ
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

                        // Kiểm tra vùng 3x3 nếu không phải plot
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
        // Kiểm tra xem vùng này có phải là hố không
        // Logic này cần được cải thiện dựa trên cách bạn đánh dấu hố
        // Tạm thời return false để test
        return false;
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
        if (plantData == null || plantData.prefab == null)
        {
            Debug.LogWarning($"Không tìm thấy prefab cho {plantData?.plantName}");
            return;
        }
        
        int size = plantData.GetSizeInt();
        
        // Tạo plant instance
        PlantInstance newPlantInstance = new PlantInstance(plantData);
        
        // Cập nhật tiles
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int tileX = startPos.x + x;
                int tileY = startPos.y + y;
                tiles[tileX, tileY].state = SoilState.Planted;
                tiles[tileX, tileY].plantInstance = newPlantInstance;
            }
        }
        
        // Tạo GameObject trong scene
        float offsetX = size * 0.5f;
        float offsetZ = size * 0.5f;
        
        Vector3 plantPos = origin + new Vector3(
            (startPos.x + offsetX) * cellSize,
            0.45f,
            (startPos.y + offsetZ) * cellSize
        );
        
        GameObject plantObject = Instantiate(plantData.prefab, plantPos, Quaternion.identity);
        
        // Lưu reference vào tile trung tâm
        int centerX = startPos.x + size / 1;
        int centerY = startPos.y + size / 1;
        tiles[centerX, centerY].plantObject = plantObject;
        
        Debug.Log($"Đã trồng {plantData.plantName} ({plantData.plantType}) tại ({startPos.x}, {startPos.y})");
    }
}
