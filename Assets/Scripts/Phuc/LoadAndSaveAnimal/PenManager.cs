using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PensManager : MonoBehaviour
{
    [Header("References")]
    public List<AnimalPen> allPens = new List<AnimalPen>();
    public List<HayCellManager> allHayManagers= new List<HayCellManager>();
    public ItemData hayBaleData;
    public Button saveButton;
    public Button loadButton;

    private void Start()
    {
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveFarm);

        if (loadButton != null)
            loadButton.onClick.AddListener(LoadFarm);
        LoadFarm();
    }

    public void SaveFarm()
    {
        SaveLoadSystem.SaveFarm(allPens/*, allHayManagers*/);
    }

    public void LoadFarm()
    {
        SaveLoadSystem.LoadFarm(allPens/*, allHayManagers, hayBaleData*/);

        foreach (var pen in allPens)
            pen.uiManager?.RefreshUI();
    }

    //private void OnApplicationQuit()
    //{
    //    SaveLoadSystem.SaveFarm(allPens, allHayManagers);
    //}
}
