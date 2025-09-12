using UnityEngine;

public class FarmInput : MonoBehaviour
{
    [SerializeField] private LayerMask gridMask;

    [Header("Raycast")]
    [SerializeField] private LayerMask plantMask;          // Layer của cây/Hitbox
    [SerializeField] private Transform player;             // Player để đo khoảng cách (nếu cần)
    [SerializeField] private float harvestClickDistance = 2.5f;
    [SerializeField] private bool requireHarvestTool = false;  // true: phải cầm tool Harvest mới được click

    public void HandleInput(FarmManager farm, SoilManager soil, PlantManager plant)
    {
        var cam = Camera.main;
        if (!cam) { soil.HideGhosts(); plant.HideGhost(); return; }

        // Ray từ tâm camera
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * harvestClickDistance, Color.red);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, gridMask | plantMask))
        {
            soil.HideGhosts(); plant.HideGhost();
            return;
        }

        // 1) Nếu trúng cây
        var clickable = hit.collider.GetComponentInParent<PlantClickable>();
        if (clickable != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Nếu cần bắt buộc cầm tool Harvest
                if (!requireHarvestTool || IsHoldingHarvest(farm))
                {
                    float dist = player ? Vector3.Distance(player.position, hit.point) : 0f;
                    if (player == null || dist <= harvestClickDistance)
                    {
                        PlantManager.Instance.TryHarvest(new Vector2Int(clickable.centerX, clickable.centerY));
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
        if (hitGrid != farm || !farm.IsWorldPointInsideThisGrid(hit.point))
        {
            soil.HideGhosts(); plant.HideGhost();
            return;
        }

        Vector2Int gridPos = farm.WorldToGrid(hit.point);
        var item = farm.hotbarUI ? farm.hotbarUI.currentItem : null;

        // Seed
        if (item != null && item.itemData != null && item.itemData.itemType == ItemType.Seed && item.quantity > 0)
        {
            soil.HideGhosts(); plant.HideGhost();

            if (plant.HandleSeedHover(gridPos, item))
            {
                if (Input.GetMouseButtonDown(0) && plant.TryPlant(gridPos, item))
                {
                    farm.hotbarUI?.hotbar?.UseAndRemoveItem(farm.hotbarUI.valueScroll, 1);
                    farm.hotbarUI?.UpdateAllSlots();
                }
            }
            return;
        }

        // Không có item
        if (item == null || item.itemData == null)
        {
            soil.HideGhosts(); plant.HideGhost();
            return;
        }

        // Hoe/Shovel
        if (item.itemData.toolType == ToolType.Hoe || item.itemData.toolType == ToolType.Shovel)
        {
            soil.HideGhosts(); plant.HideGhost();

            soil.HandleToolHover(gridPos, item);
            if (Input.GetMouseButtonDown(0))
                soil.TryDigOrFlatten(gridPos, item);

            return;
        }

        // Harvest theo grid (nếu muốn giữ song song)
        if (item.itemData.toolType == ToolType.Harvest)
        {
            soil.HideGhosts(); plant.HideGhost();
            if (Input.GetMouseButtonDown(0))
                plant.TryHarvest(gridPos);
            return;
        }

        // Watering
        if (item.itemData.toolType == ToolType.Watering)
        {
            soil.HideGhosts(); plant.HideGhost();
            if (Input.GetMouseButtonDown(0))
                soil.TryWaterAt(gridPos);
            return;
        }

        //Sprinkler
        if (item.itemData.toolType == ToolType.Sprinkler)
        {
            soil.HideGhosts(); plant.HideGhost();
            if (Input.GetMouseButtonDown(0))
            {
                int gx = gridPos.x;
                int gy = gridPos.y;
                if (farm.IsInGrid(gx, gy))
                {
                    var t = farm.Tiles[gx, gy];
                    if (t.plantInstance == null) // trống thì cho đặt
                    {
                        var prefab = item.itemData.placeablePrefab;
                        if (prefab != null)
                        {
                            Vector3 pos = farm.origin + new Vector3(
                                (gx + 0.5f) * farm.cellSize,
                                prefab.transform.position.y,
                                (gy + 0.5f) * farm.cellSize
                            );

                            var go = Instantiate(prefab, pos, Quaternion.identity);
                            var sp = go.GetComponent<Sprinkler>();
                            if (sp == null) sp = go.AddComponent<Sprinkler>();
                            sp.Init(gx, gy, 7); // 15×15 vùng tưới
                            SoilManager.Instance.RegisterSprinkler(sp);

                            // trừ vật phẩm nếu cần
                            farm.hotbarUI?.hotbar?.UseAndRemoveItem(farm.hotbarUI.valueScroll, 1);
                            farm.hotbarUI?.UpdateAllSlots();
                        }
                    }
                }
            }
            return;
        }

        soil.HideGhosts(); plant.HideGhost();
    }

    private bool IsHoldingHarvest(FarmManager farm)
    {
        var item = farm.hotbarUI ? farm.hotbarUI.currentItem : null;
        return item != null && item.itemData != null && item.itemData.toolType == ToolType.Harvest;
    }

}
