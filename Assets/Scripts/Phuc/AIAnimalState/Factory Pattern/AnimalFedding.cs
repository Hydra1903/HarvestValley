using UnityEngine;

public class AnimalFedding : MonoBehaviour
{
    public enum AnimalType { Sheep, Goat }
    public AnimalType animalType;

    public Barn barn;
    public float feedInterval = 5f;

    private bool canHarvest = false;

    private void Start()
    {
        InvokeRepeating(nameof(CheckAndConsume), feedInterval, feedInterval);
    }

    void CheckAndConsume()
    {
        if (barn == null || barn.slots == null) return;

        if (HasHay())
        {
            ConsumeHay();
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
        if (canHarvest) return;
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
                    canHarvest = true;

                    var barnUI = barn.GetComponent<BarnUI>();
                    if (barnUI != null)
                        barnUI.UpdateAllSlots();

                    return;
                }
            }
        }
    }

    public bool CanHarvest() => canHarvest;
    public void ResetHarvest() => canHarvest = false;
}
