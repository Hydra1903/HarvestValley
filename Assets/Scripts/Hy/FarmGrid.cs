using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    public int gridWidth = 30; // 6*5
    public int gridHeight = 20; // 4*5
    public float cellSize = 1f;
    public Vector3 origin = Vector3.zero;
    public GameObject dugSoilPrefab; // Prefab luống đất đã đào

    private Tile[,] tiles;
    private GameObject[,] tileObjects;

    public GameObject ghostPrefab;
    private GameObject ghostInstance;
    public GameObject waterPrefab; // Thảm nước 

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

        ghostInstance = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        ghostInstance.SetActive(false);

    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            WetTile(waterPrefab);
        }
        

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPos = hit.point;
            Vector2Int gridPos = WorldToGrid(worldPos);

            // Tìm vị trí gốc của luống 5x5 (căn về phía trên/trái)
            int startX = gridPos.x - (gridPos.x % 5);
            int startY = gridPos.y - (gridPos.y % 5);

            if (CanPlacePlot(startX, startY))
            {
                Vector3 ghostPos = origin + new Vector3(startX * cellSize, 0.28f, startY * cellSize);
                ghostInstance.transform.position = ghostPos;
                ghostInstance.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    PlacePlot(startX, startY);
                }
            }
            else
            {
                ghostInstance.SetActive(false);
            }
        }
        else
        {
            ghostInstance.SetActive(false);
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

    bool CanPlacePlot(int startX, int startY)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                int checkX = startX + x;
                int checkY = startY + y;
                if (!IsInGrid(checkX, checkY) || tiles[checkX, checkY].state == SoilState.Dug)
                    return false;
            }
        }
        return true;
    }

    void PlacePlot(int startX, int startY)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                tiles[startX + x, startY + y].state = SoilState.Dug;
            }
        }
        // Hiển thị luống đất đã đào (1 prefab lớn)
        float dugYOffset = 0.28f;
        Vector3 pos = origin + new Vector3(startX * cellSize, dugYOffset, startY * cellSize);
        tileObjects[startX, startY] = Instantiate(dugSoilPrefab, pos, Quaternion.identity);
    }

}
