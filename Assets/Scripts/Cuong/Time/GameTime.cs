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
    private float timerPlusMp;

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
            timerPlusMp += Time.deltaTime * timeSpeed;
            if (timerPlusMp >= 1000)
            {
                if (Mp.Instance.mp != 100)
                {
                    Mp.Instance.PlusMp(1);
                }
                timerPlusMp = 0;
            }
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
                UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.sleepState);
                Mp.Instance.ResetMp(true);
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
        if (day > 30)
        {
            day = 1; month++;
            Season.Instance.ChangeOfSeasons();
            Weather.Instance.SetListWeatherOfMonth();

        }
        mainUIScreen.UpdateWeatherTimeline();
        FarmStallUI.Instance.CanCollect();
        Builder.Instance.CheckCanBuild();
        SetLocationCharacter();
    }
    public void PauseGame()
    {
        isPaused = true;
    }
    public void UnpauseGame()
    {
        isPaused = false;
    }
    public void SetLocationCharacter()
    {
        CharacterController controller = CharacterStateMachine.Instance.GetComponent<CharacterController>();
        controller.enabled = false;
        CharacterStateMachine.Instance.transform.position = new Vector3(-4.591611f, 0.79f, -4.98656f);
        CharacterStateMachine.Instance.transform.eulerAngles = new Vector3(0f, 37.222f, 0f);
        controller.enabled = true; 
    }
    //dsfd
}