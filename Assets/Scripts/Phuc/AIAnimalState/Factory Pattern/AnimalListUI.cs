using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimalListUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private TMP_Text emptyText;

    private List<AnimalSlotUI> slots = new List<AnimalSlotUI>();
    public void AddAnimal(AnimalInfo animal)
    {
        emptyText.gameObject.SetActive(false);

        var slotObj = Instantiate(slotPrefab, slotParent);
        var slotUI = slotObj.GetComponent<AnimalSlotUI>();
        slotUI.Setup(animal);

        slots.Add(slotUI);
    }
    public void Refresh(List<AnimalInfo> animals)
    {
        // Xoá slot c?
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);
        slots.Clear();

        if (animals == null || animals.Count == 0)
        {
            emptyText.gameObject.SetActive(true);
            return;
        }

        emptyText.gameObject.SetActive(false);

        foreach (var animal in animals)
        {
            var slotObj = Instantiate(slotPrefab, slotParent);
            var slotUI = slotObj.GetComponent<AnimalSlotUI>();
            slotUI.Setup(animal);
            slots.Add(slotUI);
        }
    }
}
