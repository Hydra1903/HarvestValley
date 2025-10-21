using UnityEngine;
using System.Collections;

public class AnimalFedding : MonoBehaviour
{
    public enum AnimalType { Sheep, Goat }
    public AnimalType animalType;
    public Barn barn;

    [Header("Feeding Settings")]
    public float eatDelaySeconds = 5f;   
    public int requiredFeedDays = 3;   
    public int requiredGoatDays = 5;  

    private bool canHarvest = false;
    private int daysFed = 0;

    private bool sheepAteToday = false;

    private int mealsToday = 0;
    private bool hasEaten = false;
    private bool ateAtMorning = false;
    private bool ateAtEvening = false;

    private bool isWaitingToEat = false;
    private float eatTimer = 0f;

   
    private void Start()
    {
        ResetDailyEatFlags();
    }

    private void Update()
    {
        eatTimer += Time.deltaTime;
        if (eatTimer >= eatDelaySeconds)
        {
            HandleNextDay();
            eatTimer = 0f;
        }

        if (animalType == AnimalType.Goat)
        {
            HandleGoatFeeding();
        }
        else if (animalType == AnimalType.Sheep)
        {
            HandleSheepFeeding();
        }
    }

    private void HandleSheepFeeding()
    {
        if (!sheepAteToday && !isWaitingToEat)
        {
            StartCoroutine(DelayedEatSheep());
        }
    }

    private void HandleGoatFeeding()
    {
        if (!ateAtMorning && !isWaitingToEat && mealsToday == 0)
        {
            StartCoroutine(DelayedEatGoat(true, "sáng"));
        }
        if (!ateAtEvening && !isWaitingToEat && mealsToday == 1 && eatTimer > eatDelaySeconds / 2f)
        {
            StartCoroutine(DelayedEatGoat(false, "chiều"));
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
                if (daysFed >= requiredFeedDays)
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
                if (daysFed >= requiredGoatDays)
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
        yield return new WaitForSeconds(eatDelaySeconds / 2f);
        TryEatSheep();
        isWaitingToEat = false;
    }

    private IEnumerator DelayedEatGoat(bool isMorningMeal, string mealTime)
    {
        isWaitingToEat = true;
        yield return new WaitForSeconds(eatDelaySeconds / 4f);
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
        {
            Notification.Instance.ShowNotification("[Sheep] Không có Hay Bale để ăn.");
        }
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
    public int GetDaysFed() => daysFed;

    public void ResetHarvest()
    {
        canHarvest = false;
        daysFed = 0;
        ResetDailyEatFlags();
    }

    public void ResetDailyEatFlags()
    {
        sheepAteToday = false;
        mealsToday = 0;
        ateAtMorning = false;
        ateAtEvening = false;
        isWaitingToEat = false;
        eatTimer = 0f;
    }
    public bool HasEatenToday()
    {
        return hasEaten;
    }

    public int GetMealsToday()
    {
        return mealsToday;
    }
}
