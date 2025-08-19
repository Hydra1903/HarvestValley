using UnityEngine;

public enum PlantType
{
    // Cây 1x1 - trồng trên luống
    Carrot,
    Radish,
    Lettuce,
    
    // Cây 2x2 - trồng trên luống  
    Tomato,
    Corn,
    Cabbage,
    
    // Cây 3x3 - trồng trên hố
    Apple,
    Orange,
    Oak
}

public enum PlantSize
{
    Small = 1,  // 1x1
    Medium = 2, // 2x2
    Large = 3   // 3x3
}