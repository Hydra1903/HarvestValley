using UnityEngine;

public class FarmInput : MonoBehaviour
{
    [SerializeField] private LayerMask gridMask;

    [Header("Raycast")]
    [SerializeField] private LayerMask plantMask;          // Layer của cây/Hitbox
    [SerializeField] private Transform player;             // Player để đo khoảng cách (nếu cần)
    [SerializeField] private float harvestClickDistance = 2.5f;
    [SerializeField] private bool requireHarvestTool = false;  // true: phải cầm tool Harvest mới được click

    public SoilManager soilManager;
    public PlantManager plantManager;
    public FarmManager farmManager;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void HandleInput()
    {

        if (!cam) { soilManager.HideGhosts(); plantManager.HideGhost(); return; }

        // Ray từ tâm camera
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * harvestClickDistance, Color.red);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, gridMask | plantMask))
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            return;
        }

        var clickable = hit.collider.GetComponentInParent<PlantClickable>();
        if (Physics.Raycast(ray, out RaycastHit hitPlant, harvestClickDistance, plantMask))
        {
            if (clickable != null && Input.GetMouseButtonDown(0))
            {
                if (requireHarvestTool)
                {
                    var toolitem = clickable.ownerFarm.hotbarUI?.currentItem;
                    if (toolitem == null || toolitem.itemData == null || toolitem.itemData.toolType != ToolType.Harvest)
                        return;
                }

                if (player != null)
                {
                    float dist = Vector3.Distance(player.position, hitPlant.point);
                    if (dist > harvestClickDistance)
                    {
                        Notification.Instance?.ShowNotification("Quá xa để thu hoạch!");
                        return;
                    }
                }

                clickable.ownerFarm.GetComponent<PlantManager>()
                    .TryHarvest(new Vector2Int(clickable.centerX, clickable.centerY));
                return;
            }
        }

        // 1) Nếu trúng cây

        if (clickable != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Nếu cần bắt buộc cầm tool Harvest
                if (!requireHarvestTool || IsHoldingHarvest(farmManager))
                {
                    float dist = player ? Vector3.Distance(player.position, hit.point) : 0f;
                    if (player == null || dist <= harvestClickDistance)
                    {
                        plantManager.TryHarvest(new Vector2Int(clickable.centerX, clickable.centerY));
                    }
                    else
                    {
                        Notification.Instance?.ShowNotification("Quá xa để thu hoạch!");
                    }
                }
            }
            return; // nếu trúng cây thì không xử lý đất nữa
        }

        // 2) Nếu trúng grid đất
        var hitGrid = hit.collider.GetComponentInParent<FarmManager>();
        if (hitGrid != farmManager || !farmManager.IsWorldPointInsideThisGrid(hit.point))
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            return;
        }

        Vector2Int gridPos = farmManager.WorldToGrid(hit.point);
        var item = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;

        // Seed
        if (item != null && item.itemData != null && item.itemData.itemType == ItemType.Seed && item.quantity > 0)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();

            if (plantManager.HandleSeedHover(gridPos, item))
            {
                if (Input.GetMouseButtonDown(0) && plantManager.TryPlant(gridPos, item))
                {
                    farmManager.hotbarUI?.hotbar?.UseAndRemoveItem(farmManager.hotbarUI.valueScroll, 1);
                    farmManager.hotbarUI?.UpdateAllSlots();
                }
            }
            return;
        }

        // Không có item
        if (item == null || item.itemData == null)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            return;
        }

        // Hoe/Shovel
        if (item.itemData.toolType == ToolType.Hoe || item.itemData.toolType == ToolType.Shovel)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            soilManager.HandleToolHover(gridPos, item);
            if (Input.GetMouseButtonDown(0))
                soilManager.TryDigOrFlatten(gridPos, item);

            return;
        }

        // Harvest theo grid (nếu muốn giữ song song)
        if (item.itemData.toolType == ToolType.Harvest)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            if (Input.GetMouseButtonDown(0))
                plantManager.TryHarvest(gridPos);
            return;
        }

        // Watering
        if (item.itemData.toolType == ToolType.Watering)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            if (Input.GetMouseButtonDown(0))
                soilManager.TryWaterAt(gridPos);
            return;
        }

        // Sprinkler
        if (item.itemData.toolType == ToolType.Sprinkler)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            soilManager.ShowSprinklerGhost(gridPos);

            if (Input.GetMouseButtonDown(0))
            {
                int gx = gridPos.x, gy = gridPos.y;
                if (farmManager.IsInGrid(gx, gy))
                {
                    var prefab = item.itemData.placeablePrefab;
                    if (prefab != null)
                    {
                        Vector3 pos = farmManager.origin + new Vector3(
                            (gx + 0.5f) * farmManager.cellSize,
                            prefab.transform.position.y,
                            (gy + 0.5f) * farmManager.cellSize
                        );

                        var go = Instantiate(prefab, pos, Quaternion.identity);
                        var sp = go.GetComponent<Sprinkler>();
                        if (sp == null) sp = go.AddComponent<Sprinkler>();
                        sp.Init(gx, gy, 7);
                        soilManager.GetSprinklers(sp);

                        farmManager.hotbarUI?.hotbar?.UseAndRemoveItem(farmManager.hotbarUI.valueScroll, 1);
                        farmManager.hotbarUI?.UpdateAllSlots();
                    }
                }
            }
            return;
        }

        soilManager.HideGhosts(); plantManager.HideGhost();
    }

    private bool IsHoldingHarvest(FarmManager farm)
    {
        var item = farm.hotbarUI ? farm.hotbarUI.currentItem : null;
        return item != null && item.itemData != null && item.itemData.toolType == ToolType.Harvest;
    }

}
