using System.Collections.Generic;
using UnityEngine;

public class TreeSeasonMaterial : MonoBehaviour
{
    [Header("Tree Layer")]
    [SerializeField] private string treeLayerName = "Tree";

    [Header("Materials theo mùa")]
    public Material springMaterial;
    public Material summerMaterial;
    public Material autumnMaterial;
    public Material winterMaterial;

    private int treeLayer;
    private List<Renderer> treeRenderers = new List<Renderer>();

    void Start()
    {
        treeLayer = LayerMask.NameToLayer(treeLayerName);
        if (treeLayer == -1)
        {
            Debug.LogError($"Layer '{treeLayerName}' không tồn tại!");
            return;
        }

        Renderer[] allRenderers = FindObjectsByType<Renderer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var r in allRenderers)
        {
            if (r.gameObject.layer == treeLayer)
                treeRenderers.Add(r);
        }

        Debug.Log($"🌲 Đã tìm thấy {treeRenderers.Count} cây trong layer '{treeLayerName}'.");

        ApplyCurrentSeason();
    }

    public void ApplyCurrentSeason()
    {
        if (Season.Instance == null)
        {
            Debug.LogWarning("Season.Instance chưa tồn tại!");
            return;
        }

        SeasonState current = Season.Instance.currentSeason;
        Material newMat = GetMatForSeason(current);
        if (newMat == null)
        {
            Debug.LogWarning($"Không có material cho mùa: {current}");
            return;
        }

        foreach (var r in treeRenderers)
        {
            int count = r.sharedMaterials.Length;
            var mats = new Material[count];
            for (int i = 0; i < count; i++)
                mats[i] = newMat;

            r.sharedMaterials = mats;
        }

        Debug.Log($"🌤 Cập nhật vật liệu cây theo mùa: {current}");
    }

    private Material GetMatForSeason(SeasonState s)
    {
        return s switch
        {
            SeasonState.Spring => springMaterial,
            SeasonState.Summer => summerMaterial,
            SeasonState.Fall => autumnMaterial,
            SeasonState.Winter => winterMaterial,
            _ => null
        };
    }
}
