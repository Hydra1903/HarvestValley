using System.Collections.Generic;
using UnityEngine;

public class GridFarm : MonoBehaviour
{
    [Header("References")]
    public FarmManager farm;
    [Header("Materials")]
    public Material allowedMat; // xanh
    public Material deniedMat;  // đỏ
    [Header("Visuals")]
    public float yOffset = 0.05f;
    public float quadScale = 0.98f;

    private readonly List<GameObject> pool = new();
    private bool isVisible;

    private void Awake()
    {
        if (!farm) farm = GetComponentInParent<FarmManager>();
    }

    private GameObject SpawnQuad()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "GridHighlightQuad";
        go.transform.SetParent(transform, false);
        DestroyImmediate(go.GetComponent<Collider>());
        var mr = go.GetComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        go.SetActive(false);
        pool.Add(go);
        return go;
    }

    private GameObject GetFromPool()
    {
        foreach (var g in pool) if (!g.activeSelf) return g;
        return SpawnQuad();
    }

    public void HideAll()
    {
        foreach (var g in pool)
            g.SetActive(false);
        isVisible = false;
    }

    /// <summary>
    /// Hiển thị grid tại vùng quanh raycast (tâm = gridPos).
    /// Ô đỏ = đã đào, Ô xanh = có thể đào.
    /// </summary>
    public void ShowRaycastArea(Vector2Int center, int size)
    {
        if (farm == null || farm.Tiles == null) return;

        isVisible = true;
        float c = farm.cellSize;
        int half = size / 2;
        int used = 0;

        for (int dx = -half; dx <= half; dx++)
        {
            for (int dy = -half; dy <= half; dy++)
            {
                int gx = center.x + dx;
                int gy = center.y + dy;
                if (!farm.IsInGrid(gx, gy)) continue;

                var quad = GetFromPool();
                quad.SetActive(true);

                quad.transform.position = farm.origin + new Vector3(
                    (gx + 0.5f) * c,
                    yOffset,
                    (gy + 0.5f) * c
                );
                quad.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                quad.transform.localScale = Vector3.one * (c * quadScale);

                var mr = quad.GetComponent<MeshRenderer>();
                var tile = farm.Tiles[gx, gy];

                bool dug = tile.state == SoilState.Dug || tile.soilType != SoilType.None;
                mr.sharedMaterial = dug ? deniedMat : allowedMat;

                used++;
            }
        }

        // Ẩn các ô thừa
        for (int i = used; i < pool.Count; i++)
            pool[i].SetActive(false);
    }
}
