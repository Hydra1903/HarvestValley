using System.Collections.Generic;
using UnityEngine;

public class PensManager : MonoBehaviour
{
    public static PensManager Instance; // thêm singleton

    [Header("References")]
    public List<AnimalPen> allPens = new List<AnimalPen>();
    public List<HayCellManager> allHayManagers = new List<HayCellManager>();
    public ItemData hayBaleData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

        LoadFarm(); // load game khi bắt đầu
    }

    public void LoadFarm()
    {
        SaveLoadSystem.LoadFarm(allPens);

        foreach (var pen in allPens)
            pen.uiManager?.RefreshUI();
    }

    public void SaveFarm()
    {
        SaveLoadSystem.SaveFarm(allPens);
    }
}
