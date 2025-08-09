using UnityEngine;
using UnityEngine.EventSystems;

public class BarnDropHandler : MonoBehaviour, IDropHandler
{
    public Barn barn;
    public BarnUI barnUI;
    public DragItem dragItem;

    public ItemData hayBaleData;

    public void OnDrop(PointerEventData eventData)
    {
        if (dragItem.draggedItem == null)
            return;

        InventoryItem dragged = dragItem.draggedItem;
        if (dragged.itemData != hayBaleData)
        {
            Debug.Log("Only Hay Bale can be placed in the Barn.");
            return;
        }

        if (barnUI.capacity + dragged.quantity > barn.limitCapacity)
        {
            Debug.Log("Pen is Full");
            return;
        }

        bool added = barn.AddItem(dragged.itemData, dragged.quantity);
        if (added)
        {
            dragItem.draggedItem = null;
            barnUI.dragIcon.gameObject.SetActive(false);
            barnUI.UpdateAllSlots();
        }
        else
        {
            Debug.Log("Pen is now full, can't put more!");
        }
    }
}
