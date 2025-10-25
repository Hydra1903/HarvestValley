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
    public bool[] isBuilding;
    public int[] dayCounter;

    public int currentlevelBarn;
    public int currentlevelHome;
    public bool isUnlockFarmland2;
    public bool isUnlockFarmland3;
    public bool isUnlockGrassland;
    public int currentlevelPen1;
    public int currentlevelPen2;
    public bool isUnlockPen1;
    public bool isUnlockPen2;
    public bool isUnlockGreenhouse1;
    public bool isUnlockGreenhouse2;
    #endregion

    #region ----- Achivements -----
    public bool[] isAchivementComplete;
    public bool[] isReward;

    public int plantedSeedsCount;
    public int harvestedCropsCount;
    public int typesOfCropsPlantedCount;
    public int timesWateredCount;
    public int greenhouseCropsHarvestedCount;
    public int animalProductsCollectedCount;
    public int farmProductsSoldCount;
    public int perennialHarvestsCount;
    public int buildingsUpgradedOrUnlockedCount;
    public int staminaUsedCount;
    public int totalMoneyEarnedCount;
    #endregion

    #region ----- Character -----
    public ECharacter currentCharacter;

    public int currentLevel;
    public int xp;
    public int gold;
    public int mp;
    #endregion

    #region ----- Time -----
    public int day;
    public int month;
    public int year;
    #endregion

    #region ----- Weather -----
    public List<WeatherSchedule> listWeatherOfMonth = new List<WeatherSchedule>();
    #endregion

    #region ----- Season -----
    public SeasonState currentSeason;
    #endregion

    #region ----- Inventory -----
    public ItemData[] itemDataInventory;
    public int[] quantityInventory;
    public bool[] locationInventory;
    #endregion
    #region ----- Barn -----
    public ItemData[] itemDataBarn;
    public int[] quantityBarn;
    public bool[] locationBarn;
    #endregion

}
