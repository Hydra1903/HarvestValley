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
        Debug.Log($"[PensManager] Loading farm data from path:\n{SaveLoadSystem.savePath}");
        SaveLoadSystem.LoadFarm(allPens);

        foreach (var pen in allPens)
            pen.uiManager?.RefreshUI();

        StartCoroutine(CheckDayChangeRoutine());
    }

    //Check day change
    private IEnumerator CheckDayChangeRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (GameTime.Instance == null) continue;

            float absoluteHour = GetAbsoluteGameHours();
            int currentDay = GameTime.Instance.day;

            //Debug.Log($"[PensManager] Time check → AbsoluteHour: {absoluteHour:F2}, CurrentDay: {currentDay}");

            // Nếu là lần đầu khởi tạo thì gán lại
            if (lastAbsoluteHour < 0f)
            {
                lastAbsoluteHour = absoluteHour;
                lastDay = currentDay;
                continue;
            }

            // Nếu ngày thay đổi (qua ngày mới)
            if (currentDay != lastDay)
            {
                //Debug.Log($"[PensManager] Detected new day → {lastDay} → {currentDay} (AbsoluteHour: {absoluteHour:F2})");
                SaveFarm();
                lastDay = currentDay;
            }
            else if (absoluteHour - lastAbsoluteHour >= 24f)
            {
                // Trường hợp fallback nếu hệ thống chưa tăng day nhưng giờ > 24
                //Debug.Log($"[PensManager] Detected 24 hours passed → triggering SaveFarm()");
                SaveFarm();
                lastAbsoluteHour = absoluteHour;
                lastDay = currentDay + 1;
            }
        }
    }

    public float GetAbsoluteGameHours()
    {
        var t = GameTime.Instance;
        if (t == null)
        {
            //Debug.LogWarning("[PensManager] GameTime.Instance is null, returning 0.");
            return 0;
        }
        return (t.day * 24f) + t.hour;
    }

    public void SaveFarm()
    {
        if (isHandlingDay) return;
        isHandlingDay = true;

        //Debug.Log("[PensManager] SaveFarm() called → preparing to update animals...");

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
        //Debug.Log("[PensManager] All pens updated, now saving farm...");
        SaveLoadSystem.SaveFarm(allPens);
        isHandlingDay = false;
    }
}
