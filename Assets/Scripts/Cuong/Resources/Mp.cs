using UnityEngine;

public class Mp : MonoBehaviour
{
    public static Mp Instance;
    public int mp = 0;
    public int maxMana = 100;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void UseMp(int amount)
    {
        if (mp >= amount)
        {
            mp -= amount;
        }
        else
        {
            Notification.Instance.ShowNotification("Hết năng lượng!");
        }
    }
    public void PlusMp(int amount)
    {
        if (mp == maxMana)
        {
            Notification.Instance.ShowNotification("Đã đầy năng lượng!");
        }
        else
        {
            mp = Mathf.Min(mp + amount, maxMana);
        }
    }
}
