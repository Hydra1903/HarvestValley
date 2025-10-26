using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimalFedding : MonoBehaviour
{
    public enum FeedingAnimalType { Sheep, Goat }
    public FeedingAnimalType animalTypes;
    public HayCellManager hayCellManager;
    public Barn barn;
    public FeedingAnimalType GetAnimalType() => animalTypes;
    [Header("Feeding Settings")]
    public int requiredFeedDays = 3;
    public int requiredGoatDays = 5;

    private bool canHarvest = false;
    public int daysFed = 0;

    private bool hasEatenToday = false;
    private bool isWaitingToEat = false;
    private bool missedMealYesterday = false;

    private float elapsedGameHours = 0f;
    private float lastRecordedHour = 0f;

    private void Start()
    {
        ResetDailyEatFlags();

        if (GameTime.Instance != null)
            lastRecordedHour = GetAbsoluteGameHours();
    }

    private void Update()
    {
        if (GameTime.Instance == null) return;

        float currentHour = GetAbsoluteGameHours();
        float delta = currentHour - lastRecordedHour;
        if (delta < 0) delta += 24f * 31f;

        elapsedGameHours += delta;
        lastRecordedHour = currentHour;

        if (elapsedGameHours >= 24f)
        {
            HandleNextDay();
            elapsedGameHours = 0f;
        }

        if (canHarvest) return;

        if (!hasEatenToday && !isWaitingToEat)
        {
            StartCoroutine(DelayedEat());
        }
    }

    private void HandleNextDay()
    {
        bool fedEnoughYesterday = hasEatenToday;

        if (fedEnoughYesterday)
        {
            daysFed++;

            string type = animalTypes == FeedingAnimalType.Sheep ? "Sheep" : "Goat";
            //Notification.Instance.ShowNotification($"[{type}] Ăn đủ hôm qua, Số ngày ăn = {daysFed}");

            int requiredDays = (animalTypes == FeedingAnimalType.Sheep) ? requiredFeedDays : requiredGoatDays;

            if (daysFed >= requiredDays)
            {
                canHarvest = true;
            }
        }
        else
        {
            string type = animalTypes == FeedingAnimalType.Sheep ? "Sheep" : "Goat";
            //Notification.Instance.ShowNotification($"[{type}] Hôm qua không ăn.");
        }
        GetComponentInParent<AnimalPen>()?.UpdateSavedAnimalData(gameObject);
        GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
        var infoPanel = GetComponentInParent<AnimalPen>()?.penInfoPanel;
             if(infoPanel != null)
             infoPanel.RefreshUI(GetComponent<AnimalInfo>());
        missedMealYesterday = !fedEnoughYesterday;
        ResetDailyEatFlags();
    }

    private IEnumerator DelayedEat()
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(5f);
        TryEat();
        isWaitingToEat = false;
    }

    private void TryEat()
    {
        if (hasEatenToday) return;

        if (HasHay())
        {
            ConsumeHay();
            hasEatenToday = true;
            missedMealYesterday = false;
            string type = animalTypes == FeedingAnimalType.Sheep ? "Sheep" : "Goat";
            //Notification.Instance.ShowNotification($"[{type}] Ăn cỏ hôm nay.");
            GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
            GetComponentInParent<AnimalPen>()?.UpdateSavedAnimalData(gameObject);
            var infoPanel = GetComponentInParent<AnimalPen>()?.penInfoPanel;
            if (infoPanel != null)
                infoPanel.RefreshUI(GetComponent<AnimalInfo>());
        }
        else
        {
            string type = animalTypes == FeedingAnimalType.Sheep ? "Sheep" : "Goat";
            Notification.Instance.ShowNotification($"[{type}] Không còn cỏ khô để ăn.");
        }
    }
    private bool HasHay()
    {
        if (hayCellManager == null)
        {
            Debug.LogWarning("HayCellManager is null!");
            return false;
        }

        if (hayCellManager.hayCells == null || hayCellManager.hayCells.Count == 0)
        {
            Debug.LogWarning("hayCells list is null or empty!");
            return false;
        }

        foreach (var cell in hayCellManager.hayCells)
        {
            if (cell != null && cell.item != null && cell.item.quantity > 0)
                return true;
        }

        return false;
    }

    private void ConsumeHay(int amount = 1)
    {
        if (hayCellManager == null || hayCellManager.hayCells == null || hayCellManager.hayCells.Count == 0)
        {
            Debug.LogWarning("Cannot consume hay: HayCellManager or hayCells list is null/empty.");
            return;
        }

        int remaining = amount;
        foreach (var cell in hayCellManager.hayCells)
        {
            if (cell == null) continue;

            int cellQuantity = cell.item != null ? cell.item.quantity : 0;

            if (cellQuantity > 0)
            {
                int deduct = Mathf.Min(remaining, cellQuantity);

                cell.item.quantity -= deduct;
                remaining -= deduct;

                if (cell.item.quantity <= 0)
                {
                    cell.item = null;
                    cell.isEmpty = true;
                }

                cell.UpdateUI();

                Debug.Log($"Consumed {deduct} hay from {cell.name}, remaining to consume: {remaining}");

                if (remaining <= 0) break; // đã trừ đủ
            }
        }
        if (remaining > 0)
        {
            Debug.LogWarning("Not enough hay to consume! Remaining: " + remaining);
            // Có thể hiển thị Notification ở đây
        }
    }
    private float GetAbsoluteGameHours()
    {
        var t = GameTime.Instance;
        return (t.day * 24f) + t.hour;
    }
    public void SetSavedState(int savedDaysFed, bool savedCanHarvest, bool hasEatenTodayFlag = false)
    {
        daysFed = savedDaysFed;

        // Gán private canHarvest bằng reflection
        typeof(AnimalFedding)
            .GetField("canHarvest", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(this, savedCanHarvest);

        // Reset flag ăn hôm nay nếu muốn
        typeof(AnimalFedding)
            .GetField("hasEatenToday", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(this, hasEatenTodayFlag);
    }

    public int GetMealsToday() => hasEatenToday ? 1 : 0;
    public bool CanHarvest() => canHarvest;
    public int GetDaysFed() => daysFed;
    public void SetSavedState(int savedDaysFed, bool savedCanHarvest)
    {
        daysFed = savedDaysFed;
        canHarvest = savedCanHarvest;
    }
    public void ResetHarvest()
    {
        canHarvest = false;
        daysFed = 0;
        missedMealYesterday = false;
        ResetDailyEatFlags();
        GetComponentInParent<AnimalPen>()?.UpdateSavedAnimalData(gameObject);
    }

    public void ResetDailyEatFlags()
    {
        hasEatenToday = false;
        isWaitingToEat = false;
    }
}
