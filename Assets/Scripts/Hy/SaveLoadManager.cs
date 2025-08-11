using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SaveLoadManager: registry-based + fallback find for Unity6 compatibility.
/// Save format: JSON at Application.persistentDataPath/save.json
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    public static string SaveFileName = "save.json";
    public static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    [Header("Options")]
    public bool autoSaveOnQuit = false;

    // ---------- Registry ----------
    private static readonly List<FarmGrid> registeredGrids = new List<FarmGrid>();

    public static void RegisterGrid(FarmGrid g)
    {
        if (g == null) return;
        if (!registeredGrids.Contains(g)) registeredGrids.Add(g);
    }

    public static void UnregisterGrid(FarmGrid g)
    {
        if (g == null) return;
        registeredGrids.Remove(g);
    }

    public static IReadOnlyList<FarmGrid> GetRegisteredGrids() => registeredGrids.AsReadOnly();

    // ---------- Serializable save classes ----------
    [System.Serializable]
    public class PlantSaveData
    {
        public int startX;
        public int startY;
        public int size;
        public PlantType plantType;
        public int currentStage;
        public int daysInCurrentStage;
        public int harvestCount;
    }

    [System.Serializable]
    public class TileSaveData
    {
        public SoilState state;
        public SoilType soilType;
    }

    [System.Serializable]
    public class FarmGridSaveData
    {
        public string gridId;
        public float originX;
        public float originZ;
        public int width;
        public int height;
        public float cellSize;

        public TileSaveData[] tiles;
        public List<PlantSaveData> plants = new List<PlantSaveData>();
    }

    [System.Serializable]
    public class GameSaveData
    {
        public int saveVersion = 1;
        public List<FarmGridSaveData> farmGrids = new List<FarmGridSaveData>();
    }

    // ---------- Save / Load API ----------
    public void SaveAll()
    {
        List<FarmGrid> allGrids = GetGridsForSave();
        SaveAll(allGrids);
    }

    public void SaveAll(List<FarmGrid> allGrids)
    {
        GameSaveData gs = new GameSaveData();

        foreach (var grid in allGrids)
        {
            var fsd = grid.ToSaveData();
            gs.farmGrids.Add(fsd);
        }

        string json = JsonUtility.ToJson(gs, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Game saved to: {SavePath}");
    }

    public void LoadAll()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found to load.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        GameSaveData gs = JsonUtility.FromJson<GameSaveData>(json);
        if (gs == null)
        {
            Debug.LogError("Failed to parse save file.");
            return;
        }

        var allGrids = GetGridsForSave();

        foreach (var fsd in gs.farmGrids)
        {
            FarmGrid found = allGrids.FirstOrDefault(g => !string.IsNullOrEmpty(g.gridId) && g.gridId == fsd.gridId);

            if (found == null)
            {
                // fallback: find by origin proximity
                found = allGrids.FirstOrDefault(g => Vector2.Distance(new Vector2(g.origin.x, g.origin.z), new Vector2(fsd.originX, fsd.originZ)) < 0.01f);
            }

            if (found == null)
            {
                Debug.LogWarning($"No FarmGrid found matching saved gridId:{fsd.gridId} — skipping.");
                continue;
            }

            found.LoadFromSaveData(fsd);
        }

        Debug.Log("Load complete.");
    }

    private void OnApplicationQuit()
    {
        if (autoSaveOnQuit)
            SaveAll();
    }

    // ---------- Helpers ----------
    private List<FarmGrid> GetGridsForSave()
    {
        // Nếu đã có đăng ký thì xài registry (fast)
        if (registeredGrids.Count > 0)
            return new List<FarmGrid>(registeredGrids);

   
        var arr = Object.FindObjectsByType<FarmGrid>(FindObjectsSortMode.None);
        return arr.ToList();
    }
}
