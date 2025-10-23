using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FarmGrid : MonoBehaviour
{
    [Header("References")]
    public FarmManager farm;

    [Header("Colors")]
    public Color normalColor = new Color(0f, 1f, 0f, 0.65f);
    public Color dugColor = new Color(1f, 0f, 0f, 0.85f);

    [Header("Visual")]
    [Tooltip("Nâng lưới lên khỏi mặt đất để tránh z-fighting")]
    public float yOffset = 0.15f;

    [Header("Line Width")]
    [Tooltip("Độ dày của đường viền lưới (đơn vị: Unity units)")]
    [Range(0.01f, 0.5f)]
    public float lineWidth = 0.05f;

    private MeshFilter _mf;
    private MeshRenderer _mr;
    private Mesh _mesh;

    private int _W, _H;
    private int _hSegments;
    private int _vSegments;
    private int _baseVtxVertical;

    private float _lastLineWidth;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (!farm) farm = GetComponentInParent<FarmManager>();
        EnsureComponents();
        GenerateMesh();
        UpdateGridColors();
        _lastLineWidth = lineWidth;
    }

    private void Update()
    {
        // Kiểm tra nếu lineWidth thay đổi trong Editor
        if (Mathf.Abs(_lastLineWidth - lineWidth) > 0.001f)
        {
            _lastLineWidth = lineWidth;
            GenerateMesh();
            UpdateGridColors();
        }
    }

    private void OnValidate()
    {
        if (!farm) farm = GetComponentInParent<FarmManager>();
        EnsureComponents();

        // Tự động cập nhật mesh khi thay đổi lineWidth trong Inspector
        if (_mesh != null && Application.isPlaying)
        {
            GenerateMesh();
            UpdateGridColors();
        }
    }

    private void EnsureComponents()
    {
        _mf = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();

        if (_mr.sharedMaterial == null || _mr.sharedMaterial.shader == null)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = Color.white;
            mat.renderQueue = 3000;
            _mr.sharedMaterial = mat;
        }

        _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _mr.receiveShadows = false;
    }

    public void GenerateMesh()
    {
        if (farm == null) return;

        _W = farm.gridWidth;
        _H = farm.gridHeight;

        if (_W <= 0 || _H <= 0) return;

        if (_mesh != null) DestroyImmediate(_mesh);
        _mesh = new Mesh { name = "FarmGridMesh" };

        var verts = new List<Vector3>();
        var indices = new List<int>();
        var colors = new List<Color>();

        float c = farm.cellSize;
        float halfWidth = lineWidth * 0.5f;

        // ===== TẠO SEGMENT NGANG (với độ dày) =====
        for (int y = 0; y <= _H; y++)
        {
            for (int x = 0; x < _W; x++)
            {
                Vector3 a = new Vector3(x * c, 0f, y * c);
                Vector3 b = new Vector3((x + 1) * c, 0f, y * c);

                // Tạo quad với độ dày
                CreateLineQuad(verts, indices, colors, a, b, halfWidth, normalColor);
            }
        }
        _hSegments = (_H + 1) * _W;

        // ===== TẠO SEGMENT DỌC (với độ dày) =====
        _baseVtxVertical = verts.Count;

        for (int x = 0; x <= _W; x++)
        {
            for (int y = 0; y < _H; y++)
            {
                Vector3 a = new Vector3(x * c, 0f, y * c);
                Vector3 b = new Vector3(x * c, 0f, (y + 1) * c);

                // Tạo quad với độ dày
                CreateLineQuad(verts, indices, colors, a, b, halfWidth, normalColor);
            }
        }
        _vSegments = (_W + 1) * _H;

        _mesh.SetVertices(verts);
        _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        _mesh.SetColors(colors);
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        _mf.sharedMesh = _mesh;
        transform.position = farm.origin + Vector3.up * yOffset;
    }

    // Tạo một quad (hình chữ nhật) để mô phỏng đường có độ dày
    private void CreateLineQuad(List<Vector3> verts, List<int> indices, List<Color> colors,
                                Vector3 start, Vector3 end, float halfWidth, Color col)
    {
        // Tính vector vuông góc với đường
        Vector3 dir = (end - start).normalized;
        Vector3 perpendicular = new Vector3(-dir.z, 0f, dir.x) * halfWidth;

        int baseIdx = verts.Count;

        // 4 đỉnh của quad
        verts.Add(start - perpendicular); // 0
        verts.Add(start + perpendicular); // 1
        verts.Add(end + perpendicular);   // 2
        verts.Add(end - perpendicular);   // 3

        // Màu cho 4 đỉnh
        colors.Add(col);
        colors.Add(col);
        colors.Add(col);
        colors.Add(col);

        // 2 tam giác tạo thành quad
        indices.Add(baseIdx + 0);
        indices.Add(baseIdx + 1);
        indices.Add(baseIdx + 2);

        indices.Add(baseIdx + 0);
        indices.Add(baseIdx + 2);
        indices.Add(baseIdx + 3);
    }

    public void UpdateGridColors()
    {
        if (farm == null || farm.Tiles == null || _mesh == null) return;

        var cols = _mesh.colors;
        if (cols == null || cols.Length != _mesh.vertexCount)
            cols = new Color[_mesh.vertexCount];

        // Reset tất cả về màu xanh
        for (int i = 0; i < cols.Length; i++)
            cols[i] = normalColor;

        // Tô đỏ từng ô đã đào
        for (int x = 0; x < _W; x++)
        {
            for (int y = 0; y < _H; y++)
            {
                var tile = farm.Tiles[x, y];
                bool dug = (tile.state == SoilState.Dug) || (tile.soilType != SoilType.None);
                if (!dug) continue;

                PaintCellEdgesRed(cols, x, y);
            }
        }

        _mesh.colors = cols;
    }

    private int HorEdgeVertexIndex(int x, int y)
    {
        // Mỗi segment ngang có 4 vertices (quad)
        return (y * _W + x) * 4;
    }

    private int VerEdgeVertexIndex(int x, int y)
    {
        // Mỗi segment dọc có 4 vertices (quad)
        return _baseVtxVertical + (x * _H + y) * 4;
    }

    private void PaintCellEdgesRed(Color[] cols, int x, int y)
    {
        // 1) Ngang trên: (x..x+1, y)
        if (y >= 0 && y <= _H && x >= 0 && x < _W)
        {
            int idx = HorEdgeVertexIndex(x, y);
            for (int i = 0; i < 4; i++)
                cols[idx + i] = dugColor;
        }

        // 2) Ngang dưới: (x..x+1, y+1)
        if (y + 1 >= 0 && y + 1 <= _H && x >= 0 && x < _W)
        {
            int idx = HorEdgeVertexIndex(x, y + 1);
            for (int i = 0; i < 4; i++)
                cols[idx + i] = dugColor;
        }

        // 3) Dọc trái: (x, y..y+1)
        if (x >= 0 && x <= _W && y >= 0 && y < _H)
        {
            int idx = VerEdgeVertexIndex(x, y);
            for (int i = 0; i < 4; i++)
                cols[idx + i] = dugColor;
        }

        // 4) Dọc phải: (x+1, y..y+1)
        if (x + 1 >= 0 && x + 1 <= _W && y >= 0 && y < _H)
        {
            int idx = VerEdgeVertexIndex(x + 1, y);
            for (int i = 0; i < 4; i++)
                cols[idx + i] = dugColor;
        }
    }

    public void SyncPosition() => transform.position = farm.origin + Vector3.up * yOffset;

    public void SetActiveGrid(bool active)
    {
        if (gameObject.activeSelf != active)
        {
            gameObject.SetActive(active);
        }
    }

    // Hàm tiện ích để thay đổi độ dày từ code
    public void SetLineWidth(float width)
    {
        lineWidth = Mathf.Clamp(width, 0.01f, 0.5f);
        GenerateMesh();
        UpdateGridColors();
    }
}   