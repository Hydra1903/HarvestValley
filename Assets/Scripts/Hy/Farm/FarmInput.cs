using UnityEngine;

public class FarmInput : MonoBehaviour
{
    [SerializeField] private LayerMask gridMask;            //Layer của đất

    [Header("Raycast")]
    [SerializeField] private LayerMask plantMask;           // Layer của cây
    [SerializeField] private Transform player;              // Player để đo khoảng cách
    [SerializeField] private float harvestClickDistance = 2.5f;
    [SerializeField] private bool requireHarvestTool = false;  // true: phải cầm tool Harvest mới được click

    [Header("Outline")]
    [SerializeField, Range(0, 10)] private float hoverOutlineWidth;
    private Outline currentHoverOutline;

    public SoilManager soilManager;
    public PlantManager plantManager;
    public FarmManager farmManager;
    public FarmGrid farmGrid;

    private Camera cam;

    [HideInInspector]
    public Vector2Int gridPos;
    [HideInInspector]
    public InventoryItem tool;


    private void Awake()
    {
        cam = Camera.main;
    }

    public void HandleInput()
    {
        if (!cam)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            farmGrid.SetActiveGrid(false);
            return;
        }

        var item = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;
        tool = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;

        // === Ray trung tâm để thu hoạch cây ===
        Ray centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(centerRay.origin, centerRay.direction * harvestClickDistance, Color.red);

        bool hitPlantForHarvest = false;

        if (Physics.Raycast(centerRay, out var hitPlant, harvestClickDistance, plantMask))
        {
            var clickable = hitPlant.collider.GetComponentInParent<PlantClickable>();
            var outline = hitPlant.collider.GetComponentInParent<Outline>();

            SetOutline(outline);
            hitPlantForHarvest = true;

            if (clickable != null)
            {
                int x = clickable.centerX;
                int y = clickable.centerY;

                if (farmManager.IsInGrid(x, y))
                {
                    var currentPlant = farmManager.Tiles[x, y].plantInstance;
                    if (currentPlant != null && PlantInfo.Instance != null)
                    {
                        bool wateredStr = soilManager != null && soilManager.IsTileWatered(x, y);
                        if (plantManager.TryGetPlantCenterFrom(x, y, out int cx, out int cy))
                        {
                            int rd = plantManager.GetRemainingDaysConditioned(currentPlant, cx, cy);
                            currentPlant.remainingDays = (rd >= 0) ? rd : plantManager.GetRemainingDays(currentPlant);
                        }
                        PlantInfo.Instance.SetInfo(
                            currentPlant.plantData.plantType,
                            currentPlant.plantData.plantName,
                            currentPlant.remainingDays,
                            wateredStr
                        );
                        UIManager.Instance.ShowUI("PlantInfo");
                    }
                }
            }

            if (clickable && Input.GetMouseButtonDown(0))
            {
                if (!requireHarvestTool || IsHoldingHarvest(farmManager))
                {
                    if (!player || Vector3.Distance(player.position, hitPlant.point) <= harvestClickDistance)
                    {
                        if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.harvestLowState && Mp.Instance.mp >= 10)
                        {
                            CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.harvestLowState);
                        }
                        else if (Mp.Instance.mp < 10)
                        {
                            Notification.Instance.ShowNotification("Hết năng lượng!");
                        }
                        return;
                    }
                    Notification.Instance?.ShowNotification("Quá xa để thu hoạch!");
                }
            }
        }
        else
        {
            SetOutline(null);
            UIManager.Instance.HideUI("PlantInfo");
        }

        // === Ray chuột để trúng mặt đất (cho các tool khác) ===
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 1000f, Color.green);

        if (!Physics.Raycast(mouseRay, out var hit, 1000f, gridMask))
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            farmGrid.SetActiveGrid(false);
            if (!hitPlantForHarvest)
                UIManager.Instance.HideUI("PlantInfo");
            return;
        }

        gridPos = farmManager.WorldToGrid(hit.point);
        int tx = gridPos.x;
        int ty = gridPos.y;

        // Seed
        if (item != null && item.itemData != null && item.itemData.itemType == ItemType.Seed && item.quantity > 0)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            farmGrid.SetActiveGrid(false);

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
            soilManager.HideGhosts();
            plantManager.HideGhost();
            farmGrid.SetActiveGrid(false);

            if (Input.GetMouseButtonDown(0))
            {
                if (!plantManager.CanStartHarvest(gridPos))
                {
                    Notification.Instance.ShowNotification("Không thể thu hoạch");
                    return;
                }

                if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.harvestLowState && Mp.Instance.mp >= 10)
                {
                    CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.harvestLowState);
                }
                else if (Mp.Instance.mp < 10)
                {
                    Notification.Instance.ShowNotification("Hết năng lượng!");
                }
                return;
            }
            return;
        }

        // Hoe/Shovel
        if (item.itemData.toolType == ToolType.Hoe)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            soilManager.HandleToolHover(gridPos, item);
            farmGrid.SetActiveGrid(true);

            if (Input.GetMouseButtonDown(0))
            {
                if (!soilManager.CanStartHoeAt(gridPos, item))
                {
                    Notification.Instance?.ShowNotification("Không thể dùng cuốc tại đây!");
                    return;
                }

                if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.hoeState)
                    CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.hoeState);
            }
            return;
        }

        // Watering
        if (item.itemData.toolType == ToolType.Watering)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            farmGrid.SetActiveGrid(false);

            if (Input.GetMouseButtonDown(0))
            {
                if (!soilManager.CanStartWaterAt(gridPos))
                {                   
                    return;
                }

                if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.wateringState)
                    CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.wateringState);
            }
            return;
        }

        // Sprinkler
        if (item.itemData.toolType == ToolType.Sprinkler)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
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

        soilManager.HideGhosts();
        plantManager.HideGhost();
    }

    private bool IsHoldingHarvest(FarmManager farm)
    {
        var item = farm.hotbarUI ? farm.hotbarUI.currentItem : null;
        return item != null && item.itemData != null && item.itemData.toolType == ToolType.Harvest;
    }

    private void SetOutline(Outline next)
    {
        if (next == currentHoverOutline) return;

        if (currentHoverOutline)
            currentHoverOutline.OutlineWidth = 0f;

        currentHoverOutline = next;

        if (currentHoverOutline)
        {
            currentHoverOutline.OutlineMode = Outline.Mode.OutlineVisible;
            currentHoverOutline.OutlineWidth = hoverOutlineWidth;
        }
    }
}