using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
        SaveManager.Load("slot1", farms);
        Debug.Log("Load game");
    }
    public FarmManager[] farms;

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        var farms = FindObjectsByType<FarmManager>(
          FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        SaveManager.Save("slot1", farms);
    }

    [ContextMenu("Load Game")]
    public void LoadGame() => SaveManager.Load("slot1", farms);
    public void NextDayAllFarm()
    {
        if (farms == null) return;

        foreach (var f in farms)
        {
            if (f == null) continue;                     
            if (!f.isActiveAndEnabled || !f.gameObject.activeInHierarchy) continue; 
            if (f.plantManager == null)
            {
                Debug.LogWarning($"[NextDay] {f.name} missing PlantManager");
                continue;
            }
            f.plantManager.CheckNextDay();
        }
    }

}
