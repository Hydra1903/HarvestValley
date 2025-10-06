using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
    [Header("Raycast")]
    public LayerMask gridMask;

    [Header("Prefabs đất")]
    public GameObject dugSoilPrefab;  // luống 5x5
    public GameObject holePrefab;     // hố 3x3
    public GameObject sprinklerPrefab; //máy tưới

    [Header("Ghost")]
    public GameObject ghostPlotPrefab;
    public GameObject ghostHolePrefab;
    public GameObject ghostSprinklerPrefab;

    private GameObject ghostPlotInstance;
    private GameObject ghostHoleInstance;
    private GameObject ghostSprinklerInstance;

    [Header("Watering")]
    [SerializeField] private string waterChildName = "WaterOverlay"; // tên child trong prefab luống/hố bật
    [SerializeField] private string holeChildName = "Hole"; // tên child tắt
    [SerializeField] private int waterCost = 3; // năng lượng cho mỗi lần tưới (tuỳ bạn)
    [SerializeField] private int sprinkerRange = 7; // năng lượng cho mỗi lần tưới (tuỳ bạn)

    [SerializeField] private Material ghostRed;
    [SerializeField] private Material ghostBlack;

    private FarmManager farm;

    private readonly List<AreaSave> _areaSaves = new();
    private readonly List<GameObject> _areaObjects = new();
    private readonly HashSet<int> _wateredAreaIdx = new();
    private readonly List<Sprinkler> _sprinklers = new();

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
        var start = farm.CalculateStartPosition(gridPos, info.size);

        bool canPlace = CanPlaceSoil(start.x, start.y, info.size);

        // LUÔN hiện ghost, chỉ đổi material nếu không hợp lệ
        ShowGhost(start, info, canPlace);
    }

    public void HideGhosts()
    {
        if (ghostPlotInstance) ghostPlotInstance.SetActive(false);
        if (ghostHoleInstance) ghostHoleInstance.SetActive(false);
        if (ghostSprinklerInstance) ghostSprinklerInstance.SetActive(false);
    }

    private void ShowGhost(Vector2Int startPos, ToolInfo info, bool valid)
    {
        var ghost = info.size == 5 ? ghostPlotInstance : ghostHoleInstance;
        if (!ghost) return;

        Vector3 ghostPos = farm.origin + new Vector3(
            (startPos.x + info.offsetX) * farm.cellSize,
            info.offsetY,
            (startPos.y + info.offsetZ) * farm.cellSize
        );

        ghost.transform.position = ghostPos;
        ApplyGhostMaterial(ghost, valid);
        ghost.SetActive(true);
    }

    //Áp Material cho ghost
    private void ApplyGhostMaterial(GameObject go, bool valid)
    {
        var mat = valid ? ghostRed : ghostBlack;
        if (mat == null) return;

        var renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = mat;
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

  
    //Hàm đào đất
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

    //Xóa đất
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

    // ===== Ép cây 3x3 vào đúng tâm =====
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
        if (tool == ToolType.Hoe) //Hoe 5x5
        {
            info.size = 5; info.offsetY = 0.26f; info.offsetX = 5f; info.offsetZ = -0.2f;
        }
        else // Shovel 3x3
        {
            info.size = 3; info.offsetY = 0.47f; info.offsetX = 1.5f; info.offsetZ = 1.5f;
        }
        return info;
    }

    // ====== Watering ======

    //tìm tên con của prefab
    private static Transform FindChildRecursive(Transform root, string name)
    {
        if (!root) return null;
        if (root.name == name) return root;
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i);
            var r = FindChildRecursive(c, name);
            if (r != null) return r;
        }
        return null;
    }

    //Ô này đã tưới chưa
    public bool IsTileWatered(int x, int y)
    {
        if (!farm.IsInGrid(x, y)) return false;
        if (TryFindAreaContaining(x, y, out int idx))
            return _wateredAreaIdx.Contains(idx);
        return false;
    }

    //Bật 
    private void SetAreaWaterOverlay(int areaIndex, bool enable)
    {
        if (areaIndex < 0 || areaIndex >= _areaObjects.Count) return;
        var areaObj = _areaObjects[areaIndex];
        if (!areaObj) return;

        var child = FindChildRecursive(areaObj.transform, waterChildName);
        if (child) child.gameObject.SetActive(enable);
    }

    private void SetAreaHole(int areaIndex, bool enable)
    {
        if (areaIndex < 0 || areaIndex >= _areaObjects.Count) return;
        var areaObj = _areaObjects[areaIndex];
        if (!areaObj) return;

        var child = FindChildRecursive(areaObj.transform, holeChildName);
        if (child) child.gameObject.SetActive(enable);
    }

    //Tìm vùng đã tưới
    public bool TryWaterAt(Vector2Int gridPos)
    {
        // tìm vùng (luống/hố) chứa gridPos
        if (!TryFindAreaContaining(gridPos.x, gridPos.y, out int idx))
        {
            Debug.Log("Không có vùng để tưới ở đây.");
            return false;
        }

        // kiểm tra năng lượng (chỉ CHECK)
        if (waterCost > 0 && (Mp.Instance == null || Mp.Instance.mp < waterCost))
        {
            Notification.Instance?.ShowNotification("Hết năng lượng!");
            return false;
        }

        // bật overlay nước
        SetAreaWaterOverlay(idx, true);
        SetAreaHole(idx, false);

        bool ok = _wateredAreaIdx.Add(idx);

        // trừ năng lượng sau khi thành công
        if (waterCost > 0 && ok)
        {
            Mp.Instance?.UseMp(waterCost);
        }
        else
        {
            return false;
        }

        return true;
    }

    //Tắt toàn bộ
    public void ResetDailyWater()
    {
        // tắt overlay toàn bộ vùng đã tưới
        foreach (var idx in _wateredAreaIdx)
            SetAreaWaterOverlay(idx, false);
        foreach (var idx in _wateredAreaIdx) 
            SetAreaHole(idx, true);
           
        _wateredAreaIdx.Clear();
    }
    
    //tưới toàn bộ vùng
    public void WaterAllAreas()
    {
        for (int i = 0; i < _areaObjects.Count; i++)
        {
            SetAreaWaterOverlay(i, true);
            _wateredAreaIdx.Add(i);
        }
    }

    //Tưới toàn bộ theo máy tưới
    public void WaterBySprinklers()
    {
        foreach (var s in _sprinklers)
        {
            if (s != null)
                WaterSquare(new Vector2Int(s.gridX, s.gridY), s.halfRange);
        }
    }
    // Tưới tất cả luống/hố có ô bất kỳ nằm trong vùng (Máy tưới)
    public void WaterSquare(Vector2Int center, int half)
    {
        int minX = Mathf.Max(0, center.x - half);
        int maxX = Mathf.Min(farm.gridWidth - 1, center.x + half);
        int minY = Mathf.Max(0, center.y - half);
        int maxY = Mathf.Min(farm.gridHeight - 1, center.y + half);

        for (int i = 0; i < _areaSaves.Count; i++)
        {
            var a = _areaSaves[i];

            // kiểm tra overlap
            bool overlap = !(a.startX + a.size - 1 < minX || a.startX > maxX ||
                             a.startY + a.size - 1 < minY || a.startY > maxY);

            if (overlap)
            {
                SetAreaWaterOverlay(i, true);
                _wateredAreaIdx.Add(i);
                SetAreaHole(i, false); // nếu là hố thì bật layer nước
            }
        }
    }

    public bool PlaceSprinkler(Vector2Int gridPos, GameObject prefabOverride, int size = 1)
    {
        int startX = gridPos.x;
        int startY = gridPos.y;

        if (!CanPlaceSprinkler(startX, startY, size)) return false;
        var usePrefab = prefabOverride != null ? prefabOverride : sprinklerPrefab;
        if (usePrefab == null) return false;

        int gx = startX + size / 2;
        int gy = startY + size / 2;

        Vector3 pos = farm.origin + new Vector3(
            (gx + 0.5f) * farm.cellSize,
            usePrefab.transform.position.y,
            (gy + 0.5f) * farm.cellSize
        );

        var parent = transform; 
        var go = Instantiate(usePrefab, pos, Quaternion.identity, parent);

        var sp = go.GetComponent<Sprinkler>();
        if (sp == null) sp = go.AddComponent<Sprinkler>();
        sp.Init(gx, gy, sprinkerRange);

        GetSprinklers(sp);
        HideSprinklerGhost();

        return true;
    }

    public void GetSprinklers(Sprinkler s)
    {
        if (s != null && !_sprinklers.Contains(s))
            _sprinklers.Add(s);
    }
    public void RemoveSprinklers(Sprinkler s)
    {
        if (s != null) _sprinklers.Remove(s);
    }

    public void ShowSprinklerGhost(Vector2Int gridPos)
    {
        if (ghostSprinklerPrefab == null) return;
        if (ghostSprinklerInstance == null)
            ghostSprinklerInstance = Instantiate(ghostSprinklerPrefab);

        Vector3 pos = farm.origin + new Vector3(
            (gridPos.x + 0.5f) * farm.cellSize,
            0.2f,
            (gridPos.y + 0.5f) * farm.cellSize
        );

        ghostSprinklerInstance.transform.position = pos;
        ghostSprinklerInstance.SetActive(true);
    }

    public void HideSprinklerGhost()
    {
        if (ghostSprinklerInstance != null)
            ghostSprinklerInstance.SetActive(false);
    }

    private bool IsSprinklerAt(int gx, int gy)
    {
        for (int i = 0; i < _sprinklers.Count; i++)
        {
            var s = _sprinklers[i];
            if (s != null && s.gridX == gx && s.gridY == gy)
                return true;
        }
        return false;
    }

    public bool CanPlaceSprinkler(int startX, int startY, int size)
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                int cx = startX + x, cy = startY + y;
                if (!farm.IsInGrid(cx, cy)) return false;
                if (farm.Tiles[cx, cy].state == SoilState.Planted || farm.Tiles[cx, cy].plantInstance != null) return false;
                if (IsSprinklerAt(cx, cy)) return false;
            }
        return true;
    }

    // ===== Save =====
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

    public void ClearSprinklers(bool destroyGameObjects = true)
    {
        if (destroyGameObjects)
        {
            foreach (var s in _sprinklers)
                if (s != null) Destroy(s.gameObject);
        }
        _sprinklers.Clear();
    }

    public List<SprinklerSave> GetSprinklerSaves()
    {
        var list = new List<SprinklerSave>();
        foreach (var s in _sprinklers)
        {
            if (s == null) continue;
            list.Add(new SprinklerSave { gridX = s.gridX, gridY = s.gridY, halfRange = s.halfRange });
        }
        return list;
    }

    public void AddSprinklerFromSave(SprinklerSave ss)
    {
        // tái tạo sprinkler dưới Farm này
        GameObject go;
        if (sprinklerPrefab != null)
        {
            Vector3 pos = farm.origin + new Vector3(
                (ss.gridX + 0.5f) * farm.cellSize,
                0f,
                (ss.gridY + 0.5f) * farm.cellSize
            );
            go = Instantiate(sprinklerPrefab, pos, Quaternion.identity, transform);
        }
        else
        {
            // fallback: tạo object rỗng có component Sprinkler
            go = new GameObject($"Sprinkler_{ss.gridX}_{ss.gridY}");
            go.transform.SetParent(transform, worldPositionStays: false);
        }

        var s = go.GetComponent<Sprinkler>();
        if (!s) s = go.AddComponent<Sprinkler>();
        s.Init(ss.gridX, ss.gridY, ss.halfRange);
        GetSprinklers(s); 
    }
    // --- Water of the day ---
    // Lưu ở dạng “center của vùng đã tưới” để khôi phục overlay đúng vùng
    public List<Vector2Int> GetWateredCenters()
    {
        var res = new List<Vector2Int>();
        foreach (var idx in _wateredAreaIdx)
        {
            if (idx < 0 || idx >= _areaSaves.Count) continue;
            var a = _areaSaves[idx];
            int cx = a.startX + a.size / 2;
            int cy = a.startY + a.size / 2;
            res.Add(new Vector2Int(cx, cy));
        }
        return res;
    }

    public void ApplyWateredCenters(List<Vector2Int> centers)
    {
        if (centers == null) return;
        // dọn trước
        ResetDailyWater();

        // bật lại overlay theo danh sách center
        for (int i = 0; i < _areaSaves.Count; i++)
        {
            var a = _areaSaves[i];
            int cx = a.startX + a.size / 2;
            int cy = a.startY + a.size / 2;

            // có trong danh sách -> bật overlay + mark watered
            if (centers.Contains(new Vector2Int(cx, cy)))
            {
                SetAreaWaterOverlay(i, true);
                _wateredAreaIdx.Add(i);
                SetAreaHole(i, false);
            }
        }
    }

    public void ClearWatered()
    {
        ResetDailyWater();
    }
}
