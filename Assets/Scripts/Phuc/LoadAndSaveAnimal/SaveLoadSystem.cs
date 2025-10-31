using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class AnimalSaveData
{
    public string animalID;
    public AnimalFedding.FeedingAnimalType animalType;
    public string variant;
    public int daysFed;
    public bool canHarvest;
    public bool isActive;
    public int penId;
    public bool hasEatenToday;
    public int lastFedDay;
    public bool ateMorningToday;
    public bool ateEveningToday;
}

[Serializable]
public class FarmSaveData
{
    public List<AnimalSaveData> animals = new List<AnimalSaveData>();
    public List<HayCellData> hayCells = new List<HayCellData>(); // thêm vào đây
}

public static class SaveLoadSystem
{

    public static string savePath => Path.Combine(Application.persistentDataPath, "ChuongNuoi.json");

    public static void SaveFarm(List<AnimalPen> allPens)
    {
        if (!File.Exists(savePath))
        {
            var emptyData = new FarmSaveData();
            string emptyJson = JsonUtility.ToJson(emptyData, true);
            File.WriteAllText(savePath, emptyJson);
        }


        FarmSaveData data = new FarmSaveData();
        int today = DateTime.Now.Day;

        foreach (var pen in allPens)
        {
            if (pen.penHayCellManager != null)
            {
                pen.penHayCellManager.SaveAllCells(data);
            }
        }

        foreach (var pen in allPens)
        {
            foreach (var (obj, info) in pen.GetSpawnedAnimals())
            {
                if (obj == null || info == null) continue;

                var feed = obj.GetComponent<AnimalFedding>();
                if (feed == null) continue;

                var entry = new AnimalSaveData
                {
                    animalID = obj.name,
                    animalType = feed.animalTypes,
                    variant = info.variant,
                    daysFed = feed.GetDaysFed(),
                    canHarvest = feed.CanHarvest(),
                    isActive = obj.activeSelf,
                    penId = pen.penId,
                    hasEatenToday = feed.HasEatenToday(),
                    lastFedDay = feed.GetLastFedDay(),
                    ateMorningToday = feed.ateMorningToday,
                    ateEveningToday = feed.ateEveningToday
                };

                data.animals.Add(entry);
            }
        }
        //Debug.Log("[SaveFarm] Saved to: " + savePath);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }
    public static void LoadFarm(List<AnimalPen> allPens)
    {
        if (!File.Exists(savePath))
        {
            var emptyData = new FarmSaveData();
            string emptyJson = JsonUtility.ToJson(emptyData, true);
            File.WriteAllText(savePath, emptyJson);
        }

        string json = File.ReadAllText(savePath);
        FarmSaveData data = JsonUtility.FromJson<FarmSaveData>(json);

        bool pen1Unlocked = Builder.Instance != null && Builder.Instance.isUnlockPen1;
        bool pen2Unlocked = Builder.Instance != null && Builder.Instance.isUnlockPen2;

        foreach (var pen in allPens)
        {
            foreach (var (obj, _) in pen.GetSpawnedAnimals())
                if (obj != null) GameObject.Destroy(obj);

                if ((pen.penId == 1 && !pen1Unlocked) || (pen.penId == 2 && !pen2Unlocked))
                     continue;

            pen.GetSpawnedAnimals().Clear();
            pen.savedAnimals.Clear();

            if (pen.penHayCellManager != null)
                pen.penHayCellManager.LoadAllCells(data);
        }

        foreach (var animal in data.animals)
        {
            AnimalPen pen = allPens.Find(p => p.penId == animal.penId);
            if (pen == null) continue;

            if ((pen.penId == 1 && !pen1Unlocked) || (pen.penId == 2 && !pen2Unlocked))
                continue;

            // Spawn prefab dựa trên type + variant
            GameObject prefab = GetPrefabFromFeedingType(animal.animalType, animal.variant);
            GameObject obj = GameObject.Instantiate(prefab, pen.GetRandomSpawnPosition(), Quaternion.identity);

            var feed = obj.GetComponent<AnimalFedding>();
            if (feed != null)
            {
                feed.animalTypes = animal.animalType;
                feed.hayCellManager = pen.penHayCellManager;
                feed.barn = pen.barnReference;

                // set trạng thái riêng của từng con
                feed.SetSavedState(
                    animal.daysFed,
                    animal.canHarvest,
                    animal.hasEatenToday,
                    animal.lastFedDay,
                    animal.ateMorningToday,
                    animal.ateEveningToday
                );

                feed.HandleMissedMeals();

                var infoComp = obj.GetComponent<AnimalInfo>();
                obj.SetActive(animal.isActive);

                var ai = obj.GetComponent<SimpleAI>();
                if (ai != null) ai.wanderPoints = pen.wanderPoints;

                pen.RegisterAnimal(obj, infoComp?.data);
                pen.UpdateAnimalFeedStatusUI();
            }
        }
        if (pen1Unlocked || pen2Unlocked)
        {
            File.WriteAllText(savePath, JsonUtility.ToJson(new FarmSaveData(), true));
        }
    }
    public static GameObject GetPrefabFromFeedingType(AnimalFedding.FeedingAnimalType type, string variant)
    {
        if (type == AnimalFedding.FeedingAnimalType.Sheep)
        {
            return variant switch
            {
                "Black" => AnimalFactory.GetPrefab(AnimalType.BlackSheep),
                "Cream" => AnimalFactory.GetPrefab(AnimalType.CreamSheep),
                _ => AnimalFactory.GetPrefab(AnimalType.WhiteSheep)
            };
        }
        else if (type == AnimalFedding.FeedingAnimalType.Goat)
        {
            return variant switch
            {
                "Black" => AnimalFactory.GetPrefab(AnimalType.BlackGoat),
                _ => AnimalFactory.GetPrefab(AnimalType.WhiteGoat)
            };
        }
        return null;
    }
}
