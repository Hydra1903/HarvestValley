using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
    [Header("Raycast")]
    public LayerMask gridMask;

    [Header("Prefabs đất")]
    public GameObject dugSoilPrefab;  
    public GameObject holePrefab;     
    public GameObject sprinklerPrefab; 

    [Header("Ghost")]
    public GameObject ghostFurrowPrefab;
    public GameObject ghostHolePrefab;
    public GameObject ghostSprinklerPrefab;

    private GameObject ghostFurrowInstance;
    private GameObject ghostHoleInstance;
    private GameObject ghostSprinklerInstance;

    [Header("Watering")]
    [SerializeField] private string waterChildName; // tên child trong prefab luống/hố bật
    [SerializeField] private string soilChildName; 
    [SerializeField] private int waterCost = 3; 
    [SerializeField] private int sprinkerRange = 7; 

    [SerializeField] private Material ghostRed;
    [SerializeField] private Material ghostBlack;

    [SerializeField] private int useMPDigHole;
    [SerializeField] private int useMPDigFurrow;
    [SerializeField] private int useMPFlatten;

    private FarmManager farm;
    [SerializeField] private HoeMode hoeMode = HoeMode.DigFurrow5x5;


    private readonly List<AreaSave> _areaSaves = new();
    private readonly List<GameObject> _areaObjects = new();
    private readonly HashSet<int> _wateredAreaIdx = new();
    private readonly List<Sprinkler> _sprinklers = new();

    private void OnValidate()
    {
        if (!farm) farm = GetComponent<FarmManager>();
    }

    public void Initialize(FarmManager f)
    {
        farm = f;

        if (ghostFurrowPrefab)
        {
            ghostFurrowInstance = Instantiate(ghostFurrowPrefab, Vector3.zero, Quaternion.identity);
            ghostFurrowInstance.SetActive(false);
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
        // Không phải Hoe → ẩn ghost
        if (currentItem?.itemData?.toolType != ToolType.Hoe)
        {
            HideGhosts();
            return;
        }

        // === FLATTEN: chỉ hiện ghost đỏ khi trúng vùng đã đào ===
        if (hoeMode == HoeMode.Flatten)
        {
            // mặc định ẩn
            HideGhosts();

            // chỉ khi trúng luống/hố hiện có mới hiện ghost đỏ
            if (TryFindAreaContaining(gridPos.x, gridPos.y, out int idx))
            {
                var a = _areaSaves[idx];
                var start = new Vector2Int(a.startX, a.startY);
                int size = a.size;
                ShowGhostForceRed(start, size);
            }
            return;
        }

        int sizeDig = GetSizeByHoeMode();        
        var startDig = farm.CalculateStartPosition(gridPos, sizeDig);
        bool canPlace = CanPlaceSoil(startDig.x, startDig.y, sizeDig);

        ShowGhostNormal(startDig, sizeDig, canPlace);
    }


    // Ghost thường cho chế độ Đào: hợp lệ -> allowed, không hợp lệ -> đỏ
    private void ShowGhostNormal(Vector2Int startPos, int size, bool canPlace)
    {
        var info = new ToolInfo
        {
            size = size,
            offsetY = (size == 5 ? 0.305f : 0.47f),
            offsetX = (size == 5 ? 5f : 1.5f),
            offsetZ = (size == 5 ? -0.2f : 1.5f)
        };
        ShowGhost(startPos, info, canPlace, forceRed: false);
    }

    // Ghost đỏ bắt buộc cho Flatten khi trúng vùng
    private void ShowGhostForceRed(Vector2Int startPos, int size)
    {
        var info = new ToolInfo
        {
            size = size,
            offsetY = (size == 5 ? 0.305f : 0.47f),
            offsetX = (size == 5 ? 5f : 1.5f),
            offsetZ = (size == 5 ? -0.2f : 1.5f)
        };
        ShowGhost(startPos, info, valid: true, forceRed: true);
    }
    public void HideGhosts()
    {
        if (ghostFurrowInstance) ghostFurrowInstance.SetActive(false);
        if (ghostHoleInstance) ghostHoleInstance.SetActive(false);
        if (ghostSprinklerInstance) ghostSprinklerInstance.SetActive(false);
    }

    private void ShowGhost(Vector2Int startPos, ToolInfo info, bool valid, bool forceRed = false)
    {
        var ghost = info.size == 5 ? ghostFurrowInstance : ghostHoleInstance;
        if (!ghost) return;

        Vector3 ghostPos = farm.origin + new Vector3(
            (startPos.x + info.offsetX) * farm.cellSize,
            info.offsetY,
            (startPos.y + info.offsetZ) * farm.cellSize
        );

        ghost.transform.position = ghostPos;
        ApplyGhostMaterial(ghost, valid, forceRed);
        ghost.SetActive(true);
    }

    private void ApplyGhostMaterial(GameObject go, bool valid, bool forceRed = false)
    {
        // valid (đào hợp lệ) -> dùng ghostBlack (allowed)
        // invalid hoặc forceRed (Flatten highlight) -> dùng ghostRed
        var mat = (forceRed || !valid) ? ghostRed : ghostBlack;
        if (!go || mat == null) return;

        var renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = mat;
    }

    // ===== Click: Đào hoặc Hủy =====

    public void HoeAt(Vector2Int gridPos, InventoryItem item)
    {
        if (item?.itemData?.toolType != ToolType.Hoe) return;

        switch (hoeMode)
        {
            case HoeMode.DigFurrow5x5:
                Dig(gridPos, 5, useMPDigFurrow);
                Debug.LogWarning("Furrow");
                break;

            case HoeMode.DigHole3x3:
                Dig(gridPos, 3, useMPDigHole);
                Debug.LogWarning("Hole");
                break;

            case HoeMode.Flatten:
                Flatten(gridPos);
                break;

            default:
                Debug.LogWarning("Chưa chọn chế độ hoe hợp lệ!");
                break;
        }
    }

    private void Dig(Vector2Int gridPos, int size, int mpCost)
    {
        var start = farm.CalculateStartPosition(gridPos, size);

        if (!CanPlaceSoil(start.x, start.y, size))
        {
            Notification.Instance?.ShowNotification("Khu vực này đã được đào!");
            return;
        }

        if (Mp.Instance.mp < mpCost)
        {
            Notification.Instance?.ShowNotification("Hết năng lượng!");
            return;
        }

        PlaceArea(start.x, start.y, size);
        Mp.Instance.UseMp(mpCost);
        FindAnyObjectByType<SoilGrid>()?.UpdateGridColors();
    }

    public void Flatten(Vector2Int gridPos)
    {

        int cost = useMPFlatten;

        // chỉ kiểm tra đủ NL
        if (Mp.Instance != null && Mp.Instance.mp < cost)
        { 
            Notification.Instance?.ShowNotification("Hết năng lượng!");
            return; 
        }

        if (FlattenAreaAt(gridPos))
        {
            Mp.Instance?.UseMp(cost);
            FindAnyObjectByType<SoilGrid>()?.UpdateGridColors();
            HideGhosts();
        }
        else
        {
            Notification.Instance?.ShowNotification("Không có khu đất nào để hủy!");
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

        float yOffset = (size == 5) ? 0.3f : 0.45f;
        float offsetX = (size == 5) ? 5f : 1.5f;
        float offsetZ = (size == 5) ? -0.2f : 1.5f;

        Vector3 pos = farm.origin + new Vector3(
            (startX + offsetX) * farm.cellSize,
            yOffset,
            (startY + offsetZ) * farm.cellSize
        );

        var prefab = (size == 5) ? dugSoilPrefab : holePrefab;
        var go = prefab ? Instantiate(prefab, pos,  Quaternion.identity) : null;
        _areaObjects.Add(go);
        FindAnyObjectByType<SoilGrid>()?.UpdateGridColors();
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

    private struct ToolInfo
    {
        public int size;
        public float offsetY, offsetX, offsetZ;
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

        var child = FindChildRecursive(areaObj.transform, soilChildName);
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
        var t = farm.Tiles[gridPos.x, gridPos.y];
        if (t != null)
        {
            t.state = SoilState.Planted;   
        }

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

    public bool CanStartHoeAt(Vector2Int gridPos, InventoryItem item)
    {
        if (item?.itemData?.toolType != ToolType.Hoe) return false;

        switch (hoeMode)
        {
            case HoeMode.DigFurrow5x5:
                {
                    int size = 5;
                    var start = farm.CalculateStartPosition(gridPos, size);
                    if (!CanPlaceSoil(start.x, start.y, size)) return false;
                    return Mp.Instance == null || Mp.Instance.mp >= useMPDigFurrow;
                }
            case HoeMode.DigHole3x3:
                {
                    int size = 3;
                    var start = farm.CalculateStartPosition(gridPos, size);
                    if (!CanPlaceSoil(start.x, start.y, size)) return false;
                    return Mp.Instance == null || Mp.Instance.mp >= useMPDigHole;
                }
            case HoeMode.Flatten:
                {
                    // Phải trúng 1 vùng đã đào
                    if (!TryFindAreaContaining(gridPos.x, gridPos.y, out int idx)) return false;

                    // Không có cây trong vùng
                    var a = _areaSaves[idx];
                    for (int dx = 0; dx < a.size; dx++)
                        for (int dy = 0; dy < a.size; dy++)
                            if (farm.Tiles[a.startX + dx, a.startY + dy].plantInstance != null)
                                return false;

                    return Mp.Instance == null || Mp.Instance.mp >= useMPFlatten;
                }
        }
        return false;
    }

    public bool CanStartWaterAt(Vector2Int gridPos)
    {
        if (!TryFindAreaContaining(gridPos.x, gridPos.y, out int idx))
        {
            Notification.Instance?.ShowNotification("Không thể tưới nước tại đây!");
            return false;
        }    
        if (waterCost > 0 && (Mp.Instance == null || Mp.Instance.mp < waterCost)) 
        {
            Notification.Instance?.ShowNotification("Không đủ năng lượng!");
            return false;
        }

        if (_wateredAreaIdx.Contains(idx))
        {
            Notification.Instance?.ShowNotification("Đã được tưới!");
            return false;
        }
        if (WaterCan.Instance.currentWater <= 0)
        {
            Notification.Instance?.ShowNotification("Hết nước!");
            return false;
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

    //Lấy kích thước chế độ hoe
    public int GetSizeByHoeMode()
    {
        switch (hoeMode)
        {
            case HoeMode.DigFurrow5x5: return 5;
            case HoeMode.DigHole3x3: return 3;
            case HoeMode.Flatten: return 5; 
        }
        return 5;
    }

    public void SetHoeMode(HoeMode mode)
    {
        hoeMode = mode;
        if (hoeMode == HoeMode.Flatten) HideGhosts();
    }

    //Có máy tưới tại vị trí hiện tại hay không
    public bool HasSprinklerAt(int gx, int gy)
    {
        return IsSprinklerAt(gx, gy);
    }

    //Lấy GameObject tại vị trí 
    public GameObject GetAreaObjectAt(Vector2Int gridPos)
    {
        if (TryFindAreaContaining(gridPos.x, gridPos.y, out int idx))
        {
            if (idx >= 0 && idx < _areaObjects.Count)
                return _areaObjects[idx];
        }
        return null;
    }

    //Lấy vị trí theo index
    public GameObject GetAreaObjectByIndex(int index)
    {
        if (index >= 0 && index < _areaObjects.Count)
            return _areaObjects[index];
        return null;
    }

}
