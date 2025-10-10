using System.Collections;
using UnityEngine;

public class GrassManager : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask dirtGrassLayer; // Layer của DirtGrass
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private float timeHarvest = 5f;

    [Header("Tool Constraint")]
    [SerializeField] private bool requireHarvestTool = true; // Chỉ hoạt động khi cầm công cụ Harvest
    [SerializeField] private HotBarUI hotbar; // Để kiểm tra công cụ đang cầm

    [Header("Debug")]
    [SerializeField] private bool debugRay = true;

    void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // Chỉ hoạt động khi click chuột trái
        if (!Input.GetMouseButtonDown(0))
            return;

        // Nếu cần kiểm tra công cụ hiện tại
        if (requireHarvestTool && !IsHoldingHarvestTool())
            return;

        // Tạo ray từ camera
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (debugRay)
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.blue, 0.3f);
            

        // Kiểm tra va chạm với DirtGrass
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, dirtGrassLayer))
        {
            CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.mowingState);
            StartCoroutine(Harvest());
            Debug.Log($"Hit DirtGrass: {hit.collider.name}");

            // Tắt tất cả các GameObject có tag "Grass"
           
        }
    }

    private IEnumerator Harvest()
    {
        yield return new WaitForSeconds(timeHarvest);
        GameObject[] allGrass = GameObject.FindGameObjectsWithTag("Grass");
        foreach (var g in allGrass)
        {
            if (g.activeSelf)
            {
                g.SetActive(false);
                Debug.Log($"Tắt cỏ: {g.name}");
            }
        }
    }

    private bool IsHoldingHarvestTool()
    {
        if (!hotbar || hotbar.currentItem == null || hotbar.currentItem.itemData == null)
            return false;

        return hotbar.currentItem.itemData.toolType == ToolType.Harvest;
    }
}
