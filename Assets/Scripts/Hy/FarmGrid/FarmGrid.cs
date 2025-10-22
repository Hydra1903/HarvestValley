using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FarmGrid : MonoBehaviour
{
    [Header("References")]
    public FarmManager farm;             // Kéo FarmManager vào (hoặc để tự tìm ở parent)

    [Header("Colors")]
    public Color normalColor = new Color(0f, 1f, 0f, 0.65f); // Xanh viền
    public Color dugColor = new Color(1f, 0f, 0f, 0.85f); // Đỏ viền

    [Header("Visual")]
    [Tooltip("Nâng lưới lên khỏi mặt đất để tránh z-fighting")]
    public float yOffset = 0.15f;

    private MeshFilter _mf;
    private MeshRenderer _mr;
    private Mesh _mesh;

    // bộ đếm để tính nhanh
    private int _W, _H;                 // gridWidth, gridHeight
    private int _hSegments;             // số segment ngang  = (H+1)*W
    private int _vSegments;             // số segment dọc    = (W+1)*H
    private int _baseVtxVertical;       // offset vertex phần dọc = _hSegments * 2

    // ----------------------------------------------------------------------

    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (!farm) farm = GetComponentInParent<FarmManager>();
        EnsureComponents();
        GenerateMesh();                 // tạo lưới một lần
        UpdateGridColors();             // tô màu theo trạng thái hiện tại
    }

    private void OnValidate()
    {
        if (!farm) farm = GetComponentInParent<FarmManager>();
        EnsureComponents();
    }

    private void EnsureComponents()
    {
        _mf = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();

        // Dùng shader hỗ trợ Vertex Color để mesh.colors có tác dụng
        if (_mr.sharedMaterial == null || _mr.sharedMaterial.shader == null)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = Color.white;            // để nhìn đúng màu theo vertex color
            mat.renderQueue = 3000;             // vẽ sau mặt đất
            _mr.sharedMaterial = mat;
        }

        _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _mr.receiveShadows = false;
    }

    // ----------------------------------------------------------------------
    // PUBLIC API
    // ----------------------------------------------------------------------

    /// <summary>Tạo lại lưới (gọi khi đổi gridWidth/gridHeight/cellSize)</summary>
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

        // ------- TẠO SEGMENT NGANG: (H+1) hàng, mỗi hàng có W segment -------
        // Mỗi segment là 2 vertex (a->b). Ta tạo riêng từng segment để có thể tô màu riêng.
        for (int y = 0; y <= _H; y++)
        {
            for (int x = 0; x < _W; x++)
            {
                Vector3 a = new Vector3(x * c, 0f, y * c);
                Vector3 b = new Vector3((x + 1) * c, 0f, y * c);

                int i0 = verts.Count;
                verts.Add(a); verts.Add(b);
                colors.Add(normalColor); colors.Add(normalColor);
                indices.Add(i0); indices.Add(i0 + 1);
            }
        }
        _hSegments = (_H + 1) * _W;

        // ------- TẠO SEGMENT DỌC: (W+1) cột, mỗi cột có H segment -------
        for (int x = 0; x <= _W; x++)
        {
            for (int y = 0; y < _H; y++)
            {
                Vector3 a = new Vector3(x * c, 0f, y * c);
                Vector3 b = new Vector3(x * c, 0f, (y + 1) * c);

                int i0 = verts.Count;
                verts.Add(a); verts.Add(b);
                colors.Add(normalColor); colors.Add(normalColor);
                indices.Add(i0); indices.Add(i0 + 1);
            }
        }
        _vSegments = (_W + 1) * _H;
        _baseVtxVertical = _hSegments * 2; // mỗi segment có 2 vertex

        _mesh.SetVertices(verts);
        _mesh.SetIndices(indices, MeshTopology.Lines, 0);
        _mesh.SetColors(colors);

        _mf.sharedMesh = _mesh;
        transform.position = farm.origin + Vector3.up * yOffset;   // đồng bộ vị trí
    }

    /// <summary>
    /// Tô đỏ viền tại các ô đã đào (Dug) / có soilType khác None. Gọi hàm này sau Place/Flatten.
    /// </summary>
    public void UpdateGridColors()
    {
        if (farm == null || farm.Tiles == null || _mesh == null) return;

        // reset tất cả về màu xanh
        var cols = _mesh.colors;
        if (cols == null || cols.Length != _mesh.vertexCount)
            cols = new Color[_mesh.vertexCount];
        for (int i = 0; i < cols.Length; i++) cols[i] = normalColor;

        // tô đỏ từng ô đã đào
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

    // ----------------------------------------------------------------------
    // INDEXING: ánh xạ (x,y) -> index cặp vertex của 1 cạnh
    // Layout vertex:
    //  [0 .. hSegments*2-1]  : tất cả cạnh ngang (mỗi cạnh 2 vertex)
    //  [baseVtxVertical .. ] : tất cả cạnh dọc
    // ----------------------------------------------------------------------

    private int HorEdgeVertexIndex(int x, int y)   // cạnh ngang (x..x+1, y), 0 <= x < W, 0 <= y <= H
    {
        // mỗi hàng ngang có W segment, mỗi segment = 2 vertex
        return (y * _W + x) * 2;
    }

    private int VerEdgeVertexIndex(int x, int y)   // cạnh dọc (x, y..y+1), 0 <= x <= W, 0 <= y < H
    {
        // mỗi cột dọc có H segment, offset tổng cho phần dọc = _baseVtxVertical
        return _baseVtxVertical + (x * _H + y) * 2;
    }

    private void PaintCellEdgesRed(Color[] cols, int x, int y)
    {
        // BỐN CẠNH VIỀN CỦA Ô (x,y)
        // 1) Ngang trên  : (x..x+1, y)
        if (y >= 0 && y <= _H && x >= 0 && x < _W)
        {
            int i = HorEdgeVertexIndex(x, y);
            cols[i] = dugColor; cols[i + 1] = dugColor;
        }

        // 2) Ngang dưới  : (x..x+1, y+1)
        if (y + 1 >= 0 && y + 1 <= _H && x >= 0 && x < _W)
        {
            int i = HorEdgeVertexIndex(x, y + 1);
            cols[i] = dugColor; cols[i + 1] = dugColor;
        }

        // 3) Dọc trái    : (x, y..y+1)
        if (x >= 0 && x <= _W && y >= 0 && y < _H)
        {
            int i = VerEdgeVertexIndex(x, y);
            cols[i] = dugColor; cols[i + 1] = dugColor;
        }

        // 4) Dọc phải    : (x+1, y..y+1)
        if (x + 1 >= 0 && x + 1 <= _W && y >= 0 && y < _H)
        {
            int i = VerEdgeVertexIndex(x + 1, y);
            cols[i] = dugColor; cols[i + 1] = dugColor;
        }
    }

    // ----------------------------------------------------------------------
    // Tiện ích
    // ----------------------------------------------------------------------

    /// <summary>Gọi khi đổi origin/yOffset để căn lại vị trí</summary>
    public void SyncPosition() => transform.position = farm.origin + Vector3.up * yOffset;
    public void SetActiveGrid(bool active)
    {

        if (gameObject.activeSelf != active)
        {
            gameObject.SetActive(active);
        }
    }
}
