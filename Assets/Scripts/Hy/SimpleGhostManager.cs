using UnityEngine;

/// <summary>
/// Quản lý ghost preview đơn giản cho cây trồng
/// </summary>
public class SimpleGhostManager : MonoBehaviour
{
    private GameObject currentGhostInstance;
    private PlantType currentPlantType;
    private Material ghostMaterial;
    
    public void Initialize(Material ghostMat)
    {
        ghostMaterial = ghostMat;
    }
    
    /// <summary>
    /// Hiển thị ghost preview cho loại cây tại vị trí chỉ định
    /// </summary>
    public void ShowGhost(PlantData plantData, Vector3 position)
    {
        if (plantData == null || plantData.prefab == null) return;
        
        // Nếu loại cây thay đổi hoặc chưa có ghost, tạo mới
        if (currentGhostInstance == null || currentPlantType != plantData.plantType)
        {
            CreateGhostFromPrefab(plantData);
        }
        
        if (currentGhostInstance != null)
        {
            currentGhostInstance.transform.position = position;
            currentGhostInstance.SetActive(true);
        }
    }
    
    /// <summary>
    /// Ẩn ghost preview
    /// </summary>
    public void HideGhost()
    {
        if (currentGhostInstance != null)
        {
            currentGhostInstance.SetActive(false);
        }
    }

    /// Tạo ghost từ prefab gốc và áp dụng material nhạt màu
    void CreateGhostFromPrefab(PlantData plantData)
    {
        // Xóa ghost cũ nếu có
        if (currentGhostInstance != null)
        {
            DestroyImmediate(currentGhostInstance);
        }
        
        // Tạo ghost mới từ prefab gốc
        currentGhostInstance = Instantiate(plantData.prefab);
        currentGhostInstance.name = $"Ghost_{plantData.plantName}";
        currentPlantType = plantData.plantType;
        
        // Vô hiệu hóa các component không cần thiết
        DisableUnnecessaryComponents();
        
        // Áp dụng material ghost
        ApplyGhostMaterial();
        
        currentGhostInstance.SetActive(false);
    }
    
    /// Vô hiệu hóa các component không cần thiết cho ghost
    void DisableUnnecessaryComponents()
    {
        if (currentGhostInstance == null) return;
        
        // Vô hiệu hóa collider
        Collider[] colliders = currentGhostInstance.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        
        // Vô hiệu hóa rigidbody
        Rigidbody[] rigidbodies = currentGhostInstance.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        
        // Vô hiệu hóa các script khác
        MonoBehaviour[] scripts = currentGhostInstance.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Áp dụng material ghost cho tất cả renderer
    /// </summary>
    void ApplyGhostMaterial()
    {
        if (currentGhostInstance == null || ghostMaterial == null) return;
        
        Renderer[] renderers = currentGhostInstance.GetComponentsInChildren<Renderer>();
        
        foreach (var renderer in renderers)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ghostMaterial;
            }
            renderer.materials = materials;
        }
    }
    
    /// <summary>
    /// Cleanup khi destroy
    /// </summary>
    void OnDestroy()
    {
        if (currentGhostInstance != null)
        {
            DestroyImmediate(currentGhostInstance);
        }
    }
}