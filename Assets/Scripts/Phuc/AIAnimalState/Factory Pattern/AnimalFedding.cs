using UnityEngine;
using System.Collections;

public class AnimalFedding : MonoBehaviour
{
    public enum AnimalType { Sheep, Goat }
    public AnimalType animalType;
    public Barn barn;

    [Header("Feeding Settings")]
    public int requiredFeedDays = 3;
    public int requiredGoatDays = 5;

    private bool canHarvest = false;
    private int daysFed = 0;

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

            string type = animalType == AnimalType.Sheep ? "Sheep" : "Goat";
            Notification.Instance.ShowNotification($"[{type}] Ăn đủ hôm qua → DaysFed = {daysFed}");

            int requiredDays = (animalType == AnimalType.Sheep) ? requiredFeedDays : requiredGoatDays;

            if (daysFed >= requiredDays)
            {
                canHarvest = true;
                Notification.Instance.ShowNotification($"[{type}] Có thể thu hoạch!");
            }
        }
        else
        {
            string type = animalType == AnimalType.Sheep ? "Sheep" : "Goat";
            Notification.Instance.ShowNotification($"[{type}] Hôm qua không ăn.");
        }

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
        if (hasEatenToday) return; // chặn ăn 2 lần

        if (HasHay())
        {
            ConsumeHay();
            hasEatenToday = true;
            missedMealYesterday = false;

            string type = animalType == AnimalType.Sheep ? "Sheep" : "Goat";
            Notification.Instance.ShowNotification($"[{type}] Ăn cỏ hôm nay.");
            GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
        }
        else
        {
            string type = animalType == AnimalType.Sheep ? "Sheep" : "Goat";
            Notification.Instance.ShowNotification($"[{type}] Không có Hay Bale để ăn.");
        }
    }

    private bool HasHay()
    {
        if (barn == null || barn.slots == null) return false;

        for (int r = 0; r < barn.rows; r++)
        {
            for (int c = 0; c < barn.columns; c++)
            {
                var slot = barn.slots[r, c];
                if (slot != null && slot.item != null &&
                    slot.item.itemData != null &&
                    slot.item.itemData.itemName == "Hay Bale" &&
                    slot.item.quantity > 0)
                    return true;
            }
        }
        return false;
    }

    private void ConsumeHay()
    {
        if (barn == null || barn.slots == null) return;

        for (int r = 0; r < barn.rows; r++)
        {
            for (int c = 0; c < barn.columns; c++)
            {
                var slot = barn.slots[r, c];
                if (slot != null && slot.item != null &&
                    slot.item.itemData != null &&
                    slot.item.itemData.itemName == "Hay Bale" &&
                    slot.item.quantity > 0)
                {
                    slot.item.quantity--;
                    if (slot.item.quantity <= 0)
                        slot.item = null;

                    barn.GetComponent<BarnUI>()?.UpdateAllSlots();
                    return;
                }
            }
        }
    }

    private float GetAbsoluteGameHours()
    {
        var t = GameTime.Instance;
        return (t.day * 24f) + t.hour;
    }

    public int GetMealsToday() => hasEatenToday ? 1 : 0;
    public bool CanHarvest() => canHarvest;
    public int GetDaysFed() => daysFed;

    public void ResetHarvest()
    {
        canHarvest = false;
        daysFed = 0;
        missedMealYesterday = false;
        ResetDailyEatFlags();
    }

    public void ResetDailyEatFlags()
    {
        hasEatenToday = false;
        isWaitingToEat = false;
    }
}
