using System.Collections;
using System.Collections.Generic;
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

    private bool canHarvest = false;
    public int daysFed = 0;
    public bool hasEatenToday = false;
    private bool ateMorningToday = false;
    private bool ateEveningToday = false;
    private bool isWaitingToEat = false;
    public int LastFedDay = 0;

    private float prevGameHour = 0f;
    private float lastRecordedHour = 0f;
    private float elapsedGameHours = 0f;

    private List<int> eatHours = new List<int>();
    [HideInInspector] public int mealsToday = 0;

    private void Start()
    {
        ResetDailyEatFlags();

        if (GameTime.Instance != null)
            lastRecordedHour = GetAbsoluteGameHours();

        prevGameHour = GameTime.Instance != null ? GameTime.Instance.hour : 0f;
    }

    private void Update()
    {
        if (GameTime.Instance == null) return;

        float currentHour = GetAbsoluteGameHours();
        float dayHour = GameTime.Instance.hour;
        float delta = currentHour - lastRecordedHour;

        if (delta < 0) delta += 24f * 31f;
        elapsedGameHours += delta;
        lastRecordedHour = currentHour;
        if (prevGameHour > 23f && GameTime.Instance.hour < 1f)
        {
            StartCoroutine(DelayedHandleNextDay());
        }

        if (isWaitingToEat || canHarvest) return;


        if (animalTypes == FeedingAnimalType.Goat)
        {
            if (!ateMorningToday && IsHourReached(dayHour, 7f))
            {
                ateMorningToday = true;
                StartCoroutine(DelayedEat());
            }
            else if (!ateEveningToday && IsHourReached(dayHour, 17f))
            {
                ateEveningToday = true;
                StartCoroutine(DelayedEat());
            }
        }

        else
        {
            if (!hasEatenToday && IsHourReached(dayHour, 7f))
            {
                hasEatenToday = true;
                StartCoroutine(DelayedEat());
            }
        }

        prevGameHour = dayHour;
    }

    private IEnumerator DelayedEat()
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(2f);
        TryEat();
        isWaitingToEat = false;
    }

    private void TryEat()
    {
        if (!HasHay()) return;

        LastFedDay = GameTime.Instance.day;
        ConsumeHay();
        mealsToday++;
        eatHours.Add(Mathf.FloorToInt(GameTime.Instance.hour));

        if (animalTypes == FeedingAnimalType.Sheep)
        {
            hasEatenToday = true;
        }
        else if (animalTypes == FeedingAnimalType.Goat)
        {
            if (mealsToday == 1)
                ateMorningToday = true;
            else if (mealsToday == 2)
                ateEveningToday = true;

            if (mealsToday >= 2)
                hasEatenToday = true;
        }

        var pen = GetComponentInParent<AnimalPen>();
        if (pen != null)
        {
            pen.UpdateAnimalFeedStatusUI();
            pen.UpdateSavedAnimalData(gameObject);

            var infoPanel = pen.penInfoPanel;
            if (infoPanel != null)
                infoPanel.RefreshUI(GetComponent<AnimalInfo>());
        }

        Debug.Log($"{animalTypes} just ate at {GameTime.Instance.hour:0.0}h");
    }

    private bool IsHourReached(float currentHour, float targetHour, float tolerance = 0.2f)
    {
        return Mathf.Abs(currentHour - targetHour) <= tolerance;
    }

    private IEnumerator DelayedHandleNextDay()
    {
        yield return new WaitForSeconds(1f);
        HandleNextDay();
    }

    private void HandleNextDay()
    {

        if (hasEatenToday)
        {
            daysFed++;

            int requiredDays = (animalTypes == FeedingAnimalType.Sheep) ? requiredFeedDays : requiredGoatDays;
            if (daysFed >= requiredDays)
                canHarvest = true;
        }

        var pen = GetComponentInParent<AnimalPen>();
        if (pen != null)
        {
            pen.UpdateSavedAnimalData(gameObject);
            pen.UpdateAnimalFeedStatusUI();

            var infoPanel = pen.penInfoPanel;
            if (infoPanel != null)
                infoPanel.RefreshUI(GetComponent<AnimalInfo>());
            StartCoroutine(DelayedPenUpdateAfterNewDay(pen));
        }

        ResetDailyEatFlags();
    }

    private bool HasHay()
    {
        if (hayCellManager == null || hayCellManager.hayCells == null || hayCellManager.hayCells.Count == 0)
            return false;

        foreach (var cell in hayCellManager.hayCells)
        {
            int qty = cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
            if (qty > 0) return true;
        }
        return false;
    }
    private void ConsumeHay(int amount = 1)
    {
        if (hayCellManager == null) return;
        int remaining = amount;

        foreach (var cell in hayCellManager.hayCells)
        {
            if (cell == null) continue;

            int qty = cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
            if (qty <= 0) continue;

            int deduct = Mathf.Min(remaining, qty);

            if (cell.cellIndex == 0)
                cell.quanlityCell1 -= deduct;
            else
                cell.quanlityCell2 -= deduct;

            remaining -= deduct;
            cell.UpdateUI();

            if (remaining <= 0) break;
        }
    }
    public bool HasEatenAt(int hour)
    {
        return eatHours.Contains(hour);
    }
    private float GetAbsoluteGameHours()
    {
        var t = GameTime.Instance;
        return (t.day * 24f) + t.hour;
    }
    public void SetSavedState(int savedDaysFed, bool savedCanHarvest, bool hasEatenTodayFlag = false)
    {
        daysFed = savedDaysFed;
        canHarvest = savedCanHarvest;
        hasEatenToday = hasEatenTodayFlag;
    }
    private IEnumerator DelayedPenUpdateAfterNewDay(AnimalPen pen)
    {
        yield return new WaitForSeconds(0.1f);

        if (pen == null) yield break;

        pen.UpdateSavedAnimalData(gameObject);
        pen.UpdateAnimalFeedStatusUI();

        var infoPanel = pen.penInfoPanel;
        if (infoPanel != null)
            infoPanel.RefreshUI(GetComponent<AnimalInfo>());


        var penUI = pen.GetComponentInChildren<AnimalPenUIManager>();
        if (penUI != null)
            penUI.UpdateFeedStatus();
    }

    public int GetMealsToday() => mealsToday;
    public bool CanHarvest() => canHarvest;
    public int GetDaysFed() => daysFed;

    public void ResetHarvest()
    {
        canHarvest = false;
        daysFed = 0;
        ResetDailyEatFlags();

        GetComponentInParent<AnimalPen>()?.UpdateSavedAnimalData(gameObject);
    }

    public void ResetDailyEatFlags()
    {
        hasEatenToday = false;
        ateMorningToday = false;
        ateEveningToday = false;
        isWaitingToEat = false;
        mealsToday = 0;
        eatHours.Clear();
    }
}
