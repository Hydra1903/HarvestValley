using UnityEngine;

public class GhostManager : MonoBehaviour
{
    private GameObject currentGhostInstance;
    private PlantType currentPlantType;
    private Material[] ghostMats;

    // Lưu rotation & "anchor" XZ của ghost hiện tại
    private Quaternion currentRotation = Quaternion.identity;
    private Vector3 lastAnchorXZ = new Vector3(float.NaN, 0f, float.NaN);
    private bool hasRotation = false;

    [SerializeField] private bool snapYToPrefab = true;
    [SerializeField] private bool randomizeRotation = true;

    // Độ nhạy để phát hiện đã chuyển sang tâm khác (trung tâm ô khác)
    [SerializeField] private float anchorEpsilon = 0.0001f;

    public Quaternion CurrentRotation => currentRotation;

    public void Initialize(params Material[] mats) => ghostMats = mats;

    private float GetPrefabY(PlantData pd)
        => (pd && pd.growthPrefabs != null && pd.growthPrefabs.Length > 0 && pd.growthPrefabs[0])
           ? pd.growthPrefabs[0].transform.position.y : 0f;

    public void ShowGhost(PlantData plantData, Vector3 position)
    {
        if (plantData == null || plantData.growthPrefabs == null || plantData.growthPrefabs.Length == 0) return;

        // Tạo mới hoặc đổi loại cây -> reset rotation để random 1 lần
        if (currentGhostInstance == null || currentPlantType != plantData.plantType)
        {
            CreateGhostFromPrefab(plantData);
            hasRotation = false;                    // cho phép random lần đầu
            lastAnchorXZ = new Vector3(float.NaN, 0f, float.NaN);
        }

        if (snapYToPrefab) position.y = GetPrefabY(plantData);

        bool movedCell = float.IsNaN(lastAnchorXZ.x) ||
                         ((new Vector2(position.x, position.z) - new Vector2(lastAnchorXZ.x, lastAnchorXZ.z)).sqrMagnitude > anchorEpsilon);

        if (randomizeRotation && (!hasRotation || movedCell))
        {
            currentRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            hasRotation = true;
            lastAnchorXZ = new Vector3(position.x, 0f, position.z);
        }

        currentGhostInstance.transform.SetPositionAndRotation(position, currentRotation);
        currentGhostInstance.SetActive(true);
    }

    public void ShowGhost(PlantData plantData, Vector3 position, Quaternion rotation)
    {
        if (plantData == null || plantData.growthPrefabs == null || plantData.growthPrefabs.Length == 0) return;

        if (currentGhostInstance == null || currentPlantType != plantData.plantType)
        {
            CreateGhostFromPrefab(plantData);
        }

        if (snapYToPrefab) position.y = GetPrefabY(plantData);

        currentRotation = rotation;     //
        hasRotation = true;
        lastAnchorXZ = new Vector3(position.x, 0f, position.z);

        currentGhostInstance.transform.SetPositionAndRotation(position, currentRotation);
        currentGhostInstance.SetActive(true);
    }

    public void HideGhost()
    {
        if (currentGhostInstance != null) currentGhostInstance.SetActive(false);
    }

    private void CreateGhostFromPrefab(PlantData plantData)
    {
        if (currentGhostInstance != null) DestroyImmediate(currentGhostInstance);

        var src = plantData.growthPrefabs[0];
        currentGhostInstance = Instantiate(src);
        currentGhostInstance.name = $"Ghost_{plantData.plantName}";
        currentPlantType = plantData.plantType;

        DisableUnnecessaryComponents();
        ApplyGhostMaterial();
        currentGhostInstance.SetActive(false);
    }

    private void DisableUnnecessaryComponents()
    {
        foreach (var c in currentGhostInstance.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        foreach (var rb in currentGhostInstance.GetComponentsInChildren<Rigidbody>(true)) rb.isKinematic = true;
        foreach (var s in currentGhostInstance.GetComponentsInChildren<MonoBehaviour>(true))
            if (s != this) s.enabled = false;
    }

    private void ApplyGhostMaterial()
    {
        if (currentGhostInstance == null || ghostMats == null || ghostMats.Length == 0) return;

        var renderers = currentGhostInstance.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            int count = Mathf.Max(1, r.sharedMaterials.Length);
            var mapped = new Material[count];
            for (int i = 0; i < count; i++)
                mapped[i] = i < ghostMats.Length ? ghostMats[i] : ghostMats[ghostMats.Length - 1];
            r.sharedMaterials = mapped;
        }
    }

    private void OnDestroy()
    {
        if (currentGhostInstance != null) DestroyImmediate(currentGhostInstance);
    }
}
