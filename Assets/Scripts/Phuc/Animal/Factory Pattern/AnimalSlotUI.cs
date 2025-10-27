using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button sellButton;

    private AnimalPen penRef;
    private AnimalInfo animalRef;

    public void Setup(AnimalInfo animal, AnimalPen pen)
    {
        animalRef = animal;
        penRef = pen;

        if (animal != null && animal.data != null)
        {
            icon.sprite = animal.data.icon;
        }

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(OnSellClicked);
    }

    private void OnSellClicked()
    {
        if (animalRef != null && penRef != null)
        {
            penRef.RemoveAnimal(animalRef.gameObject);
        }
    }
}
