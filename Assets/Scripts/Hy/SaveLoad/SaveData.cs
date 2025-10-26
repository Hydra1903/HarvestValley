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
    public int harvestCount; //số lần thu hoạch
}

[Serializable]
public class SprinklerSave
{
    public int gridX, gridY;
    public int halfRange;
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
    public List<SprinklerSave> sprinklers = new();  
    public List<Vector2Int> wateredCenters = new();
}

[Serializable]
public class GameSave
{
    public bool hasFarm;
    public string nameFarm;

    public List<FarmGridSave> grids = new();

    #region ----- Building -----
    public bool[] isBuilding = new bool[12];
    public int[] dayCounter = new int[12];

    public int currentlevelBarn = 1;
    public int currentlevelHome = 1;
    public bool isUnlockFarmland2 = false;
    public bool isUnlockFarmland3 = false;
    public bool isUnlockGrassland = false;
    public int currentlevelPen1 = 0;
    public int currentlevelPen2 = 0;
    public bool isUnlockPen1 = false;
    public bool isUnlockPen2 = false;
    public bool isUnlockGreenhouse1 = false;
    public bool isUnlockGreenhouse2 = false;
    #endregion

    #region ----- Achivements -----
    public bool[] isAchivementComplete = new bool[36];
    public bool[] isReward = new bool[36];

    public int plantedSeedsCount = 0;
    public int harvestedCropsCount = 0;
    public int typesOfCropsPlantedCount = 0;
    public int timesWateredCount = 0;
    public int greenhouseCropsHarvestedCount = 0;
    public int animalProductsCollectedCount = 0;
    public int farmProductsSoldCount = 0;
    public int perennialHarvestsCount = 0;
    public int buildingsUpgradedOrUnlockedCount = 0;
    public int staminaUsedCount = 0;
    public int totalMoneyEarnedCount = 0;
    #endregion

    #region ----- Character -----
    public ECharacter currentCharacter;

    public int currentLevel = 1;
    public int xp = 0;
    public int gold = 0;
    public int mp = 0;
    #endregion

    #region ----- Time -----
    public int day = 1;
    public int month = 0;
    public int year = 0;
    #endregion

    #region ----- Weather -----
    public List<WeatherSchedule> listWeatherOfMonth = new List<WeatherSchedule>();
    #endregion

    #region ----- Season -----
    public SeasonState currentSeason = 0;
    #endregion

    #region ----- Inventory -----
    public ItemData[] itemDataInventory = new ItemData[32];
    public int[] quantityInventory = new int[32];
    public bool[] locationInventory = new bool[32];
    #endregion
    #region ----- Barn -----
    public ItemData[] itemDataBarn = new ItemData[35];
    public int[] quantityBarn = new int[35];
    public bool[] locationBarn = new bool[35];
    #endregion

}
