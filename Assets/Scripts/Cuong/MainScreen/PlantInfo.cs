using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantInfo : MonoBehaviour
{
    public TextMeshProUGUI namePlant;
    public TextMeshProUGUI remainingTime;
    public TextMeshProUGUI watered;
    public GameObject[] icon;
    public void SetInfo(PlantType type, string name, string remainingTime, string watered)
    {
        namePlant.text = name;
        this.remainingTime.text = remainingTime;
        this.watered.text = watered;
        for(int i = 0; i < icon.Length; i++)
        {
            icon[i].SetActive(false);
        }
        switch (type)
        {
            case PlantType.Apple:
                icon[0].SetActive(true);
                break;
            case PlantType.Apricot:
                icon[1].SetActive(true);
                break;
            case PlantType.Asparagus:
                icon[2].SetActive(true);
                break;
            case PlantType.Beetroot:
                icon[3].SetActive(true);
                break;
            case PlantType.BellPepper:
                icon[4].SetActive(true);
                break;
            case PlantType.BottleGourd:
                icon[5].SetActive(true);
                break;
            case PlantType.Cabbage:
                icon[6].SetActive(true);
                break;
            case PlantType.Carrot:
                icon[7].SetActive(true);
                break;
            case PlantType.Cauliflower:
                icon[8].SetActive(true);
                break;
            case PlantType.Cherry:
                icon[9].SetActive(true);
                break;
            case PlantType.Chilli:
                icon[10].SetActive(true);
                break;
            case PlantType.Corn:
                icon[11].SetActive(true);
                break;
            case PlantType.Cucumber:
                icon[12].SetActive(true);
                break;
            case PlantType.DelicataSquash:
                icon[13].SetActive(true);
                break;
            case PlantType.Eggplant:
                icon[14].SetActive(true);
                break;
            case PlantType.GreenBean:
                icon[15].SetActive(true);
                break;
            case PlantType.Lemon:
                icon[16].SetActive(true);
                break;
            case PlantType.Onion:
                icon[17].SetActive(true);
                break;
            case PlantType.Orange:
                icon[18].SetActive(true);
                break;
            case PlantType.Peach:
                icon[19].SetActive(true);
                break;
            case PlantType.Pear:
                icon[20].SetActive(true);
                break;
            case PlantType.Plum:
                icon[21].SetActive(true);
                break;
            case PlantType.Potato:
                icon[22].SetActive(true);
                break;
            case PlantType.Pumpkin:
                icon[23].SetActive(true);
                break;
            case PlantType.StripedPumpkin:
                icon[24].SetActive(true);
                break;
            case PlantType.Tomato:
                icon[25].SetActive(true);
                break;
            case PlantType.Watermelon:
                icon[26].SetActive(true);
                break;
            case PlantType.WhitePumpkin:
                icon[27].SetActive(true);
                break;
        }
    }
}
