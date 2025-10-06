using UnityEngine;
using UnityEngine.InputSystem;

public class AnimalFedding : MonoBehaviour
{
    public enum AnimalType { Sheep, Goat }
    public AnimalType animalType;

    public Barn barn;

    private bool canHarvest = false;
    public int daysFed = 0;

    // Sheep
    private bool sheepAteToday = false;   // đã ăn trong ngày chưa

    // Goat
    private int mealsToday = 0;           // số bữa đã ăn trong ngày
    private bool ateAtMorning = false;    // 7h
    private bool ateAtEvening = false;    // 19h

    private int lastKnownDay = -1;
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
            if (hour >= 7 && !ateAtMorning)
            {
                TryEatGoatMeal(ref ateAtMorning);
            }
            if (hour >= 19 && !ateAtEvening)
            {
                TryEatGoatMeal(ref ateAtEvening);
            }
        }
        else if (animalType == AnimalType.Sheep)
        {
            if (!sheepAteToday)
            {
                TryEatSheep();
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
                sheepAteToday = false;
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
    }
    private void TryEatSheep()
    {
        if (HasHay())
        {
            ConsumeHay();
            sheepAteToday = true;
            Notification.Instance.ShowNotification("[Sheep] Đã ăn cỏ hôm nay.");
            var pen = GetComponentInParent<AnimalPen>();
            if (pen != null)
            {
                pen.UpdateAnimalFeedStatusUI();
            }
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
            var pen = GetComponentInParent<AnimalPen>();
            if (pen != null)
            {
                pen.UpdateAnimalFeedStatusUI();
            }
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
    }
    public bool HasEatenToday()
    {
        if (animalType == AnimalType.Sheep)
            return sheepAteToday;
        else // Goat
            return mealsToday > 0;
    }

    // Số bữa đã ăn trong ngày (dành cho goat)
    public int GetMealsToday() => mealsToday;

    // Số ngày đã hoàn thành (được tính trong HandleNextDay)
    public int GetDaysFed() => daysFed;

    // Reset flags hàng ngày (nếu bạn muốn gọi thủ công)
    public void ResetDailyEatFlags()
    {
        sheepAteToday = false;
        mealsToday = 0;
        ateAtMorning = false;
        ateAtEvening = false;
    }
}
