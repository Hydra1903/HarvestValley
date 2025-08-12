using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public static GameTime Instance;
    public int day = 1;
    public int month = 1;
    public int year = 1;
    public int hour = 6;
    public int minute = 0;

    public float timeSpeed = 60f; 
    private float timer;

    public MainUIScreen mainUIScreen;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Update()
    {
        timer += Time.deltaTime * timeSpeed;
        if (timer >= 60) 
        {
            minute++; timer = 0;
        }
        if (minute >= 60)
        {
            minute = 0; hour++;
        }
        if (hour >= 24)
        {
            hour = 0; day++;           
        }
        if (day > 30)
        {
            day = 1; month++;
            Season.Instance.ChangeOfSeasons();
        }
        if (month > 4)
        {
            month = 1; year++;
        }
        mainUIScreen.UpdateTime();
    }
    public void NextDay()
    {
        hour = 6; minute = 0; day++;
    }
    public void EndDay()
    {
        NextDay();
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
    }
}
