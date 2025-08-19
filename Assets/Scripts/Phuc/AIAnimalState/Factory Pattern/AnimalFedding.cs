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
        InvokeRepeating(nameof(ConsumeHay), feedInterval, feedInterval);
    }

    void ConsumeHay()
    {
        if (canHarvest) return;

        if (barn == null) return;

        for (int r = 0; r < barn.rows; r++)
        {
            for (int c = 0; c < barn.columns; c++)
            {
                var slot = barn.slots[r, c];
                if (slot.item != null && slot.item.itemData.itemName == "Hay Bale" && slot.item.quantity > 0)
                {
                    slot.item.quantity--;
                    if (slot.item.quantity == 0)
                    {
                        slot.item = null;
                    }

                    Debug.Log($"{animalType} has eaten Hay Bale Can harvest now");
                    canHarvest = true;

                    if (barn.GetComponent<BarnUI>() != null)
                        barn.GetComponent<BarnUI>().UpdateAllSlots();

                    return;
                }
            }
        }

        canHarvest = false;
    }

    public bool CanHarvest()
    {
        return canHarvest;
    }

    public void ResetHarvest()
    {
        canHarvest = false;
    }
}
