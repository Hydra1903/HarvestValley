using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    //[SerializeField] private TMP_Text nameText;
    private AnimalPen penRef;
    [SerializeField] private Button sellButton;

    private AnimalInfo animalRef; // con vật được gán vào slot này

    public void Setup(AnimalInfo animal)
    {
        animalRef = animal;
        if (animal != null && animal.animalData != null)
        {
            icon.sprite = animal.animalData.icon;
            //nameText.text = animal.animalData.animalName;
        }

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(OnSellClicked);
    }

    private void OnSellClicked()
    {
        if (animalRef != null && penRef != null)
        {
            penRef.RemoveAnimal(animalRef);
        }
    }
}
