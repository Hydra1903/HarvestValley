using TMPro;
using UnityEngine;
public enum TimeOfDay
{
    Day,
    Night
}
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

    public TimeOfDay currentTimeOfDay;
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
            Weather.Instance.SetCurrentWeather();
        }
        if (hour >= 19)
        {
            currentTimeOfDay = TimeOfDay.Night;
        }
        if (hour >= 24)
        {
            NextDay();
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
        currentTimeOfDay = TimeOfDay.Day;
        hour = 6; minute = 0; day++;
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
