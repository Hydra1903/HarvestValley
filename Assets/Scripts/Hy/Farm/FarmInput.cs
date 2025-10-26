using UnityEngine;
public class FarmInput : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private LayerMask gridMask;
    [SerializeField] private LayerMask plantMask;           // Layer của cây
    [SerializeField] private LayerMask soilMask;

    [SerializeField] private Transform player;              // Player để đo khoảng cách
    [SerializeField, Min(0f)] private float interactDistance = 2.0f;
    [SerializeField] private bool requireHarvestTool = false;  // true: phải cầm tool Harvest mới được click

    [Header("Hand Mode")]
    [SerializeField] private HandMode currentHandMode = HandMode.Harvest;

    [Header("Plant Outline")]
    [SerializeField, Range(0, 10)] private float hoverOutlineWidth;
    private Outline currentHoverOutline;
    [Header("Soil Outline")]
    [SerializeField, Range(0, 10)] private float soilOutlineWidth;
    private Outline currentSoilOutline; 

    public SoilManager soilManager;
    public PlantManager plantManager;
    public FarmManager farmManager;
    public SoilGrid soilGrid;
    public PlantGrid plantGrid;
    private Camera cam;

    [HideInInspector]
    public Vector2Int gridPos;
    [HideInInspector]
    public InventoryItem tool;


    private void Awake()
    {
        cam = Camera.main;
    }
    public HandMode GetHandMode()
    {
        return currentHandMode;
    }

    public void HandleInput()
    {
        if (!cam)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            soilGrid.SetActiveGrid(false);
            return;
        }
        int combinedMask = gridMask | soilMask ;
        var item = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;
        tool = farmManager.hotbarUI ? farmManager.hotbarUI.currentItem : null;

        // === Ray trung tâm để tương tác với cây ===
        Ray centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(centerRay.origin, centerRay.direction * interactDistance, Color.red);

        bool hitPlantForHarvest = false;

        if (Physics.Raycast(centerRay, out var hitPlant, interactDistance, plantMask))
        {
            var clickable = hitPlant.collider.GetComponentInParent<PlantClickable>();
            var outline = hitPlant.collider.GetComponentInParent<Outline>();

            if (!CheckDistance(hitPlant.point, interactDistance)) return;
            
            // Chỉ hiện outline khi có mode active (không phải None)
            if (item == null && currentHandMode != HandMode.None)
            {
                SetPlantOutline(outline);
            }
            else if (item != null && requireHarvestTool && IsHoldingHarvest(farmManager))
            {
                SetPlantOutline(outline);
            }
            else if (item == null && currentHandMode == HandMode.None)
            {
                SetPlantOutline(null);
            }

            hitPlantForHarvest = true;

            // Hiển thị thông tin cây khi hover vào cây
            if (clickable != null)
            {
                int tx = clickable.centerX;
                int ty = clickable.centerY;

                if (farmManager.IsInGrid(tx, ty))
                {
                    if (!CheckDistance(hitPlant.point, interactDistance)) return;

                    var currentPlant = farmManager.Tiles[tx, ty].plantInstance;
                    if (currentPlant != null && PlantInfo.Instance != null)
                    {
                        bool wateredStr = soilManager != null && soilManager.IsTileWatered(tx, ty);
                        if (plantManager.TryGetPlantCenterFrom(tx, ty, out int cx, out int cy))
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

            // Xử lý click vào cây khi không cầm tool
            if (clickable && Input.GetMouseButtonDown(0) && item == null)
            {
                if (!CheckDistance(hitPlant.point, interactDistance)) return;

                if (plantManager.CanPlantRemove(gridPos) || plantManager.CanStartHarvest(gridPos))
                {
                    HandlePlantClick(clickable, hitPlant.point);
                }
                return;
            }
        }

        // === Ray chuột để trúng mặt đất (cho các tool khác) ===
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mouseRay.origin, mouseRay.direction * interactDistance, Color.green);

        if (!Physics.Raycast(mouseRay, out var hit, interactDistance, combinedMask))
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            soilGrid.SetActiveGrid(false);
            if (!hitPlantForHarvest)
                UIManager.Instance.HideUI("PlantInfo");
            return;
        }

        gridPos = farmManager.WorldToGrid(hit.point);

        if (item != null && item.itemData != null && item.itemData.itemType == ItemType.Seed && item.quantity > 0)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();

            // ẨN grid đất, HIỆN grid trồng cây
            soilGrid.SetActiveGrid(false);
            if (plantGrid != null)
                plantGrid.SetActiveGrid(true);

            // Lấy thông tin về cây sẽ trồng
            var pd = farmManager.plantDatabase ? farmManager.plantDatabase.GetPlantData(item.itemData.plantType) : null;

            if (pd != null)
            {
                int size = pd.GetSizeInt();
                Vector2Int start;
                bool canPlant = false;

                // Tính vị trí start giống logic trong PlantManager
                if (size == 3)
                {
                    if (soilManager.TrySnapStartToHole3x3(gridPos, out start))
                    {
                        canPlant = plantManager.HandleSeedHover(gridPos, item);
                        // Highlight vùng trồng
                        if (plantGrid != null)
                            plantGrid.ShowPlantArea(start, size, canPlant);
                    }
                    else
                    {
                        if (plantGrid != null)
                            plantGrid.Hide();
                    }
                }
                else
                {
                    start = farmManager.CalculateStartPosition(gridPos, size);
                    canPlant = plantManager.HandleSeedHover(gridPos, item);

                    // Highlight vùng trồng
                    if (plantGrid != null)
                        plantGrid.ShowPlantArea(start, size, canPlant);
                }

                // Click để trồng
                if (Input.GetMouseButtonDown(0) && canPlant && plantManager.TryPlant(gridPos, item))
                {
                    farmManager.hotbarUI?.hotbar?.UseAndRemoveItem(farmManager.hotbarUI.valueScroll, 1);
                    farmManager.hotbarUI?.UpdateAllSlots();

                    // Cập nhật PlantGrid sau khi trồng
                    if (plantGrid != null)
                        plantGrid.UpdateGridColors();
                }
            }
            else
            {
                // Không có plant data
                if (plantGrid != null)
                    plantGrid.Hide();
            }
            return;
        }
        else
        {
            // Không cầm seed → ẩn plant grid
            if (plantGrid != null)
            {
                plantGrid.SetActiveGrid(false);
                plantGrid.Hide();
            }
        }

        // Hoe/Shovel
        if (item != null && item.itemData != null && item.itemData.toolType == ToolType.Hoe)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            soilManager.HandleToolHover(gridPos, item);
            soilGrid.SetActiveGrid(true);

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
        if (item != null && item.itemData != null && item.itemData.toolType == ToolType.Watering)
        {
            soilManager.HideGhosts();
            plantManager.HideGhost();
            soilGrid.SetActiveGrid(false);

            if (Physics.Raycast(mouseRay, out var soilHit, interactDistance, gridMask))
            {
                Vector2Int hitPos = farmManager.WorldToGrid(soilHit.point);

                // Kiểm tra xem có trúng vùng đã đào không
                if (soilManager.TryFindAreaContaining(hitPos.x, hitPos.y, out int areaIdx))
                {
                    GameObject areaObj = soilManager.GetAreaObjectByIndex(areaIdx);

                    if (areaObj != null)
                    {
                        // Lấy Outline component
                        var soilOutlineComponent = areaObj.GetComponent<SoilOutline>();
                        if (soilOutlineComponent == null)
                        {
                            // Nếu chưa có, tự động thêm
                            soilOutlineComponent = areaObj.AddComponent<SoilOutline>();
                        }

                        var outline = soilOutlineComponent.GetOutline();
                        SetSoilOutline(outline);
                    }
                }
                else
                {
                    // Không trúng vùng nào -> tắt outline
                    SetSoilOutline(null);
                }
            }
            else
            {
                SetSoilOutline(null);
            }

            // Click để tưới
            if (Input.GetMouseButtonDown(0))
            {
                if (!soilManager.CanStartWaterAt(gridPos))
                {
                    Notification.Instance?.ShowNotification("Không thể tưới nước tại đây!");
                    return;
                }

                if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.wateringState)
                    CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.wateringState);
            }
            return;
        }
        else
        {
            // Không cầm watering -> tắt soil outline
            SetSoilOutline(null);
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

        if (item == null || item.itemData == null || item.itemData.itemType != ItemType.Seed)
        {
            if (plantGrid != null)
            {
                plantGrid.SetActiveGrid(false);
                plantGrid.Hide();
            }
        }

        if (item == null || item.itemData == null || item.itemData.toolType != ToolType.Watering)
        {
            SetSoilOutline(null);
        }

        if (item == null || item.itemData == null || item.itemData.toolType != ToolType.Hoe)
        {
            if (soilGrid != null)
            {
                soilGrid.SetActiveGrid(false);
            }
        }
    }

    private void HandlePlantClick(PlantClickable clickable, Vector3 hitPoint)
    {
        switch (currentHandMode)
        {
            case HandMode.Harvest:
                if (!player || Vector3.Distance(player.position, hitPoint) <= interactDistance)
                {
                    if (!plantManager.CanStartHarvest(gridPos)) return;

                    if (CharacterStateMachine.Instance.currentState != CharacterStateMachine.Instance.harvestLowState && Mp.Instance.mp >= 10)
                    {
                        CharacterStateMachine.Instance.ChangeState(CharacterStateMachine.Instance.harvestLowState);
                    }
                }
                else
                {
                    Notification.Instance?.ShowNotification("Quá xa để thu hoạch!");
                }
                break;

            case HandMode.Remove:
                plantManager.TryRemovePlant(clickable.centerX, clickable.centerY);
                break;
        }
    }

    private bool IsHoldingHarvest(FarmManager farm)
    {
        var item = farm.hotbarUI ? farm.hotbarUI.currentItem : null;
        return item != null && item.itemData != null && item.itemData.toolType == ToolType.Harvest;
    }

    private void SetPlantOutline(Outline next)
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

    private void SetSoilOutline(Outline next)
    {
        if (next == currentSoilOutline) return;

        // Tắt outline cũ
        if (currentSoilOutline)
            currentSoilOutline.OutlineWidth = 0f;

        currentSoilOutline = next;

        // Bật outline mới
        if (currentSoilOutline)
        {
            currentSoilOutline.OutlineMode = Outline.Mode.OutlineVisible;
            currentSoilOutline.OutlineWidth = soilOutlineWidth;
        }
    }

    public void SetHandMode(HandMode mode)
    {
        currentHandMode = mode;
    }
    private bool CheckDistance(Vector3 worldPoint, float dist)
    {
        if (!player) return true; 
        return Vector3.Distance(player.position, worldPoint) <= dist;
    }

}