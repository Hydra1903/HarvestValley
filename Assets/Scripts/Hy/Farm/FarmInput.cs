using UnityEngine;

public class FarmInput : MonoBehaviour
{
    [SerializeField] private LayerMask gridMask;

    public void HandleInput(FarmManager farm, SoilManager soil, PlantManager plant)
    {
        var cam = Camera.main;
        if (!cam) { soil.HideGhosts(); plant.HideGhost(); return; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, gridMask))
        {
            soil.HideGhosts(); plant.HideGhost();
            return;
        }

        var hitGrid = hit.collider.GetComponentInParent<FarmManager>();
        if (hitGrid != farm || !farm.IsWorldPointInsideThisGrid(hit.point))
        {
            soil.HideGhosts(); plant.HideGhost();
            return;
        }

        Vector2Int gridPos = farm.WorldToGrid(hit.point);
        var item = farm.hotbarUI ? farm.hotbarUI.currentItem : null;

        // Seed hover + click
        if (item != null && item.itemData != null && item.itemData.itemType == ItemType.Seed && item.quantity > 0)
        {
            soil.HideGhosts(); plant.HideGhost();

            if (plant.HandleSeedHover(gridPos, item))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (plant.TryPlant(gridPos, item))
                    {
                        // trừ 1 hạt
                        if (farm.hotbarUI != null && farm.hotbarUI.hotbar != null)
                        {
                            farm.hotbarUI.hotbar.UseAndRemoveItem(farm.hotbarUI.valueScroll, 1);
                            farm.hotbarUI.UpdateAllSlots();
                        }
                    }
                }
            }
            return;
        }

        // Tool hover + click
        if (item == null || item.itemData == null)
        {
            soil.HideGhosts(); plant.HideGhost();
            return;
        }

        // Hoe/Shovel => Đào/Hủy
        if (item.itemData.toolType == ToolType.Hoe || item.itemData.toolType == ToolType.Shovel)
        {
            soil.HideGhosts(); plant.HideGhost();

            soil.HandleToolHover(gridPos, item);

            if (Input.GetMouseButtonDown(0))
                soil.TryDigOrFlatten(gridPos, item);

            return;
        }

        // Harvest
        if (item.itemData.toolType == ToolType.Harvest)
        {
            soil.HideGhosts(); plant.HideGhost();
            if (Input.GetMouseButtonDown(0))
                plant.TryHarvest(gridPos);
            return;
        }

        soil.HideGhosts(); plant.HideGhost();
    }
}
