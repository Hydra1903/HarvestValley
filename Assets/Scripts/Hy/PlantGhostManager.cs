using UnityEngine;

/// <summary>
/// Quản lý ghost preview cho cây trồng bằng cách sử dụng 1 ghost duy nhất
/// và thay đổi prefab + material động
/// </summary>
public class PlantGhostManager : MonoBehaviour
{
    [Header("Ghost Settings")]
    public Material ghostMaterial; // Material trong suốt/nhạt màu
    
    private GameObject currentGhostInstance;
    private PlantType currentPlantType;
    private Material[] originalMaterials;
    private Renderer[] ghostRenderers;
    
    void Start()
    {
        // Ghost sẽ được tạo động khi cần
    }
    
    /// <summary>
    /// Hiển thị ghost preview cho loại cây tại vị trí chỉ định
    /// </summary>
    public void ShowGhost(PlantType plantType, Vector3 position, GameObject plantPrefab)
    {
        // Nếu loại cây thay đổi hoặc chưa có ghost, tạo mới
        if (currentGhostInstance == null || currentPlantType != plantType)
        {
            CreateGhostFromPrefab(plantType, plantPrefab);
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
    
    /// <summary>
    /// Tạo ghost từ prefab gốc và áp dụng material nhạt màu
    /// </summary>
    void CreateGhostFromPrefab(PlantType plantType, GameObject plantPrefab)
    {
        // Xóa ghost cũ nếu có
        if (currentGhostInstance != null)
        {
            DestroyImmediate(currentGhostInstance);
        }
        
        // Tạo ghost mới từ prefab gốc
        currentGhostInstance = Instantiate(plantPrefab);
        currentGhostInstance.name = $"Ghost_{plantType}";
        currentPlantType = plantType;
        
        // Vô hiệu hóa các component không cần thiết
        DisableUnnecessaryComponents();
        
        // Áp dụng material ghost
        ApplyGhostMaterial();
        
        // Ẩn ghost ban đầu
        currentGhostInstance.SetActive(false);
    }
    
    /// <summary>
    /// Vô hiệu hóa các component không cần thiết cho ghost
    /// </summary>
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
        
        // Vô hiệu hóa các script khác (nếu có)
        MonoBehaviour[] scripts = currentGhostInstance.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) // Không vô hiệu hóa chính script này
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
        
        ghostRenderers = currentGhostInstance.GetComponentsInChildren<Renderer>();
        
        foreach (var renderer in ghostRenderers)
        {
            // Lưu material gốc (không cần thiết cho ghost nhưng để tham khảo)
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