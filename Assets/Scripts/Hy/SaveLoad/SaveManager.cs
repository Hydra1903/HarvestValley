using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NUnit.Framework.Interfaces;

public static class SaveManager
{
    public static GameSave game = new GameSave();
    static string PathFor(string slot) =>
        System.IO.Path.Combine(Application.persistentDataPath, $"farm_{slot}.json");

    public static void CreateFarm(string slot, string nameFarm)
    {
        game = new GameSave();
        game.hasFarm = true;
        game.nameFarm = nameFarm;
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
    }
    public static void SaveIsMerchantSpawned(string slot)
    {
        game.isMerchantSpawned = MerchantRandom.Instance.isMerchantSpawned;
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
    }
    public static void SaveCharacter(string slot)
    {
        game.currentCharacter = CharacterSelection.Instance.currentCharacter;
        switch (game.currentCharacter)
        {
            case ECharacter.Rin:
                game.mp = 100;
                break;
            case ECharacter.May:
                game.mp = 85;
                break;
            case ECharacter.Kai:
                game.mp = 110;
                break;
            case ECharacter.Max:
                game.mp = 100;
                break;
            case ECharacter.Hana:
                game.mp = 90;
                break;
            case ECharacter.Leon:
                game.mp = 130;
                break;
        }
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
    }
    public static void SaveListWeather(string slot)
    {
        #region ----- Save Weather -----
        game.listWeatherOfMonth = Weather.Instance.listWeatherOfMonth;
        #endregion
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
    }
    public static void DeleteFarm(string slot)
    {
        game.hasFarm = false;
        game.nameFarm = null;
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
    }
    //SaveManager.cs
    public static void Save(string slot, IEnumerable<FarmManager> farms)
    {
        var game = new GameSave();
        foreach (var f in farms) game.grids.Add(f.BuildSave());
        //foreach (var f in farms) game.grids.Add(f.BuildSave());

        #region ----- Save Building -----
        game.isBuilding = Builder.Instance.isBuilding;
        game.dayCounter = Builder.Instance.dayCounter;
        game.currentlevelBarn = Builder.Instance.currentlevelBarn;
        game.currentlevelHome = Builder.Instance.currentlevelHome;
        game.isUnlockFarmland2 = Builder.Instance.isUnlockFarmland2;
        game.isUnlockFarmland3 = Builder.Instance.isUnlockFarmland3;
        game.isUnlockGrassland = Builder.Instance.isUnlockGrassland;
        game.currentlevelPen1 = Builder.Instance.currentlevelPen1;
        game.currentlevelPen2 = Builder.Instance.currentlevelPen2;
        game.isUnlockPen1 = Builder.Instance.isUnlockPen1;
        game.isUnlockPen2 = Builder.Instance.isUnlockPen2;
        game.isUnlockGreenhouse1 = Builder.Instance.isUnlockGreenhouse1;
        game.isUnlockGreenhouse2 = Builder.Instance.isUnlockGreenhouse2;
        #endregion

        #region ----- Save Achivements -----
        game.isAchivementComplete = Achivements.Instance.isAchivementComplete;
        game.isReward = AchivementsUI.Instance.isReward;
        game.plantedSeedsCount = Achivements.Instance.plantedSeedsCount;
        game.harvestedCropsCount = Achivements.Instance.harvestedCropsCount;
        game.typesOfCropsPlantedCount = Achivements.Instance.typesOfCropsPlantedCount;
        game.timesWateredCount = Achivements.Instance.timesWateredCount;
        game.greenhouseCropsHarvestedCount = Achivements.Instance.greenhouseCropsHarvestedCount;
        game.animalProductsCollectedCount = Achivements.Instance.animalProductsCollectedCount;
        game.farmProductsSoldCount = Achivements.Instance.farmProductsSoldCount;
        game.perennialHarvestsCount = Achivements.Instance.perennialHarvestsCount;
        game.buildingsUpgradedOrUnlockedCount = Achivements.Instance.buildingsUpgradedOrUnlockedCount;
        game.staminaUsedCount = Achivements.Instance.staminaUsedCount;
        game.totalMoneyEarnedCount = Achivements.Instance.totalMoneyEarnedCount;
        #endregion

        #region ----- Save Stats -----
        game.currentLevel = LevelManager.Instance.currentLevel;
        game.xp = Xp.Instance.xp;   
        game.gold = Gold.Instance.gold;
        game.mp = Mp.Instance.mp;
        #endregion

        #region ----- Save Time -----
        game.day = GameTime.Instance.day;
        game.month = GameTime.Instance.month;
        game.year = GameTime.Instance.year;
        #endregion

        #region ----- Save Season -----
        game.currentSeason = Season.Instance.currentSeason;
        #endregion

        #region ----- Save Inventory -----
        Inventory.Instance.SaveItem();
        game.itemDataInventory = Inventory.Instance.saveItemData;
        game.quantityInventory = Inventory.Instance.saveQuantity;
        game.locationInventory = Inventory.Instance.saveLocation;
        #endregion

        #region ----- Save Barn -----
        Barn.Instance.SaveItem();
        game.itemDataBarn = Barn.Instance.saveItemData;
        game.quantityBarn = Barn.Instance.saveQuantity;
        game.locationBarn = Barn.Instance.saveLocation;
        #endregion
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
        Debug.Log(" Đường dẫn file JSON: " + game);
        Debug.Log(" Đường dẫn file JSON: " + PathFor(slot));
    }

    public static bool Load(string slot, IEnumerable<FarmManager> farms)
    {
        var path = PathFor(slot);
        if (!File.Exists(path)) return false;

        game = JsonUtility.FromJson<GameSave>(File.ReadAllText(path));
        var dict = new Dictionary<string, FarmGridSave>();
        foreach (var s in game.grids) dict[s.gridId] = s;

        foreach (var f in farms)
            if (dict.TryGetValue(f.gridId, out var s)) f.LoadFromSave(s);

        #region ----- Load Building -----
        Builder.Instance.isBuilding = game.isBuilding;
        Builder.Instance.dayCounter = game.dayCounter;
        Builder.Instance.currentlevelBarn = game.currentlevelBarn;
        Builder.Instance.currentlevelHome = game.currentlevelHome;
        Builder.Instance.isUnlockFarmland2 = game.isUnlockFarmland2;
        Builder.Instance.isUnlockFarmland3 = game.isUnlockFarmland3;
        Builder.Instance.isUnlockGrassland = game.isUnlockGrassland;
        Builder.Instance.currentlevelPen1 = game.currentlevelPen1;
        Builder.Instance.currentlevelPen2 = game.currentlevelPen2;
        Builder.Instance.isUnlockPen1 = game.isUnlockPen1;
        Builder.Instance.isUnlockPen2 = game.isUnlockPen2;
        Builder.Instance.isUnlockGreenhouse1 = game.isUnlockGreenhouse1;
        Builder.Instance.isUnlockGreenhouse2 = game.isUnlockGreenhouse2;
        #endregion

        #region ----- Load Achivements -----
        Achivements.Instance.isAchivementComplete = game.isAchivementComplete;
        AchivementsUI.Instance.isReward = game.isReward;
        Achivements.Instance.plantedSeedsCount = game.plantedSeedsCount;
        Achivements.Instance.harvestedCropsCount = game.harvestedCropsCount;
        Achivements.Instance.typesOfCropsPlantedCount = game.typesOfCropsPlantedCount;
        Achivements.Instance.timesWateredCount = game.timesWateredCount;
        Achivements.Instance.greenhouseCropsHarvestedCount = game.greenhouseCropsHarvestedCount;
        Achivements.Instance.animalProductsCollectedCount = game.animalProductsCollectedCount;
        Achivements.Instance.farmProductsSoldCount = game.farmProductsSoldCount;
        Achivements.Instance.perennialHarvestsCount = game.perennialHarvestsCount;
        Achivements.Instance.buildingsUpgradedOrUnlockedCount = game.buildingsUpgradedOrUnlockedCount;
        Achivements.Instance.staminaUsedCount = game.staminaUsedCount;
        Achivements.Instance.totalMoneyEarnedCount = game.totalMoneyEarnedCount;
        #endregion

        #region ----- Load Character -----
        CharacterStateMachine.Instance.currentCharacter = game.currentCharacter;

        LevelManager.Instance.currentLevel = game.currentLevel;
        Xp.Instance.xp = game.xp;
        Gold.Instance.gold = game.gold;
        Mp.Instance.mp = game.mp;
        #endregion

        #region ----- Load Time -----
        GameTime.Instance.day = game.day;
        GameTime.Instance.month = game.month;
        GameTime.Instance.year = game.year;
        #endregion

        #region ----- Load Weather -----
        Weather.Instance.listWeatherOfMonth = game.listWeatherOfMonth;
        #endregion

        #region ----- Load Season -----
        Season.Instance.currentSeason = game.currentSeason;
        #endregion

        #region ----- Load Inventory -----
        Inventory.Instance.saveItemData = game.itemDataInventory;
        Inventory.Instance.saveQuantity = game.quantityInventory;
        Inventory.Instance.saveLocation = game.locationInventory;
        Inventory.Instance.LoadItem();
        #endregion

        #region ----- Load Barn -----
        Barn.Instance.saveItemData = game.itemDataBarn;
        Barn.Instance.saveQuantity = game.quantityBarn;
        Barn.Instance.saveLocation = game.locationBarn;
        Barn.Instance.LoadItem();
        #endregion

        #region ----- Load Character -----
        CharacterStateMachine.Instance.currentCharacter = game.currentCharacter;
        #endregion

        #region ----- Load NameFarm -----
        MainUIScreen.Instance.textNameFarm.text = game.nameFarm;
        #endregion

        #region ----- Load Merchant -----
        MerchantRandom.Instance.isMerchantSpawned = game.isMerchantSpawned;
        #endregion
        return true;     
    }
    public static bool IsHasFarm()
    {
        var path = PathFor("slot1");
        if (!File.Exists(path)) return false;
        game = JsonUtility.FromJson<GameSave>(File.ReadAllText(path));
        return game.hasFarm;
    }

}
