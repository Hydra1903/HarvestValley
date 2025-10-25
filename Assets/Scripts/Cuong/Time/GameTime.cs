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
    public int month = 0;
    public int year = 0;
    public int hour = 6;
    public int minute = 0;

    public float timeSpeed = 60f;
    private float timer;

    public bool isPaused;

    //public bool canHarvestToday = false;
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
        if (!isPaused)
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
            if (hour >= 18)
            {
                currentTimeOfDay = TimeOfDay.Night;
                mainUIScreen.UpdateIconTimeOfDay();
                MusicBackground.Instance.ChangeBackgroundMusic();
            }
            if (hour >= 24)
            {
                NextDay();
            }
            if (day > 30)
            {
                day = 1; month++;
                Season.Instance.ChangeOfSeasons();
                Weather.Instance.SetListWeatherOfMonth();
                mainUIScreen.UpdateWeatherTimeline();
            }
            if (month >= 4)
            {
                month = 0; year++;
            }
            mainUIScreen.UpdateTime();
        }
    }
    public void NextDay()
    {
        hour = 6; minute = 0; day++;
        currentTimeOfDay = TimeOfDay.Day;
        mainUIScreen.UpdateIconTimeOfDay();
        if (day <= 30)
        {
            mainUIScreen.UpdateWeatherTimeline();
        }
        FarmStallUI.Instance.CanCollect();
        Builder.Instance.CheckCanBuild();
    }
    public void PauseGame()
    {
        isPaused = true;
    }
    public void UnpauseGame()
    {
        isPaused = false;
    }
}