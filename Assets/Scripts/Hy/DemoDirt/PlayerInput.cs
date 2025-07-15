using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public FarmGridManager farmGrid;
    public ToolType currentTool;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Chuột trái: cuốc
            TryUseTool(ToolType.Hoe);
        else if (Input.GetMouseButtonDown(1)) // Chuột phải: xẻng
            TryUseTool(ToolType.Shovel);
    }

    void TryUseTool(ToolType tool)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 hitPos = hit.point;
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(hitPos.x), Mathf.RoundToInt(hitPos.z));
            farmGrid.UseTool(gridPos, tool);
        }
    }
}
