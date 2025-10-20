using UnityEngine;

public class FarmManager : MonoBehaviour
{
    [Header("Save/ID")]
    public string gridId = "";

    [Header("Kích thước")]
    public int gridWidth = 30;
    public int gridHeight = 20;
    public float cellSize = 1f;
    public Vector3 origin = Vector3.zero;

    [Header("Database & UI")]
    public PlantDatabase plantDatabase;
    public HotBarUI hotbarUI;

    public Tile[,] Tiles { get; private set; }
    private FarmSaveSystem _save;

    public SoilManager soilManager;
    public PlantManager plantManager;
    public FarmInput farmInputManager;

    public FarmGridSave BuildSave() => _save.BuildSave();
    public void LoadFromSave(FarmGridSave s) => _save.LoadFromSave(s);

    private void Awake()
    {
        if (!hotbarUI) hotbarUI = FindFirstObjectByType<HotBarUI>();
        if (!plantDatabase) plantDatabase = FindFirstObjectByType<PlantDatabase>();
    }

    private void Start()
    {
        AllocateTiles(gridWidth, gridHeight);
        soilManager.Initialize(this);
        plantManager.Initialize(this, soilManager);

        _save = GetComponent<FarmSaveSystem>();
        if (_save == null) _save = gameObject.AddComponent<FarmSaveSystem>();
        _save.Initialize(this);
        _save.soilManager = soilManager;
        _save.plantManager = plantManager;
    }

    private void Update()
    {
        farmInputManager.HandleInput();

        if (Input.GetKeyDown(KeyCode.N))
        {
            plantManager.AdvanceDay();
            GameTime.Instance.NextDay();
        }

        if(Weather.Instance.currentWeather == WeatherState.Rainy || Weather.Instance.currentWeather == WeatherState.Stormy)
        {
            soilManager.WaterAllAreas();
        }
    }

    // === Helpers chung ===

    //Tạo mảng Tile theo kích thước
    public void AllocateTiles(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;

        Tiles = new Tile[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                Tiles[x, y] = new Tile();
    }

    //Tọa độ lưới
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize);
        return new Vector2Int(x, y);
    }

    //Kiểm tra có nằm trong luoi
    public bool IsInGrid(int x, int y) => x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;

    public bool IsWorldPointInsideThisGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - origin;
        return local.x >= 0 && local.z >= 0 &&
               local.x < gridWidth * cellSize &&
               local.z < gridHeight * cellSize;
    }

    //Tính vị trí 
    public Vector2Int CalculateStartPosition(Vector2Int gridPos, int size)
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
}


