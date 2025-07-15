using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;


public class FarmGridManager : MonoBehaviour
{
    public GameObject furrowSoilPrefab;
    public GameObject treeSoilPrefab;
    public GridVisualizer gridVisualizer;

    private Dictionary<Vector2Int, SoilTile> grid = new();

    public void UseTool(Vector2Int center, ToolType tool)
    {
        switch (tool)
        {
            case ToolType.Hoe:
                CreateSoil(center, SoilType.Furrow, 2);
                gridVisualizer.ShowGrid(center, 2); // hiển thị vùng 2x2
                break;

            case ToolType.Shovel:
                CreateSoil(center, SoilType.TreePlot, 3);
                gridVisualizer.ShowGrid(center, 3); // hiển thị vùng 3x3
                break;
        }
    }

    void CreateSoil(Vector2Int origin, SoilType type, int size)
    {
        int half = size / 2;

        for (int x = -half; x <= half; x++)
            for (int y = -half; y <= half; y++)
            {
                Vector2Int pos = new Vector2Int(origin.x + x, origin.y + y);
                if (!grid.ContainsKey(pos))
                {
                    GameObject prefab = (type == SoilType.Furrow) ? furrowSoilPrefab : treeSoilPrefab;
                    GameObject soilObj = Instantiate(prefab, GridToWorld(pos), Quaternion.identity);
                    SoilTile tile = soilObj.GetComponent<SoilTile>();
                    tile.gridPosition = pos;
                    tile.soilType = type;
                    grid[pos] = tile;
                }
            }
    }

    Vector3 GridToWorld(Vector2Int pos) => new Vector3(pos.x, 0, pos.y);
}
