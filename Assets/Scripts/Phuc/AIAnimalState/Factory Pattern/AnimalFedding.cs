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

    private bool sheepAteToday = false;
    private int mealsToday = 0;
    private bool ateAtMorning = false;
    private bool ateAtEvening = false;

    private bool isWaitingToEat = false;
    private bool justHarvested = false; 
    private int lastDayChecked = -1;

    private void Start()
    {
        ResetDailyEatFlags();

        if (GameTime.Instance != null)
            lastDayChecked = GameTime.Instance.day;
    }

    private void Update()
    {
        if (GameTime.Instance != null && GameTime.Instance.day != lastDayChecked)
        {
            HandleNextDay();
            lastDayChecked = GameTime.Instance.day;
        }

        if (justHarvested) return; 

        if (animalType == AnimalType.Goat)
            HandleGoatFeeding();
        else
            HandleSheepFeeding();
    }

    private void HandleSheepFeeding()
    {
        if (!sheepAteToday && !isWaitingToEat)
            StartCoroutine(DelayedEatSheep());
    }

    private void HandleGoatFeeding()
    {
        if (!ateAtMorning && !isWaitingToEat && mealsToday == 0)
            StartCoroutine(DelayedEatGoat(true, "sáng"));
        else if (!ateAtEvening && !isWaitingToEat && mealsToday == 1 && GameTime.Instance.hour >= 12)
            StartCoroutine(DelayedEatGoat(false, "chiều"));
    }

    private void HandleNextDay()
    {
        if (justHarvested)
        {
            justHarvested = false; 
        }

        if (animalType == AnimalType.Sheep)
        {
            if (sheepAteToday)
            {
                daysFed++;
                Notification.Instance.ShowNotification($"[Sheep] Ăn đủ hôm nay → DaysFed = {daysFed}");
                if (daysFed >= requiredFeedDays)
                {
                    canHarvest = true;
                    Notification.Instance.ShowNotification("[Sheep] Có thể thu hoạch!");
                }
            }
            else
                Notification.Instance.ShowNotification("[Sheep] Hôm nay không ăn.");

            sheepAteToday = false;
        }
        else if (animalType == AnimalType.Goat)
        {
            if (mealsToday >= 2)
            {
                daysFed++;
                Notification.Instance.ShowNotification($"[Goat] Ăn đủ 2 bữa → DaysFed = {daysFed}");
                if (daysFed >= requiredGoatDays)
                {
                    canHarvest = true;
                    Notification.Instance.ShowNotification("[Goat] Có thể thu hoạch!");
                }
            }
            else
                Notification.Instance.ShowNotification("[Goat] Ăn chưa đủ.");

            mealsToday = 0;
            ateAtMorning = false;
            ateAtEvening = false;
        }

        isWaitingToEat = false;
    }

    private IEnumerator DelayedEatSheep()
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(1f);
        if (!justHarvested) 
            TryEatSheep();
        isWaitingToEat = false;
    }

    private IEnumerator DelayedEatGoat(bool isMorningMeal, string mealTime)
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(1f);
        if (!justHarvested)
            TryEatGoatMeal(isMorningMeal);
        isWaitingToEat = false;
    }

    private void TryEatSheep()
    {
        if (HasHay())
        {
            ConsumeHay();
            sheepAteToday = true;
            Notification.Instance.ShowNotification("[Sheep] Đã ăn cỏ hôm nay.");
            GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
        }
        else
            Notification.Instance.ShowNotification("[Sheep] Không có Hay Bale để ăn.");
    }

    private void TryEatGoatMeal(bool isMorningMeal)
    {
        if (HasHay())
        {
            ConsumeHay();
            mealsToday++;
            if (isMorningMeal) ateAtMorning = true;
            else ateAtEvening = true;

            Notification.Instance.ShowNotification($"[Goat] Ăn cỏ, mealsToday = {mealsToday}");
            GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
        }
        else
            Notification.Instance.ShowNotification("[Goat] Không có Hay Bale để ăn.");
    }

    bool HasHay()
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
                {
                    return true;
                }
            }
        }
        return false;
    }

    void ConsumeHay()
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
                    if (slot.item.quantity == 0)
                        slot.item = null;

                    var barnUI = barn.GetComponent<BarnUI>();
                    barnUI?.UpdateAllSlots();
                    return;
                }
            }
        }
    }

    public bool HasEatenToday()
    {
        if (animalType == AnimalType.Sheep)
            return sheepAteToday;
        else
            return (ateAtMorning || ateAtEvening);
    }

    public int GetMealsToday()
    {
        int meals = 0;
        if (animalType == AnimalType.Sheep && sheepAteToday) meals = 1;
        else if (animalType == AnimalType.Goat)
        {
            if (ateAtMorning) meals++;
            if (ateAtEvening) meals++;
        }
        return meals;
    }

    public bool CanHarvest() => canHarvest;
    public int GetDaysFed() => daysFed;

    public void ResetHarvest()
    {
        canHarvest = false;
        daysFed = 0;
        justHarvested = true; 
        StopAllCoroutines(); 
        ResetDailyEatFlags();
    }

    public void ResetDailyEatFlags()
    {
        sheepAteToday = false;
        mealsToday = 0;
        ateAtMorning = false;
        ateAtEvening = false;
        isWaitingToEat = false;
    }
}
