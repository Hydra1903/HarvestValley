using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    static string PathFor(string slot) =>
        System.IO.Path.Combine(Application.persistentDataPath, $"farm_{slot}.json");

    public static void Save(string slot, IEnumerable<FarmGrid> grids)
    {
        var game = new GameSave();

        foreach (var g in grids)
            game.grids.Add(g.BuildSave()); // gọi FarmGrid.BuildSave()

        var json = JsonUtility.ToJson(game, true);
        File.WriteAllText(PathFor(slot), json);
        Debug.Log($"[SaveManager] Saved -> {PathFor(slot)}");
    }

    public static bool Load(string slot, IEnumerable<FarmGrid> grids)
    {
        var path = PathFor(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] No save at {path}");
            return false;
        }

        var json = File.ReadAllText(path);
        var game = JsonUtility.FromJson<GameSave>(json);

        var dict = new Dictionary<string, FarmGridSave>();
        foreach (var s in game.grids) dict[s.gridId] = s;

        foreach (var g in grids)
        {
            if (dict.TryGetValue(g.gridId, out var s))
                g.LoadFromSave(s); // gọi FarmGrid.LoadFromSave()
        }

        Debug.Log($"[SaveManager] Loaded <- {path}");
        return true;

    }
}
