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
    private int lastFedDayMorning = -1;
    private int lastFedDayEvening = -1;
    private int lastHandledDay = -1;

    public bool ateMorningToday = false;
    public bool ateEveningToday = false;
    public bool hasEatenToday = false;
    private bool isWaitingToEat = false;
    private bool isLoaded = false;

    private GameTime gameTimeCache;

    private void Awake()
    {
        gameTimeCache = GameTime.Instance;
    }

    private void Start()
    {
        ResetDailyEatFlags();

        if (gameTimeCache == null)
            gameTimeCache = GameTime.Instance;
        if (gameTimeCache == null) return;

        int currentDay = Mathf.FloorToInt(GetAbsoluteGameHours() / 24f);

        if (!isLoaded)
        {
            lastHandledDay = currentDay - 1;
            LastFedDay = currentDay - 1;
        }

        float hourInDay = GetAbsoluteGameHours() % 24f;

        // Ăn khởi tạo khi load
        if (!canHarvest)
        {
            if (animalTypes == FeedingAnimalType.Sheep && hourInDay >= 7f)
                StartCoroutine(SafeStartCoroutine(DelayedEatSheep()));
            else if (animalTypes == FeedingAnimalType.Goat)
            {
                if (hourInDay >= 7f && hourInDay < 12f)
                    StartCoroutine(SafeStartCoroutine(DelayedEat(true)));
                else if (hourInDay >= 17f)
                    StartCoroutine(SafeStartCoroutine(DelayedEat(false)));
            }
        }
    }

    private void Update()
    {
        if (gameTimeCache == null)
            gameTimeCache = GameTime.Instance;
        if (gameTimeCache == null) return;

        float currentHour = GetAbsoluteGameHours();
        int currentDay = Mathf.FloorToInt(currentHour / 24f);
        float hourInDay = currentHour % 24f;

        if (currentDay != lastHandledDay)
        {
            HandleNextDay(currentDay);
            lastHandledDay = currentDay;
            HandleMissedMeals();
        }

        if (isWaitingToEat || canHarvest) return;

        if (animalTypes == FeedingAnimalType.Sheep && !hasEatenToday && hourInDay >= 7f)
            StartCoroutine(SafeStartCoroutine(DelayedEatSheep()));
        else if (animalTypes == FeedingAnimalType.Goat)
        {
            if (!ateMorningToday && hourInDay >= 7f && hourInDay < 12f)
                StartCoroutine(SafeStartCoroutine(DelayedEat(true)));
            else if (!ateEveningToday && hourInDay >= 17f)
                StartCoroutine(SafeStartCoroutine(DelayedEat(false)));
        }
    }

    private IEnumerator SafeStartCoroutine(IEnumerator routine)
    {
        if (isWaitingToEat || canHarvest) yield break;
        isWaitingToEat = true;
        yield return routine;
        isWaitingToEat = false;
    }

    private IEnumerator DelayedEat(bool isMorning = true)
    {
        yield return new WaitForSeconds(1f);
        TryEat(isMorning);
    }

    private IEnumerator DelayedEatSheep()
    {
        yield return new WaitForSeconds(1f);
        TryEat();
    }

    private void TryEat()
    {
        TryEat(true);
    }

    private void TryEat(bool isMorning)
    {
        if (canHarvest) return; // Không ăn nếu đã đạt harvest
        if (!HasHay()) return;

        float currentAbsoluteHour = GetAbsoluteGameHours();
        int currentDay = Mathf.FloorToInt(currentAbsoluteHour / 24f);

        var pen = GetComponentInParent<AnimalPen>();

        if (animalTypes == FeedingAnimalType.Sheep)
        {
            if (hasEatenToday) return;

            ConsumeHay();
            mealsToday++;
            LastFedDay = currentDay;
            hasEatenToday = true;
            pen?.UpdateSavedAnimalData(gameObject);
        }
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

            pen?.UpdateSavedAnimalData(gameObject);
        }

        pen?.UpdateAnimalFeedStatusUI();
    }

    public void HandleNextDay(int currentDay)
    {
        if (!canHarvest && hasEatenToday)
        {
            daysFed++;
            int requiredDays = animalTypes == FeedingAnimalType.Sheep ? requiredFeedDays : requiredGoatDays;
            if (daysFed >= requiredDays)
                canHarvest = true;
        }

        // Reset flags hàng ngày
        hasEatenToday = false;
        ateMorningToday = false;
        ateEveningToday = false;
        mealsToday = 0;

        GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
    }

    public void HandleMissedMeals()
    {
        if (canHarvest) return; // Không ăn nếu đã harvest

        float hourInDay = GetAbsoluteGameHours() % 24f;

        if (animalTypes == FeedingAnimalType.Goat)
        {
            if (!ateMorningToday && hourInDay >= 7f)
                TryEat(true);

            if (!ateEveningToday && hourInDay >= 17f)
                TryEat(false);
        }
        else if (animalTypes == FeedingAnimalType.Sheep)
        {
            if (!hasEatenToday && hourInDay >= 7f)
                TryEat();
        }
    }

    public float GetAbsoluteGameHours()
    {
        var t = gameTimeCache != null ? gameTimeCache : GameTime.Instance;
        if (t == null) return 0;
        return (t.day * 24f) + t.hour;
    }

    private bool HasHay()
    {
        if (hayCellManager == null || hayCellManager.hayCells == null) return false;
        foreach (var cell in hayCellManager.hayCells)
        {
            if (cell == null) continue;
            int qty = cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
            if (qty > 0) return true;
        }
        return false;
    }

    private void ConsumeHay()
    {
        if (hayCellManager == null || hayCellManager.hayCells == null) return;

        var availableCells = hayCellManager.hayCells.FindAll(
            c => c != null && (c.cellIndex == 0 ? c.quanlityCell1 : c.quanlityCell2) > 0);

        if (availableCells.Count == 0) return;

        if (animalTypes == FeedingAnimalType.Sheep)
        {
            foreach (var cell in hayCellManager.hayCells)
            {
                if (cell == null) continue;
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
        else
        {
            availableCells.Sort((a, b) =>
            {
                int qtyA = a.cellIndex == 0 ? a.quanlityCell1 : a.quanlityCell2;
                int qtyB = b.cellIndex == 0 ? b.quanlityCell1 : b.quanlityCell2;
                return qtyB.CompareTo(qtyA);
            });

            foreach (var cell in availableCells)
            {
                if (cell == null) continue;
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
            return hour < 12 ? ateMorningToday : ateEveningToday;
        return hasEatenToday;
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
