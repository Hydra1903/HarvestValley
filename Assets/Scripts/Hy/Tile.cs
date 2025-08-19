using UnityEngine;
using static FarmGrid;

public class Tile
{
    public SoilState state = SoilState.Normal;
    public SoilType soilType = SoilType.None; 
    public PlantInstance plantInstance = null;
    public GameObject plantObject = null;
}
