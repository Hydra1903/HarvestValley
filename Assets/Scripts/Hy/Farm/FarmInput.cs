using UnityEngine;

public class FarmInput : MonoBehaviour
{
    [SerializeField] private LayerMask gridMask;            //Layer của đất

    [Header("Raycast")]
    [SerializeField] private LayerMask plantMask;           // Layer của cây
    [SerializeField] private Transform player;              // Player để đo khoảng cách
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

        Ray centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(centerRay.origin, centerRay.direction * harvestClickDistance, Color.red);

        if (Physics.Raycast(centerRay, out var hitPlant, harvestClickDistance, plantMask))
        {
            var clickable = hitPlant.collider.GetComponentInParent<PlantClickable>();
            if (clickable && Input.GetMouseButtonDown(0))
            {
                // chỉ cần tool nếu bạn bật requireHarvestTool
                if (!requireHarvestTool || IsHoldingHarvest(farmManager))
                {
                    if (!player || Vector3.Distance(player.position, hitPlant.point) <= harvestClickDistance)
                    {
                        clickable.ownerFarm.GetComponent<PlantManager>().TryHarvest(new Vector2Int(clickable.centerX, clickable.centerY));
                        return; 
                    }
                    Notification.Instance?.ShowNotification("Quá xa để thu hoạch!");
                }
            }
        }
       
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        var item = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;
        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 1000f, Color.green);

        if (!Physics.Raycast(mouseRay, out var hit, 1000f, gridMask))
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            return;
        }

        // 2) Nếu trúng grid đất
        var hitGrid = hit.collider.GetComponentInParent<FarmManager>();
        if (hitGrid != farmManager || !farmManager.IsWorldPointInsideThisGrid(hit.point))
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            return;
        }

        Vector2Int gridPos = farmManager.WorldToGrid(hit.point);
        

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
            if (Input.GetMouseButtonDown(0))
            {
                plantManager.TryHarvest(gridPos);
            }
            return;
        }

        // Hoe/Shovel
        if (item.itemData.toolType == ToolType.Hoe || item.itemData.toolType == ToolType.Shovel)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            soilManager.HandleToolHover(gridPos, item);
            if (Input.GetMouseButtonDown(0))
            {
                CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.hoeState);
                soilManager.TryDigOrFlatten(gridPos, item);
            }
            return;
        }

        // Watering
        if (item.itemData.toolType == ToolType.Watering)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            if (Input.GetMouseButtonDown(0))
            {
                soilManager.TryWaterAt(gridPos);
                CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.wateringState);
            }
            return;
        }

        // Sprinkler
        if (item.itemData.toolType == ToolType.Sprinkler)
        {
            soilManager.HideGhosts(); plantManager.HideGhost();
            soilManager.ShowSprinklerGhost(gridPos);

            if (Input.GetMouseButtonDown(0))
            {
                if (soilManager.PlaceSprinkler(gridPos, item.itemData.placeablePrefab, 1))
                {
                    farmManager.hotbarUI?.hotbar?.UseAndRemoveItem(farmManager.hotbarUI.valueScroll, 1);
                    farmManager.hotbarUI?.UpdateAllSlots();
                }
                else
                {
                    Notification.Instance?.ShowNotification("Không thể đặt Sprinkler ở khu đất này!");
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
