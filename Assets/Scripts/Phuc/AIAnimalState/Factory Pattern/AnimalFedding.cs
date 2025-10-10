using UnityEngine;
using System.Collections;

public class AnimalFedding : MonoBehaviour
{
    public enum AnimalType { Sheep, Goat }
    public AnimalType animalType;
    public Barn barn;

    [Header("Feeding Delay")]
    public float eatDelaySeconds = 30f;
    private bool canHarvest = false;
    public int daysFed = 0;

    // Sheep
    private bool sheepAteToday = false;

    // Goat
    private int mealsToday = 0;
    private bool ateAtMorning = false;
    private bool ateAtEvening = false;
    private int lastKnownDay = -1;
    private bool isWaitingToEat = false;
    private void Start()
    {
        lastKnownDay = GameTime.Instance.day;
    }

    private void Update()
    {
        int hour = GameTime.Instance.hour;
        int currentDay = GameTime.Instance.day;

        if (currentDay != lastKnownDay)
        {
            HandleNextDay();
            lastKnownDay = currentDay;
        }
        if (animalType == AnimalType.Goat)
        {
            if (hour >= 7 && !ateAtMorning && !isWaitingToEat)
            {
                StartCoroutine(DelayedEatGoat(true, "sáng"));
            }
            if (hour >= 19 && !ateAtEvening && !isWaitingToEat)
            {
                StartCoroutine(DelayedEatGoat(false, "chiều"));
            }
        }
        else if (animalType == AnimalType.Sheep)
        {
            if (!sheepAteToday && !isWaitingToEat)
            {
                StartCoroutine(DelayedEatSheep());
            }
        }
    }

    private void HandleNextDay()
    {
        if (animalType == AnimalType.Sheep)
        {
            if (sheepAteToday)
            {
                daysFed++;
                Notification.Instance.ShowNotification($"[Sheep] Ăn đủ hôm nay → DaysFed = {daysFed}");
                if (daysFed >= 3)
                {
                    canHarvest = true;
                    Notification.Instance.ShowNotification("[Sheep] Có thể thu hoạch!");
                }
            }
            else
            {
                Notification.Instance.ShowNotification("[Sheep] Hôm nay không ăn, giữ nguyên trạng thái.");
            }
            sheepAteToday = false;
        }
        else if (animalType == AnimalType.Goat)
        {
            if (mealsToday >= 2)
            {
                daysFed++;
                Notification.Instance.ShowNotification($"[Goat] Ăn đủ 2 bữa hôm nay → DaysFed = {daysFed}");
                if (daysFed >= 5)
                {
                    canHarvest = true;
                    Notification.Instance.ShowNotification("[Goat] Có thể thu hoạch!");
                }
            }
            else
            {
                Notification.Instance.ShowNotification($"[Goat] Ăn chưa đủ (mealsToday = {mealsToday}), giữ nguyên trạng thái.");
            }

            mealsToday = 0;
            ateAtMorning = false;
            ateAtEvening = false;
        }

        isWaitingToEat = false;
    }
    private IEnumerator DelayedEatSheep()
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(eatDelaySeconds);
        TryEatSheep();
        isWaitingToEat = false;
    }

    private IEnumerator DelayedEatGoat(bool isMorningMeal, string mealTime)
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(eatDelaySeconds);
        TryEatGoatMeal(ref isMorningMeal);
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
        {
            Notification.Instance.ShowNotification("[Sheep] Không có Hay Bale để ăn.");
        }
    }
    private void TryEatGoatMeal(ref bool mealFlag)
    {
        if (HasHay())
        {
            ConsumeHay();
            mealsToday++;
            mealFlag = true;
            Notification.Instance.ShowNotification($"[Goat] Ăn cỏ, mealsToday = {mealsToday}");
            GetComponentInParent<AnimalPen>()?.UpdateAnimalFeedStatusUI();
        }
        else
        {
            Notification.Instance.ShowNotification("[Goat] Không có Hay Bale để ăn.");
        }
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
                    if (barnUI != null)
                        barnUI.UpdateAllSlots();
                    return;
                }
            }
        }
    }

    public bool CanHarvest() => canHarvest;

    public void ResetHarvest()
    {
        canHarvest = false;
        daysFed = 0;
        sheepAteToday = false;
        mealsToday = 0;
        ateAtMorning = false;
        ateAtEvening = false;
        isWaitingToEat = false;
    }
    public bool HasEatenToday() => animalType == AnimalType.Sheep ? sheepAteToday : mealsToday > 0;
    public int GetMealsToday() => mealsToday;
    public int GetDaysFed() => daysFed;
    public void ResetDailyEatFlags()
    {
        sheepAteToday = false;
        mealsToday = 0;
        ateAtMorning = false;
        ateAtEvening = false;
        isWaitingToEat = false;
    }
}
