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
        // Chuyển công cụ
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = ToolType.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = ToolType.Shovel;

        // Ẩn cả hai ghost trước
        ghostPlotInstance.SetActive(false);
        ghostHoleInstance.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPos = hit.point;
            Vector2Int gridPos = WorldToGrid(worldPos);

            int size = (currentTool == ToolType.Hoe) ? 5 : 3;
            GameObject prefab = (currentTool == ToolType.Hoe) ? dugSoilPrefab : holePrefab;
            GameObject ghost = (currentTool == ToolType.Hoe) ? ghostPlotInstance : ghostHoleInstance;

            // Tìm góc trên-trái của vùng
            int startX = gridPos.x - (size / 2);
            int startY = gridPos.y - (size / 2);

            // Đảm bảo vùng không vượt ra ngoài lưới
            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;
            if (startX + size > gridWidth) startX = gridWidth - size;
            if (startY + size > gridHeight) startY = gridHeight - size;

            if (CanPlaceSoil(startX, startY, size))
            {
                // Nếu pivot ở giữa
                Vector3 ghostPos = origin + new Vector3(
                    (startX + (size - 1) / 2f) * cellSize,
                    0.28f,
                    (startY + (size - 1) / 2f) * cellSize
                );
                ghost.transform.position = ghostPos;
                ghost.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    PlaceArea(startX, startY, size, prefab);
                }
            }
        }
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
        Vector3 pos = origin + new Vector3(x * cellSize, dugYOffset, y * cellSize);
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
        Vector3 pos = origin + new Vector3(
            (startX + (size - 1) / 2f) * cellSize,
            dugYOffset,
            (startY + (size - 1) / 2f) * cellSize
        );
        Instantiate(prefab, pos, Quaternion.identity);
    }
}
