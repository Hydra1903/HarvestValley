using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PensManager : MonoBehaviour
{
    [Header("References")]
    public List<AnimalPen> allPens = new List<AnimalPen>();
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
        SaveLoadSystem.SaveFarm(allPens);
    }

    public void LoadFarm()
    {
        SaveLoadSystem.LoadFarm(allPens);

        foreach (var pen in allPens)
            pen.uiManager?.RefreshUI();
    }

    private void OnApplicationQuit()
    {
        SaveLoadSystem.SaveFarm(allPens);
    }
}
