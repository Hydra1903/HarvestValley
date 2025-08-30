using System.Collections.Generic;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
    public static SoilManager Instance { get; private set; }

    [Header("Raycast")]
    public LayerMask gridMask;

    [Header("Prefabs đất")]
    public GameObject dugSoilPrefab;  // luống 5x5
    public GameObject holePrefab;     // hố 3x3

    [Header("Ghost đất")]
    public GameObject ghostPlotPrefab;
    public GameObject ghostHolePrefab;

    private GameObject ghostPlotInstance;
    private GameObject ghostHoleInstance;

    private FarmManager farm;

    private readonly List<AreaSave> _areaSaves = new();
    private readonly List<GameObject> _areaObjects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Initialize(FarmManager f)
    {
        farm = f;

        if (ghostPlotPrefab)
        {
            ghostPlotInstance = Instantiate(ghostPlotPrefab, Vector3.zero, Quaternion.identity);
            ghostPlotInstance.SetActive(false);
        }
        if (ghostHolePrefab)
        {
            ghostHoleInstance = Instantiate(ghostHolePrefab, Vector3.zero, Quaternion.identity);
            ghostHoleInstance.SetActive(false);
        }
    }

    // ===== Hover / Ghost =====
    public void HandleToolHover(Vector2Int gridPos, InventoryItem currentItem)
    {
        var info = GetToolInfo((currentItem?.itemData?.toolType) ?? ToolType.None);
        var start = FarmManager.Instance.CalculateStartPosition(gridPos, info.size);

        if (CanPlaceSoil(start.x, start.y, info.size))
            ShowGhost(start, info);
        else
            HideGhosts();
    }

    public void HideGhosts()
    {
        if (ghostPlotInstance) ghostPlotInstance.SetActive(false);
        if (ghostHoleInstance) ghostHoleInstance.SetActive(false);
    }

    private void ShowGhost(Vector2Int startPos, ToolInfo info)
    {
        var ghost = info.size == 5 ? ghostPlotInstance : ghostHoleInstance;
        if (!ghost) return;

        Vector3 ghostPos = farm.origin + new Vector3(
            (startPos.x + info.offsetX) * farm.cellSize,
            info.offsetY,
            (startPos.y + info.offsetZ) * farm.cellSize
        );

        ghost.transform.position = ghostPos;
        ghost.SetActive(true);
    }

    // ===== Click: Đào hoặc Hủy =====
    public void TryDigOrFlatten(Vector2Int gridPos, InventoryItem currentItem)
    {
        var info = GetToolInfo((currentItem?.itemData?.toolType) ?? ToolType.None);
        var start = farm.CalculateStartPosition(gridPos, info.size);
        var expectedType = (info.size == 5) ? SoilType.Plot : SoilType.Hole;
        int cost = (info.size == 5) ? 10 : 6;

        // chỉ kiểm tra đủ NL
        if (Mp.Instance != null && Mp.Instance.mp < cost)
        { Notification.Instance?.ShowNotification("Hết năng lượng!"); return; }

        // Hủy nếu đang đúng loại
        if (TryFindAreaContaining(gridPos.x, gridPos.y, out int idx))
        {
            var a = _areaSaves[idx];
            if (a.soilType == expectedType)
            {
                if (FlattenAreaAt(gridPos)) // thành công mới trừ NL
                {
                    if (cost > 0) Mp.Instance?.UseMp(cost);
                }
                return;
            }
        }

        // Đặt mới
        if (CanPlaceSoil(start.x, start.y, info.size))
        {
            PlaceArea(start.x, start.y, info.size);
            if (cost > 0) Mp.Instance?.UseMp(cost);
        }
    }

    // ===== Core soil ops =====
    public bool TryFindAreaContaining(int x, int y, out int index)
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

    public bool CanPlaceSoil(int startX, int startY, int size)
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                int cx = startX + x, cy = startY + y;
                if (!farm.IsInGrid(cx, cy)) return false;
                if (farm.Tiles[cx, cy].state == SoilState.Dug) return false;
            }
        return true;
    }

    public void PlaceArea(int startX, int startY, int size)
    {
        for (int dx = 0; dx < size; dx++)
            for (int dy = 0; dy < size; dy++)
            {
                farm.Tiles[startX + dx, startY + dy].state = SoilState.Dug;
                farm.Tiles[startX + dx, startY + dy].soilType = (size == 5) ? SoilType.Plot : SoilType.Hole;
            }

        var a = new AreaSave
        {
            startX = startX,
            startY = startY,
            size = size,
            soilType = (size == 5) ? SoilType.Plot : SoilType.Hole
        };
        _areaSaves.Add(a);

        float yOffset = (size == 5) ? 0.235f : 0.45f;
        float offsetX = (size == 5) ? 5f : 1.5f;
        float offsetZ = (size == 5) ? -0.2f : 1.5f;

        Vector3 pos = farm.origin + new Vector3(
            (startX + offsetX) * farm.cellSize,
            yOffset,
            (startY + offsetZ) * farm.cellSize
        );

        var prefab = (size == 5) ? dugSoilPrefab : holePrefab;
        var go = prefab ? Instantiate(prefab, pos, (size == 3 ? RandomizeRotation() : Quaternion.identity)) : null;
        _areaObjects.Add(go);
    }

    public bool FlattenAreaAt(Vector2Int gridPos)
    {
        int x = gridPos.x, y = gridPos.y;
        if (!farm.IsInGrid(x, y)) return false;

        if (!TryFindAreaContaining(x, y, out int idx))
        { Debug.Log("Không có luống/hố để hủy tại đây."); return false; }

        var a = _areaSaves[idx];

        // chặn nếu có cây
        for (int dx = 0; dx < a.size; dx++)
            for (int dy = 0; dy < a.size; dy++)
            {
                var t = farm.Tiles[a.startX + dx, a.startY + dy];
                if (t.plantInstance != null)
                { Debug.LogWarning("Không thể hủy vì vùng đang có cây!"); return false; }
            }

        // reset tiles
        for (int dx = 0; dx < a.size; dx++)
            for (int dy = 0; dy < a.size; dy++)
            {
                var t = farm.Tiles[a.startX + dx, a.startY + dy];
                t.state = SoilState.Normal;
                t.soilType = SoilType.None;
            }

        // destroy prefab
        if (idx >= 0 && idx < _areaObjects.Count && _areaObjects[idx] != null)
        { Destroy(_areaObjects[idx]); _areaObjects[idx] = null; }

        // remove record
        _areaSaves.RemoveAt(idx);
        if (idx >= 0 && idx < _areaObjects.Count) _areaObjects.RemoveAt(idx);

        Debug.Log($"Đã hủy {(a.size == 5 ? "luống 5x5" : "hố 3x3")} tại ({a.startX},{a.startY}).");
        return true;
    }

    // ===== Strategy helpers cho cây lớn =====
    public bool TrySnapStartToHole3x3(Vector2Int hoverPos, out Vector2Int snappedStart)
    {
        if (TryFindAreaContaining(hoverPos.x, hoverPos.y, out int idx))
        {
            var a = _areaSaves[idx];
            if (a.soilType == SoilType.Hole && a.size == 3)
            { snappedStart = new Vector2Int(a.startX, a.startY); return true; }
        }
        snappedStart = default; return false;
    }

    public bool IsExactAreaMatch(int startX, int startY, int size, SoilType type)
    {
        for (int i = 0; i < _areaSaves.Count; i++)
        {
            var a = _areaSaves[i];
            if (a.soilType == type && a.size == size && a.startX == startX && a.startY == startY)
                return true;
        }
        return false;
    }

    // ===== Util =====
    private Quaternion RandomizeRotation()
    {
        float y = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, y, 0f);
    }

    private struct ToolInfo
    {
        public int size;
        public float offsetY, offsetX, offsetZ;
    }

    private ToolInfo GetToolInfo(ToolType tool)
    {
        var info = new ToolInfo();
        if (tool == ToolType.Hoe) // 5x5
        {
            info.size = 5; info.offsetY = 0.235f; info.offsetX = -0.2f; info.offsetZ = 5f;
        }
        else // Shovel 3x3
        {
            info.size = 3; info.offsetY = 0.45f; info.offsetX = 1.5f; info.offsetZ = 1.5f;
        }
        return info;
    }

    // ===== Save helpers =====
    public List<AreaSave> GetAreas() => _areaSaves;
    public void ClearAreas()
    {
        foreach (var go in _areaObjects) if (go) Destroy(go);
        _areaObjects.Clear();
        _areaSaves.Clear();

        for (int x = 0; x < farm.gridWidth; x++)
            for (int y = 0; y < farm.gridHeight; y++)
            {
                farm.Tiles[x, y].state = SoilState.Normal;
                farm.Tiles[x, y].soilType = SoilType.None;
            }
    }
    public void AddAreaFromSave(AreaSave a) => PlaceArea(a.startX, a.startY, a.size);
}
