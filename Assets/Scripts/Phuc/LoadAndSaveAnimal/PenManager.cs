using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PensManager : MonoBehaviour
{
    public static PensManager Instance;

    [Header("References")]
    public List<AnimalPen> allPens = new List<AnimalPen>();
    public List<HayCellManager> allHayManagers = new List<HayCellManager>(); 
    public ItemData hayBaleData;

    private bool isHandlingDay = false;
    private float lastAbsoluteHour = -1f;
    private int lastDay = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SaveLoadSystem.LoadFarm(allPens);

        foreach (var hayManager in allHayManagers)
        {
            if (hayManager != null)
                hayManager.hayCells.ForEach(cell => cell?.UpdateUI());
        }

        foreach (var pen in allPens)
            pen.uiManager?.RefreshUI();

        StartCoroutine(CheckDayChangeRoutine());
    }

    private IEnumerator CheckDayChangeRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (GameTime.Instance == null) continue;

            float absoluteHour = GetAbsoluteGameHours();
            int currentDay = GameTime.Instance.day;

            if (lastAbsoluteHour < 0f)
            {
                lastAbsoluteHour = absoluteHour;
                lastDay = currentDay;
                continue;
            }

            if (currentDay != lastDay)
            {
                SaveFarm();
                lastDay = currentDay;
            }
            else if (absoluteHour - lastAbsoluteHour >= 24f)
            {
                SaveFarm();
                lastAbsoluteHour = absoluteHour;
                lastDay = currentDay + 1;
            }
        }
    }

    public float GetAbsoluteGameHours()
    {
        var t = GameTime.Instance;
        if (t == null) return 0;
        return (t.day * 24f) + t.hour;
    }

    public void SaveFarm()
    {
        if (isHandlingDay) return;
        isHandlingDay = true;

        foreach (var pen in allPens)
        {
            foreach (var (animalObj, _) in pen.GetSpawnedAnimals())
            {
                var feeding = animalObj.GetComponent<AnimalFedding>();
                if (feeding != null)
                {
                    float currentHour = feeding.GetAbsoluteGameHours();
                    int currentDay = Mathf.FloorToInt(currentHour / 24f);
                    if (currentDay != feeding.LastFedDay)
                        feeding.HandleNextDay(currentDay);
                }
            }
        }

        SaveLoadSystem.SaveFarm(allPens);

        foreach (var hayManager in allHayManagers)
        {
            if (hayManager != null)
                hayManager.hayCells.ForEach(cell => cell?.UpdateUI());
        }

        isHandlingDay = false;
    }
}
