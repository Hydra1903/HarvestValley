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
}

[Serializable]
public class FarmSaveData
{
    public List<AnimalSaveData> animals = new List<AnimalSaveData>();
}

public static class SaveLoadSystem
{
    public static string savePath => Path.Combine(Application.persistentDataPath, "farmSave.json");
    public static HayCellSaveData haybaler = new HayCellSaveData();

    public static void SaveFarm(List<AnimalPen> allPens)
    {
        FarmSaveData data = new FarmSaveData();

        foreach (var pen in allPens)
        {
            foreach (var info in pen.savedAnimals)
            {
                var objTuple = pen.GetSpawnedAnimals().Find(t => t.Item1.name == info.animalID);
                if (objTuple.Item1 == null) continue;

                var feed = objTuple.Item1.GetComponent<AnimalFedding>();
                if (feed == null) continue;

                var entry = new AnimalSaveData
                {
                    animalID = info.animalID,
                    animalType = feed.animalTypes,
                    variant = info.variant,
                    daysFed = feed.GetDaysFed(),
                    canHarvest = feed.CanHarvest(),
                    isActive = objTuple.Item1.activeSelf,
                    penId = pen.penId
                };
                data.animals.Add(entry);
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"✅ Farm saved to: {savePath}");
    }

    public static void LoadFarm(List<AnimalPen> allPens)
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("⚠ No save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        FarmSaveData data = JsonUtility.FromJson<FarmSaveData>(json);

        foreach (var pen in allPens)
        {
            foreach (var (obj, _) in pen.GetSpawnedAnimals())
                if (obj != null) GameObject.Destroy(obj);

            pen.GetSpawnedAnimals().Clear();
            pen.savedAnimals.Clear();
        }

        foreach (var animal in data.animals)
        {
            AnimalPen pen = allPens.Find(p => p.penId == animal.penId);
            if (pen == null) continue;

            GameObject prefab = GetPrefabFromFeedingType(animal.animalType, animal.variant);
            if (prefab == null)
            {
                Debug.LogWarning($"Missing prefab for {animal.animalType} variant {animal.variant}");
                continue;
            }

            GameObject obj = GameObject.Instantiate(prefab, pen.GetRandomSpawnPosition(), Quaternion.identity);

            var feed = obj.GetComponent<AnimalFedding>();
            if (feed != null)
            {
                feed.animalTypes = animal.animalType;
                feed.hayCellManager = pen.penHayCellManager;
                feed.barn = pen.barnReference;
                feed.SetSavedState(animal.daysFed, animal.canHarvest, false);
            }

            var infoComp = obj.GetComponent<AnimalInfo>();
            if (infoComp != null && pen.penInfoPanel != null)
                infoComp.InjectPanel(pen.penInfoPanel);

            var ai = obj.GetComponent<SimpleAI>();
            if (ai != null) ai.wanderPoints = pen.wanderPoints;

            pen.RegisterAnimal(obj, infoComp?.data);
            pen.UpdateAnimalFeedStatusUI();
        }
    }

    private static GameObject GetPrefabFromFeedingType(AnimalFedding.FeedingAnimalType type, string variant)
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
