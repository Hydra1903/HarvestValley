using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlantGrid : MonoBehaviour
{
    [Header("References")]
    public FarmManager farm;
    public SoilManager soilManager;

    [Header("Colors")]
    [Tooltip("Màu ô bình thường (đã đào nhưng chưa hover)")]
    public Color normalDugColor = new Color(0f, 1f, 0f, 0.3f);

    [Tooltip("Màu khi có thể trồng (hover + hợp lệ)")]
    public Color validColor = new Color(0f, 1f, 0f, 0.9f);

    [Tooltip("Màu khi không thể trồng (hover + không hợp lệ)")]
    public Color invalidColor = new Color(1f, 0f, 0f, 0.9f);

    [Tooltip("Màu viền cho các ô đã trồng")]
    public Color plantedColor = new Color(1f, 0f, 0f, 0.85f);

    [Header("Visual")]
    [Tooltip("Nâng lưới lên khỏi mặt đất")]
    public float yOffset = 0.25f;

    [Header("Line Width")]
    [Range(0.01f, 0.5f)]
    public float lineWidth = 0.08f;

    private MeshFilter _mf;
    private MeshRenderer _mr;
    private Mesh _mesh;

    private int _W, _H;
    private int _totalHorizontalQuads;
    private int _totalVerticalQuads;

    // Map để lưu index của từng edge
    private Dictionary<string, int> _edgeToQuadIndex = new Dictionary<string, int>();

    // Vùng highlight hiện tại
    private Vector2Int _highlightStart = new Vector2Int(-1, -1);
    private int _highlightSize = 0;
    private bool _highlightValid = false;

    private float _lastLineWidth;

    private void Awake()
    {
        if (!farm) farm = GetComponentInParent<FarmManager>();
        if (!soilManager) soilManager = GetComponentInParent<SoilManager>();
        EnsureComponents();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EnsureComponents();
        GenerateMesh();
        UpdateGridColors();
        _lastLineWidth = lineWidth;
    }

    private void Update()
    {
        if (Mathf.Abs(_lastLineWidth - lineWidth) > 0.001f)
        {
            _lastLineWidth = lineWidth;
            GenerateMesh();
            UpdateGridColors();
        }
    }

    private void EnsureComponents()
    {
        if (_mf == null)
            _mf = GetComponent<MeshFilter>();
        if (_mf == null)
            _mf = gameObject.AddComponent<MeshFilter>();

        if (_mr == null)
            _mr = GetComponent<MeshRenderer>();
        if (_mr == null)
            _mr = gameObject.AddComponent<MeshRenderer>();

        if (_mr.sharedMaterial == null || _mr.sharedMaterial.shader == null)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = Color.white;
            mat.renderQueue = 3001;
            _mr.sharedMaterial = mat;
        }

        _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _mr.receiveShadows = false;
    }

    /// <summary>
    /// Tạo mesh giống SoilGrid - hiện toàn bộ farm
    /// </summary>
    public void GenerateMesh()
    {
        if (farm == null) return;

        _W = farm.gridWidth;
        _H = farm.gridHeight;

        if (_W <= 0 || _H <= 0) return;

        if (_mf == null)
        {
            EnsureComponents();
            if (_mf == null) return;
        }

        if (_mesh != null)
        {
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
        }

        _mesh = new Mesh { name = "PlantGridMesh" };

        var verts = new List<Vector3>();
        var indices = new List<int>();
        var colors = new List<Color>();
        _edgeToQuadIndex.Clear();

        float c = farm.cellSize;
        float halfWidth = lineWidth * 0.5f;

        int quadIndex = 0;

        // ===== TẠO SEGMENT NGANG =====
        for (int y = 0; y <= _H; y++)
        {
            for (int x = 0; x < _W; x++)
            {
                Vector3 a = new Vector3(x * c, 0f, y * c);
                Vector3 b = new Vector3((x + 1) * c, 0f, y * c);

                string key = $"H_{x}_{y}";
                _edgeToQuadIndex[key] = quadIndex;

                CreateLineQuad(verts, indices, colors, a, b, halfWidth, normalDugColor);
                quadIndex++;
            }
        }
        _totalHorizontalQuads = quadIndex;

        // ===== TẠO SEGMENT DỌC =====
        for (int x = 0; x <= _W; x++)
        {
            for (int y = 0; y < _H; y++)
            {
                Vector3 a = new Vector3(x * c, 0f, y * c);
                Vector3 b = new Vector3(x * c, 0f, (y + 1) * c);

                string key = $"V_{x}_{y}";
                _edgeToQuadIndex[key] = quadIndex;

                CreateLineQuad(verts, indices, colors, a, b, halfWidth, normalDugColor);
                quadIndex++;
            }
        }
        _totalVerticalQuads = quadIndex - _totalHorizontalQuads;

        _mesh.SetVertices(verts);
        _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        _mesh.SetColors(colors);
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        _mf.sharedMesh = _mesh;
        transform.position = farm.origin + Vector3.up * yOffset;
    }

    private void CreateLineQuad(List<Vector3> verts, List<int> indices, List<Color> colors,
                                Vector3 start, Vector3 end, float halfWidth, Color col)
    {
        Vector3 dir = (end - start).normalized;
        Vector3 perpendicular = new Vector3(-dir.z, 0f, dir.x) * halfWidth;

        int baseIdx = verts.Count;

        verts.Add(start - perpendicular);
        verts.Add(start + perpendicular);
        verts.Add(end + perpendicular);
        verts.Add(end - perpendicular);

        colors.Add(col);
        colors.Add(col);
        colors.Add(col);
        colors.Add(col);

        indices.Add(baseIdx + 0);
        indices.Add(baseIdx + 1);
        indices.Add(baseIdx + 2);

        indices.Add(baseIdx + 0);
        indices.Add(baseIdx + 2);
        indices.Add(baseIdx + 3);
    }

    /// <summary>
    /// Cập nhật màu grid dựa trên vùng đã đào và vùng hover
    /// </summary>
    public void UpdateGridColors()
    {
        if (farm == null || farm.Tiles == null || _mesh == null) return;

        var cols = _mesh.colors;
        if (cols == null || cols.Length != _mesh.vertexCount)
            cols = new Color[_mesh.vertexCount];

        // Reset về màu trong suốt
        for (int i = 0; i < cols.Length; i++)
            cols[i] = new Color(0, 0, 0, 0); // Hoàn toàn trong suốt

        // Tô màu xanh nhạt cho các ô đã đào
        for (int x = 0; x < _W; x++)
        {
            for (int y = 0; y < _H; y++)
            {
                var tile = farm.Tiles[x, y];
                bool dug = (tile.state == SoilState.Dug) || (tile.soilType != SoilType.None);
                if (dug)
                {
                    PaintCellEdges(cols, x, y, normalDugColor);
                }
            }
        }

        // Highlight vùng trồng (nếu có)
        if (_highlightSize > 0 && _highlightStart.x >= 0)
        {
            Color highlightColor = _highlightValid ? validColor : invalidColor;

            for (int dx = 0; dx < _highlightSize; dx++)
            {
                for (int dy = 0; dy < _highlightSize; dy++)
                {
                    int x = _highlightStart.x + dx;
                    int y = _highlightStart.y + dy;

                    if (x >= 0 && x < _W && y >= 0 && y < _H)
                    {
                        PaintCellEdges(cols, x, y, highlightColor);
                    }
                }
            }
        }

        for (int x = 0; x < _W; x++)
        {
            for (int y = 0; y < _H; y++)
            {
                var tile = farm.Tiles[x, y];
                if (tile.state == SoilState.Planted)
                {
                    // Tô viền đỏ cho ô đã trồng
                    PaintCellEdges(cols, x, y, plantedColor);
                }
            }
        }

        _mesh.colors = cols;
    }

    private void PaintCellEdges(Color[] cols, int x, int y, Color color)
    {
        // 1) Ngang trên
        if (y >= 0 && y <= _H && x >= 0 && x < _W)
        {
            string key = $"H_{x}_{y}";
            if (_edgeToQuadIndex.TryGetValue(key, out int quadIdx))
            {
                int vIdx = quadIdx * 4;
                if (vIdx + 3 < cols.Length)
                {
                    cols[vIdx + 0] = color;
                    cols[vIdx + 1] = color;
                    cols[vIdx + 2] = color;
                    cols[vIdx + 3] = color;
                }
            }
        }

        // 2) Ngang dưới
        if (y + 1 >= 0 && y + 1 <= _H && x >= 0 && x < _W)
        {
            string key = $"H_{x}_{y + 1}";
            if (_edgeToQuadIndex.TryGetValue(key, out int quadIdx))
            {
                int vIdx = quadIdx * 4;
                if (vIdx + 3 < cols.Length)
                {
                    cols[vIdx + 0] = color;
                    cols[vIdx + 1] = color;
                    cols[vIdx + 2] = color;
                    cols[vIdx + 3] = color;
                }
            }
        }

        // 3) Dọc trái
        if (x >= 0 && x <= _W && y >= 0 && y < _H)
        {
            string key = $"V_{x}_{y}";
            if (_edgeToQuadIndex.TryGetValue(key, out int quadIdx))
            {
                int vIdx = quadIdx * 4;
                if (vIdx + 3 < cols.Length)
                {
                    cols[vIdx + 0] = color;
                    cols[vIdx + 1] = color;
                    cols[vIdx + 2] = color;
                    cols[vIdx + 3] = color;
                }
            }
        }

        // 4) Dọc phải
        if (x + 1 >= 0 && x + 1 <= _W && y >= 0 && y < _H)
        {
            string key = $"V_{x + 1}_{y}";
            if (_edgeToQuadIndex.TryGetValue(key, out int quadIdx))
            {
                int vIdx = quadIdx * 4;
                if (vIdx + 3 < cols.Length)
                {
                    cols[vIdx + 0] = color;
                    cols[vIdx + 1] = color;
                    cols[vIdx + 2] = color;
                    cols[vIdx + 3] = color;
                }
            }
        }
    }

    /// <summary>
    /// Highlight vùng sẽ trồng cây (gọi từ FarmInput)
    /// </summary>
    public void ShowPlantArea(Vector2Int startPos, int size, bool isValid)
    {
        _highlightStart = startPos;
        _highlightSize = size;
        _highlightValid = isValid;
        UpdateGridColors();
    }

    /// <summary>
    /// Xóa highlight
    /// </summary>
    public void Hide()
    {
        _highlightStart = new Vector2Int(-1, -1);
        _highlightSize = 0;
        UpdateGridColors();
    }

    /// <summary>
    /// Hiển thị grid (bật object)
    /// </summary>
    public void SetActiveGrid(bool active)
    {
        if (gameObject.activeSelf != active)
        {
            gameObject.SetActive(active);
            if (active)
            {
                GenerateMesh();
                UpdateGridColors();
            }
        }
    }

    public void SyncPosition()
    {
        if (farm)
            transform.position = farm.origin + Vector3.up * yOffset;
    }

    private void OnDestroy()
    {
        if (_mesh != null)
        {
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
        }
    }
}