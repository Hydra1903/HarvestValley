using System.Collections;
using UnityEngine;

public class AnimalFedding : MonoBehaviour
{
    public enum FeedingAnimalType { Sheep, Goat }
    public FeedingAnimalType animalTypes;

    [Header("References")]
    public HayCellManager hayCellManager;
    public Barn barn;

    [Header("Feeding Settings")]
    public int requiredFeedDays = 3;
    public int requiredGoatDays = 5;

    [Header("Feed Tracking")]
    public bool canHarvest = false;
    public int daysFed = 0;
    public int mealsToday = 0;
    public int LastFedDay = 0;
    private int lastFedDayMorning = -1;  // ăn sáng dê
    private int lastFedDayEvening = -1; //ăn sáng dê
    private int lastHandledDay = -1;

    public bool ateMorningToday = false;
    public bool ateEveningToday = false;
    public bool hasEatenToday = false;
    private bool isWaitingToEat = false;
    private bool isLoaded = false;

    private void Start()
    {
        ResetDailyEatFlags();

        if (GameTime.Instance == null) return;

        int currentDay = Mathf.FloorToInt(GetAbsoluteGameHours() / 24f);

        if (!isLoaded)
        {
            lastHandledDay = currentDay - 1;
            LastFedDay = currentDay - 1;
        }

        float hourInDay = GetAbsoluteGameHours() % 24f;

        if (animalTypes == FeedingAnimalType.Sheep && hourInDay >= 7f)
            StartCoroutine(DelayedEat());
        else if (animalTypes == FeedingAnimalType.Goat)
        {
            if (hourInDay >= 7f && hourInDay < 12f)
                StartCoroutine(DelayedEat(true));
            else if (hourInDay >= 17f)
                StartCoroutine(DelayedEat(false));
        }
    }

    private void Update()
    {
        if (GameTime.Instance == null) return;

        float currentHour = GetAbsoluteGameHours();
        int currentDay = Mathf.FloorToInt(currentHour / 24f);
        float hourInDay = currentHour % 24f;

        // Xử lý qua ngày mới
        if (currentDay != lastHandledDay)
        {
            HandleNextDay(currentDay);
            lastHandledDay = currentDay;
            HandleMissedMeals();
        }

        // Nếu đang ăn → bỏ qua
        if (isWaitingToEat) return;

        // Sheep ăn buổi sáng
        if (animalTypes == FeedingAnimalType.Sheep && !hasEatenToday && hourInDay >= 7f)
        {
            isWaitingToEat = true;
            StartCoroutine(DelayedEatSheep());
        }
        // Goat ăn sáng / chiều
        else if (animalTypes == FeedingAnimalType.Goat)
        {
            if (!ateMorningToday && hourInDay >= 7f && hourInDay < 12f)
            {
                isWaitingToEat = true;
                StartCoroutine(DelayedEat(true));
            }
            else if (!ateEveningToday && hourInDay >= 17f)
            {
                isWaitingToEat = true;
                StartCoroutine(DelayedEat(false));
            }
        }
    }
    private IEnumerator DelayedEat(bool isMorning = true)
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(1f);
        TryEat(isMorning);
        isWaitingToEat = false;
    }
    private IEnumerator DelayedEatSheep()
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(1f);
        TryEat();
        isWaitingToEat = false;
    }

    private void TryEat()
    {
        TryEat(true);
    }
    private void TryEat(bool isMorning)
    {
        if (!HasHay()) return;
        float currentAbsoluteHour = GetAbsoluteGameHours();
        int currentDay = Mathf.FloorToInt(currentAbsoluteHour / 24f);

        if (animalTypes == FeedingAnimalType.Sheep)
        {
            if (hasEatenToday) return;
            ConsumeHay();             
            mealsToday++;
            LastFedDay = currentDay;
            hasEatenToday = true;
            var pen = GetComponentInParent<AnimalPen>();
            if (pen != null)
            {
                pen.UpdateSavedAnimalData(this.gameObject);
            }
        }
        // --- Goat ---
        else if (animalTypes == FeedingAnimalType.Goat)
        {
            if (isMorning && lastFedDayMorning == currentDay) return;
            if (!isMorning && lastFedDayEvening == currentDay) return;

            ConsumeHay();
            mealsToday++;
            LastFedDay = currentDay;

            if (isMorning)
            {
                ateMorningToday = true;
                lastFedDayMorning = currentDay;
            }
            else
            {
                ateEveningToday = true;
                lastFedDayEvening = currentDay;
            }

            if (ateMorningToday && ateEveningToday)
                hasEatenToday = true;
            var pen = GetComponentInParent<AnimalPen>();
            if (pen != null)
            {
                pen.UpdateSavedAnimalData(this.gameObject);
            }
        }

        GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
    }


    public void HandleMissedMeals()
    {
        if (animalTypes == FeedingAnimalType.Goat)
        {
            float hourInDay = GetAbsoluteGameHours() % 24f;

            // Nếu buổi sáng chưa ăn và giờ đã qua buổi sáng → ăn ngay
            if (!ateMorningToday && hourInDay >= 7f)
                TryEat(true);

            // Nếu buổi tối chưa ăn và giờ đã qua buổi tối → ăn ngay
            if (!ateEveningToday && hourInDay >= 17f)
                TryEat(false);
        }
        else if (animalTypes == FeedingAnimalType.Sheep)
        {
            float hourInDay = GetAbsoluteGameHours() % 24f;
            if (!hasEatenToday && hourInDay >= 7f)
                TryEat();
        }
    }

    public void HandleNextDay(int currentDay)
    {
        if (hasEatenToday)
        {
            daysFed++;
            int requiredDays = animalTypes == FeedingAnimalType.Sheep ? requiredFeedDays : requiredGoatDays;

            if (daysFed >= requiredDays)
                canHarvest = true;
        }

        // Reset flags cho ngày mới
        hasEatenToday = false;
        ateMorningToday = false;
        ateEveningToday = false;
        mealsToday = 0;

        GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
        if (PensManager.Instance != null)
        {
            Debug.Log($"[AnimalFedding] Auto-save triggered for {animalTypes} on new day. Path: {SaveLoadSystem.savePath}");
            PensManager.Instance.SaveFarm();
        }
    }



    public float GetAbsoluteGameHours()
    {
        var t = GameTime.Instance;
        return (t.day * 24f) + t.hour;
    }

    // Kiểm tra có Hay Bale không
    private bool HasHay()
    {
        if (hayCellManager == null || hayCellManager.hayCells == null) return false;

        foreach (var cell in hayCellManager.hayCells)
        {
            int qty = cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
            if (qty > 0) return true;
        }
        return false;
    }

    // Ăn (trừ 1 Hay Bale)
    private void ConsumeHay()
    {
        if (hayCellManager == null || hayCellManager.hayCells == null) return;

        // Lọc các ô còn hay
        var availableCells = hayCellManager.hayCells.FindAll(c => c != null && (c.cellIndex == 0 ? c.quanlityCell1 : c.quanlityCell2) > 0);

        if (availableCells.Count == 0) return;

        if (animalTypes == FeedingAnimalType.Sheep)
        {
            // Sheep ăn: ưu tiên ô đầu tiên trong danh sách hayCells còn hay
            foreach (var cell in hayCellManager.hayCells) // duyệt theo thứ tự trong danh sách
            {
                int qty = cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
                if (qty <= 0) continue;

                // Trừ 1 bale
                if (cell.cellIndex == 0) cell.quanlityCell1--;
                else cell.quanlityCell2--;

                // Cập nhật icon và UI
                if (cell.itemIcon != null)
                    cell.itemIcon.gameObject.SetActive((cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2) > 0);
                cell.UpdateUI();

                // Nếu ô này còn hay, lần sau Sheep sẽ tiếp tục trừ từ ô này
                return;
            }
        }
        else // Goat: vẫn trừ ô nhiều nhất như trước
        {
            availableCells.Sort((a, b) =>
            {
                int qtyA = a.cellIndex == 0 ? a.quanlityCell1 : a.quanlityCell2;
                int qtyB = b.cellIndex == 0 ? b.quanlityCell1 : b.quanlityCell2;
                return qtyB.CompareTo(qtyA);
            });

            foreach (var cell in availableCells)
            {
                int qty = cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
                if (qty <= 0) continue;

                if (cell.cellIndex == 0) cell.quanlityCell1--;
                else cell.quanlityCell2--;

                if (cell.itemIcon != null)
                    cell.itemIcon.gameObject.SetActive((cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2) > 0);
                cell.UpdateUI();

                return;
            }
        }
    }

    public void SetSavedState(
    int savedDaysFed,
    bool savedCanHarvest,
    bool hasEatenTodayFlag,
    int lastFed,
    bool ateMorning,
    bool ateEvening)
    {
        daysFed = savedDaysFed;
        canHarvest = savedCanHarvest;
        hasEatenToday = hasEatenTodayFlag;
        LastFedDay = lastFed;

        ateMorningToday = ateMorning;
        ateEveningToday = ateEvening;

        if (ateMorning) lastFedDayMorning = lastFed;
        if (ateEvening) lastFedDayEvening = lastFed;
        isLoaded = true;

    }


    private void ResetDailyEatFlags()
    {
        hasEatenToday = false;
        ateMorningToday = false;
        ateEveningToday = false;
        isWaitingToEat = false;
        mealsToday = 0;
    }

    public bool HasEatenAtHour(int hour)
    {
        if (animalTypes == FeedingAnimalType.Goat)
        {
            if (hour < 12) return ateMorningToday;
            else return ateEveningToday;
        }
        else
        {
            return hasEatenToday;
        }
    }

    public bool CanHarvest() => canHarvest;
    public int GetDaysFed() => daysFed;
    public int GetMealsToday() => mealsToday;
    public int GetLastFedDay() => LastFedDay;
    public bool HasEatenToday() => hasEatenToday;

    public void ResetHarvest()
    {
        canHarvest = false;
        daysFed = 0;
        ResetDailyEatFlags();
    }
}
