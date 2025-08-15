using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AreaSave
{
    public int startX;
    public int startY;
    public int size;         
    public SoilType soilType; 
}

[Serializable]
public class PlantSave
{
    public PlantType type;
    public int size;          //kích cỡ
    public int stage;         //trạng thái
    public int daysInStage;   //ngày của trạng thái
    public int centerX;
    public int centerY;
}

[Serializable]
public class FarmGridSave
{
    public string gridId;
    public int width;
    public int height;
    public float cellSize;
    public Vector3 origin;
    public List<AreaSave> areas = new();
    public List<PlantSave> plants = new();
}

[Serializable]
public class GameSave
{
    public List<FarmGridSave> grids = new();
}
