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

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPos = hit.point;
            Vector2Int gridPos = WorldToGrid(worldPos);

            if (IsInGrid(gridPos.x, gridPos.y))
            {
                Vector3 ghostPos = origin + new Vector3(gridPos.x * cellSize, 0.28f, gridPos.y * cellSize);
                ghostInstance.transform.position = ghostPos;
                ghostInstance.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    DigTile(gridPos.x, gridPos.y);
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
        if (tiles[x, y].isDug) return;

        tiles[x, y].isDug = true;

        // Hiển thị luống đất đã đào
        float dugYOffset = 0.28f; // cao hơn mặt đất một chút
        Vector3 pos = origin + new Vector3(x * cellSize, dugYOffset, y * cellSize);
        tileObjects[x, y] = Instantiate(dugSoilPrefab, pos, Quaternion.identity);
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

}
