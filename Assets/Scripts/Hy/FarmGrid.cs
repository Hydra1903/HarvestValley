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
                // Có thể khởi tạo prefab ô đất thường ở đây nếu muốn
            }
        }

        ghostPlotInstance = Instantiate(ghostPlotPrefab, Vector3.zero, Quaternion.identity);
        ghostPlotInstance.SetActive(false);
        ghostHoleInstance = Instantiate(ghostHolePrefab, Vector3.zero, Quaternion.identity);
        ghostHoleInstance.SetActive(false);

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
    }

    void HandleMouseInput()
    {
        // Ẩn cả hai ghost trước
        ghostPlotInstance.SetActive(false);
        ghostHoleInstance.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPos = hit.point;
            Vector2Int gridPos = WorldToGrid(worldPos);

            // Lấy thông tin tool hiện tại
            ToolInfo toolInfo = GetCurrentToolInfo();
            
            // Tìm vùng đặt
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
}
