using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum EWaterCanState
{
    On,
    Off
}
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
    public EWaterCanState currentState = EWaterCanState.Off;
    void Start()
    {
        sliderWaterCan.value = (float)currentWater / maxWater;
        textCurrentWaterCan.text = currentWater.ToString();
    }
    public void CheckCurrentState()
    {
        if (currentState == EWaterCanState.On)
        {
            UIManager.Instance.ShowUI("WaterCan");
        }
        else if(currentState == EWaterCanState.Off)
        {
            UIManager.Instance.HideUI("WaterCan");
        }
    }
    public void FillTheWaterCan()
    {
        if (HotBarUI.Instance.currentItem != null)
        {
            if (HotBarUI.Instance.currentItem.itemData.itemName == "Watering Can")
            {
                if (currentWater < maxWater)
                {
                    currentWater = maxWater;
                    sliderWaterCan.value = 1;
                    textCurrentWaterCan.text = "100";
                }
                else
                {
                    Notification.Instance.ShowNotification("Đã đầy nước!");
                }
            }
            else
            {
                Notification.Instance.ShowNotification("Chưa cầm bình nước tưới!");
            }
        }
        else
        {
            Notification.Instance.ShowNotification("Chưa cầm bình nước tưới!");
        }

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
