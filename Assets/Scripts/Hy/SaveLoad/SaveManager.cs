using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    static string PathFor(string slot) =>
        System.IO.Path.Combine(Application.persistentDataPath, $"farm_{slot}.json");

    // SaveManager.cs
    public static void Save(string slot, IEnumerable<FarmManager> farms)
    {
        var game = new GameSave();
        foreach (var f in farms) game.grids.Add(f.BuildSave());
        File.WriteAllText(PathFor(slot), JsonUtility.ToJson(game, true));
    }

    public static bool Load(string slot, IEnumerable<FarmManager> farms)
    {
        var path = PathFor(slot);
        if (!File.Exists(path)) return false;

        var game = JsonUtility.FromJson<GameSave>(File.ReadAllText(path));
        var dict = new Dictionary<string, FarmGridSave>();
        foreach (var s in game.grids) dict[s.gridId] = s;

        foreach (var f in farms)
            if (dict.TryGetValue(f.gridId, out var s)) f.LoadFromSave(s);
        return true;
    }
}
