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
            return;
        }

        var item = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;
        tool = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;
        // === Ray trung tâm để thu hoạch cây ===
        Ray centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(centerRay.origin, centerRay.direction * harvestClickDistance, Color.red);

        if (Physics.Raycast(centerRay, out var hitPlant, harvestClickDistance, plantMask))
        {
            var clickable = hitPlant.collider.GetComponentInParent<PlantClickable>();
            var outline = hitPlant.collider.GetComponentInParent<Outline>();

            if (clickable && Input.GetMouseButtonDown(0))
            {
                // chỉ cần tool nếu bạn bật requireHarvestTool
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
            SetOutline(outline);
        }
        else
        {
            SetOutline(null);
        }

        // === Ray chuột để trúng mặt đất ===
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 1000f, Color.green);

        if (!Physics.Raycast(mouseRay, out var hit, 1000f, gridMask))
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            UIManager.Instance.HideUI("PlantInfo");
            return;
        }

        // 2) Nếu trúng grid đất
        var hitGrid = hit.collider.GetComponentInParent<FarmManager>();
        if (hitGrid != farmManager || !farmManager.IsWorldPointInsideThisGrid(hit.point))
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            UIManager.Instance.HideUI("PlantInfo");
            return;
        }


        gridPos = farmManager.WorldToGrid(hit.point);
        int tx = gridPos.x;
        int ty = gridPos.y;
        if (farmManager.IsInGrid(tx, ty))
        {
            var currentPlant = farmManager.Tiles[tx, ty].plantInstance;
            if (currentPlant != null && PlantInfo.Instance != null)
            {
                // Trạng thái tưới: nếu bạn có hệ thống vùng tưới, có thể dùng SoilManager.IsTileWatered
                bool wateredStr = soilManager != null && soilManager.IsTileWatered(tx, ty) ? true : false;
                if (plantManager.TryGetPlantCenterFrom(tx, ty, out int cx, out int cy))
                {
                    int rd = plantManager.GetRemainingDaysConditioned(currentPlant, cx, cy);
                    // Nếu điều kiện hôm nay không đủ → dùng số ngày "lý tưởng" để UI luôn không âm
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
            else
            {
                UIManager.Instance.HideUI("PlantInfo");
            }
        }


        // Seed
        if (item != null && item.itemData != null && item.itemData.itemType == ItemType.Seed && item.quantity > 0)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();

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
            if (Input.GetMouseButtonDown(0))
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
            return;
        }

        // Hoe/Shovel
        if (item.itemData.toolType == ToolType.Hoe || item.itemData.toolType == ToolType.Shovel)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            soilManager.HandleToolHover(gridPos, item);
            if (Input.GetMouseButtonDown(0))
            {
                if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.hoeState && Mp.Instance.mp >= 10)
                {
                    CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.hoeState);
                }
                else if (Mp.Instance.mp < 10)
                {
                    Notification.Instance.ShowNotification("Hết năng lượng!");
                }
                //soilManager.TryDigOrFlatten(gridPos, tool);
            }
            return;
        }

        // Watering
        if (item.itemData.toolType == ToolType.Watering)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            //if (CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.idleState)
            //{
                if (Input.GetMouseButtonDown(0))
                {

                    //soilManager.TryWaterAt(gridPos);                   
                    if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.wateringState)
                    {
                        CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.wateringState);
                    }
            }
                return;
            //}    
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
