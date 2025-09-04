using UnityEngine;

public class PlantClickable : MonoBehaviour
{
    public int centerX { get; private set; }
    public int centerY { get; private set; }

    public void Init(int cx, int cy)
    {
        centerX = cx;
        centerY = cy;
    }
}
