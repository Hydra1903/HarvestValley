using UnityEngine;

public class PlantClickable : MonoBehaviour
{
    public int centerX { get; private set; }
    public int centerY { get; private set; }

    public FarmManager ownerFarm { get; private set; }
    public void Init(FarmManager farm, int cx, int cy)
    {
        ownerFarm = farm;
        centerX = cx; centerY = cy;
    }
}
