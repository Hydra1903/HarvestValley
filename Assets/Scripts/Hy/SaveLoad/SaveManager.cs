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
        var gs = game ?? new GameSave();


        gs.hasFarm = gs.hasFarm || true;

        gs.grids = new List<FarmGridSave>();
        foreach (var f in farms)
            gs.grids.Add(f.BuildSave());

        #region ----- Save Building -----
        gs.isBuilding = Builder.Instance.isBuilding;
        gs.dayCounter = Builder.Instance.dayCounter;
        gs.currentlevelBarn = Builder.Instance.currentlevelBarn;
        gs.currentlevelHome = Builder.Instance.currentlevelHome;
        gs.isUnlockFarmland2 = Builder.Instance.isUnlockFarmland2;
        gs.isUnlockFarmland3 = Builder.Instance.isUnlockFarmland3;
        gs.isUnlockGrassland = Builder.Instance.isUnlockGrassland;
        gs.currentlevelPen1 = Builder.Instance.currentlevelPen1;
        gs.currentlevelPen2 = Builder.Instance.currentlevelPen2;
        gs.isUnlockPen1 = Builder.Instance.isUnlockPen1;
        gs.isUnlockPen2 = Builder.Instance.isUnlockPen2;
        gs.isUnlockGreenhouse1 = Builder.Instance.isUnlockGreenhouse1;
        gs.isUnlockGreenhouse2 = Builder.Instance.isUnlockGreenhouse2;
        #endregion

        #region ----- Save Achivements -----
        gs.isAchivementComplete = Achivements.Instance.isAchivementComplete;
        gs.isReward = AchivementsUI.Instance.isReward;
        gs.plantedSeedsCount = Achivements.Instance.plantedSeedsCount;
        gs.harvestedCropsCount = Achivements.Instance.harvestedCropsCount;
        gs.typesOfCropsPlantedCount = Achivements.Instance.typesOfCropsPlantedCount;
        gs.timesWateredCount = Achivements.Instance.timesWateredCount;
        gs.greenhouseCropsHarvestedCount = Achivements.Instance.greenhouseCropsHarvestedCount;
        gs.animalProductsCollectedCount = Achivements.Instance.animalProductsCollectedCount;
        gs.farmProductsSoldCount = Achivements.Instance.farmProductsSoldCount;
        gs.perennialHarvestsCount = Achivements.Instance.perennialHarvestsCount;
        gs.buildingsUpgradedOrUnlockedCount = Achivements.Instance.buildingsUpgradedOrUnlockedCount;
        gs.staminaUsedCount = Achivements.Instance.staminaUsedCount;
        gs.totalMoneyEarnedCount = Achivements.Instance.totalMoneyEarnedCount;
        #endregion

        #region ----- Save Stats -----
        gs.currentLevel = LevelManager.Instance.currentLevel;
        gs.xp = Xp.Instance.xp;
        gs.gold = Gold.Instance.gold;
        gs.mp = Mp.Instance.mp;
        #endregion

        #region ----- Save Time -----
        gs.day = GameTime.Instance.day;
        gs.month = GameTime.Instance.month;
        gs.year = GameTime.Instance.year;
        #endregion

        #region ----- Save Season -----
        gs.currentSeason = Season.Instance.currentSeason;
        #endregion

        #region ----- Save Inventory -----
        Inventory.Instance.SaveItem();
        gs.itemDataInventory = Inventory.Instance.saveItemData;
        gs.quantityInventory = Inventory.Instance.saveQuantity;
        gs.locationInventory = Inventory.Instance.saveLocation;
        #endregion

        #region ----- Save Barn -----
        Barn.Instance.SaveItem();
        gs.itemDataBarn = Barn.Instance.saveItemData;
        gs.quantityBarn = Barn.Instance.saveQuantity;
        gs.locationBarn = Barn.Instance.saveLocation;
        #endregion

        // Gán lại vào SaveManager.game rồi ghi file
        game = gs;
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
        Debug.Log("Saved to: " + PathFor(slot));
    }


    public static bool Load(string slot, IEnumerable<FarmManager> farms)
    {
        var path = PathFor(slot);
        if (!File.Exists(path)) return false;

        game = JsonUtility.FromJson<GameSave>(File.ReadAllText(path));
        var dict = new Dictionary<string, FarmGridSave>();
        foreach (var s in game.grids) dict[s.gridId] = s;

        // 🔧 Dựng lại từng farm:
        foreach (var f in farms)
        {
            if (f == null || !f.isActiveAndEnabled) continue;
            if (!dict.TryGetValue(f.gridId, out var s)) continue;

            var sys = f.GetComponent<FarmSaveSystem>();
            if (sys == null) sys = f.gameObject.AddComponent<FarmSaveSystem>();

            // đảm bảo các manager đã có & đã Initialize
            if (f.soilManager != null) f.soilManager.Initialize(f);
            if (f.plantManager != null) f.plantManager.Initialize(f, f.soilManager);

            sys.Initialize(f);
            sys.soilManager = f.soilManager;
            sys.plantManager = f.plantManager;

            sys.LoadFromSave(s);
        }

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
