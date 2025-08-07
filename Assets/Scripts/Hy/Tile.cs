using UnityEngine;
using static FarmGrid;

public class Tile
{
    public SoilState state = SoilState.Normal;
    public PlantInstance plantInstance = null; // Instance cây trồng trên tile này (nếu có)
    public GameObject plantObject = null; // GameObject của cây trong scene
}
