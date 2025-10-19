using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterCan : MonoBehaviour
{
    public static WaterCan Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public ItemData waterCan;
    private int maxWater = 100;
    public int currentWater;
    public Slider sliderWaterCan;
    public TextMeshProUGUI textCurrentWaterCan;
    void Start()
    {
        sliderWaterCan.value = (float)currentWater / maxWater;
        textCurrentWaterCan.text = currentWater.ToString();
    }
    public void CheckCurrentItemWaterCan()
    {
        if (HotBarUI.Instance.currentItem != null)
        {
            if (HotBarUI.Instance.currentItem.itemData == waterCan)
            {
                UIManager.Instance.ShowUI("WaterCan");
            }
            else
            {
                UIManager.Instance.HideUI("WaterCan");
            }
        }
    }
    public void FillTheWaterCan()
    {
        currentWater = maxWater;
        sliderWaterCan.value = 1;
        textCurrentWaterCan.text = "100";
    }
    public bool CheckCurrentWater()
    {
        if (20 > currentWater)
        {
            Notification.Instance.ShowNotification("Hết nước!");
            return false;
        }
        else
        {
            return true;
        }
    }
    public void ConsumeWater()
    {
        currentWater -= 20;
        sliderWaterCan.value = (float)currentWater / maxWater;
        textCurrentWaterCan.text = currentWater.ToString();
    }
}
