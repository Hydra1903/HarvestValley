using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public GameObject gridIndicatorPrefab;
    private List<GameObject> activeIndicators = new();

    public void ShowGrid(Vector2Int center, int size, float duration = 2f)
    {
        ClearGrid();
        int half = size / 2;

        for (int x = -half; x <= half; x++)
            for (int y = -half; y <= half; y++)
            {
                Vector2Int pos = new Vector2Int(center.x + x, center.y + y);
                Vector3 worldPos = new Vector3(pos.x, 0.01f, pos.y);
                GameObject g = Instantiate(gridIndicatorPrefab, worldPos, Quaternion.identity);
                activeIndicators.Add(g);
            }

        StartCoroutine(HideAfter(duration));
    }

    IEnumerator HideAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        ClearGrid();
    }

    public void ClearGrid()
    {
        foreach (var go in activeIndicators)
            Destroy(go);
        activeIndicators.Clear();
    }
}
