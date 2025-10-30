using UnityEngine;

[ExecuteAlways]   // chạy cả khi chưa Play
public class FarmGridDebugger : MonoBehaviour
{
    public FarmManager farm;

    public Color gridColor = Color.white;
    public Color holeColor = Color.red;
    public Color plotColor = Color.yellow;

    private void OnDrawGizmos()
    {
        if (farm == null) farm = GetComponent<FarmManager>();
        if (farm == null) return;

        for (int x = 0; x < farm.gridWidth; x++)
        {
            for (int y = 0; y < farm.gridHeight; y++)
            {
                // trung tâm tile
                Vector3 pos = farm.origin + new Vector3(
                    (x + 0.5f) * farm.cellSize,
                    0f,
                    (y + 0.5f) * farm.cellSize
                );

                // vẽ lưới mặc định
                Gizmos.color = gridColor;
                Gizmos.DrawWireCube(pos, new Vector3(farm.cellSize, 0.05f, farm.cellSize));

                // nếu có SoilType thì đổi màu
                if (farm.Tiles != null)
                {
                    var t = farm.Tiles[x, y];
                    if (t != null)
                    {
                        if (t.soilType == SoilType.Hole) Gizmos.color = holeColor;
                        else if (t.soilType == SoilType.Furrow) Gizmos.color = plotColor;
                        else continue;

                        Gizmos.DrawCube(pos, new Vector3(farm.cellSize * 0.9f, 0.05f, farm.cellSize * 0.9f));
                    }
                }
            }
        }
    }
}
