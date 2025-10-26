using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;
using NUnit.Framework.Interfaces;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;

public class MainUIScreen : MonoBehaviour
{
    public static MainUIScreen Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    [Header("--- FPS UI ---")]
    public TextMeshProUGUI textFPS;
    private float deltaTime = 0.0f;
    private float fps = 0.0f;

    [Header("--- Level UI ---")]
    public GameObject[] panelLevelUp;
    public GameObject backgroundLevelUp;
    public TextMeshProUGUI textNumberLevelUp;
    public Slider xpBar;
    public TextMeshProUGUI textCurrentXp;
    public TextMeshProUGUI textCurrentLevel;
    public LevelManager levelManager;
    public Animator animator;

    [Header("--- Mana UI ---")]
    public Slider mpBar;
    public TextMeshProUGUI textCurrentMp;

    [Header("--- Gold UI ---")]
    public TextMeshProUGUI textGold;

    [Header("--- Time UI ---")]
    public TextMeshProUGUI textTime;
    public TextMeshProUGUI textDay;
    public Image iconTimeOfDay;
    public Sprite iconDay;
    public Sprite iconNight;

    [Header("--- Season UI ---")]
    public GameObject spring;
    public GameObject summer;
    public GameObject fall;
    public GameObject winter;
    public GameObject currentPanelSeason;

    [Header("--- Calendar UI ---")]
    public Image[] background;
    public Image[] iconWeather;
    public Sprite clearIcon;
    public Sprite rainyIcon;
    public Sprite stormyIcon;
    public Sprite snowyIcon;

    [Header("--- Weather UI ---")]
    public GameObject clear;
    public GameObject rainy;
    public GameObject stormy;
    public GameObject snowy;
    public GameObject currentPanelWeather;

    public Image iconWeatherTimeline;
    public TextMeshProUGUI textTimeline1;
    public TextMeshProUGUI textTimeline2;

    [Header("--- Stamina UI ---")]
    public Slider staminaBar;

    [Header("--- Action UI ---")]
    public Slider actionBar;
    public Image frame;

    [Header("--- NameFarm UI ---")]
    public TextMeshProUGUI textNameFarm;

    [Header("--- AvatarCharacter UI ---")]
    public Sprite[] avatarCharacter;
    public Image avatarUI;
    public Image iconMap;

    void Start()
    {
        UpdateXpUI();
        UpdateMpUI();
        UpdateGold();
        SetAvatarCharacter();
        UpdateSeason();
        UpdateCalendar();
        UpdateWeather();
        InvokeRepeating(nameof(UpdateFPSDisplay), 0, 0.5f);
        UpdateWeatherTimeline();
    }
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        UpdateStamina();

        if (Input.GetKey(KeyCode.E))
        {
            frame.fillAmount += Time.deltaTime;
            if (frame.fillAmount >= 1)
            {
                CharacterStateMachine.Instance.ExitState();
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            frame.fillAmount = 0;
        }
    }
    #region ----- FPS UI -----
    void UpdateFPSDisplay()
    {
        textFPS.text = $"{fps:0.} FPS";
    }
    #endregion

    #region ----- LEVEL XP UI -----
    public void UpdateXpUI()
    {
        if (levelManager.currentLevel < levelManager.levelMax)
        {
            xpBar.value = (float)Xp.Instance.xp / levelManager.xpThresholds[levelManager.currentLevel - 1];
            textCurrentXp.text = Xp.Instance.xp.ToString("N0", new CultureInfo("de-DE")) + "/" + levelManager.xpThresholds[levelManager.currentLevel - 1].ToString("N0", new CultureInfo("de-DE")) + " XP";
        }
        else
        {
            xpBar.value = 1;
            textCurrentXp.text = "Cấp độ tối đa";
        }
        textCurrentLevel.text = levelManager.currentLevel.ToString();
    }
    public void ShowPanelLevelUp()
    {
        textNumberLevelUp.text = levelManager.currentLevel.ToString();
        backgroundLevelUp.SetActive(true);
        panelLevelUp[levelManager.currentLevel - 2].SetActive(true);
        StartCoroutine(Hide(panelLevelUp[levelManager.currentLevel - 2]));

        UISounds.Instance.PlaySound_LevelUp();
    }

    IEnumerator Hide(GameObject currentPanelLevelUp)
    {
        yield return new WaitForSeconds(4f);
        Invoke("HideUI",1f);
        animator.Play("Off");
        currentPanelLevelUp.SetActive(false);
    }
    public void HideUI()

    {
        backgroundLevelUp.SetActive(false);
    }
    #endregion

    #region ----- MANA MP UI -----
    public void UpdateMpUI()
    {
        mpBar.value = (float)Mp.Instance.mp / Mp.Instance.maxMana;
        textCurrentMp.text = Mp.Instance.mp.ToString() + "/" + Mp.Instance.maxMana.ToString();
    }
    #endregion

    #region ----- GOLD UI -----
    public void UpdateGold()
    {
        textGold.text = Gold.Instance.gold.ToString("N0", new CultureInfo("de-DE"));
    }
    #endregion

    #region ----- TIME UI -----
    public async void UpdateTime()
    {
        textTime.text = $"{GameTime.Instance.hour}:{GameTime.Instance.minute:00}";
        string day = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UI_MainScreen", "TEXT_Day").Task;
        textDay.text = day +" "+ GameTime.Instance.day.ToString();
    }
    public void UpdateIconTimeOfDay()
    {
        if (GameTime.Instance.currentTimeOfDay == TimeOfDay.Day)
        {
            iconTimeOfDay.sprite = iconDay;
        }
        else
        {
            iconTimeOfDay.sprite = iconNight;
        }
    }
    #endregion

    #region ----- SEASON UI -----
    public void UpdateSeason()
    {
        if (Season.Instance.currentSeason == SeasonState.Spring)
        {
            currentPanelSeason.SetActive(false);
            spring.SetActive(true);
            currentPanelSeason = spring;
        }
        else if (Season.Instance.currentSeason == SeasonState.Summer)
        {
            currentPanelSeason.SetActive(false);
            summer.SetActive(true);
            currentPanelSeason = summer;
        }
        else if (Season.Instance.currentSeason == SeasonState.Fall)
        {
            currentPanelSeason.SetActive(false);
            fall.SetActive(true);
            currentPanelSeason = fall;
        }
        else if (Season.Instance.currentSeason == SeasonState.Winter)
        {
            currentPanelSeason.SetActive(false);
            winter.SetActive(true);
            currentPanelSeason = winter;
        }
    }
    #endregion

    #region ----- CALENDAR UI -----
    public void UpdateCalendar()
    {
        for (int i = 0; i < 30; i++)
        {
            Color color;
            switch (Weather.Instance.listWeatherOfMonth[i].weather)
            {
                case WeatherState.Clear:
                    iconWeather[i].sprite = clearIcon;
                    if (ColorUtility.TryParseHtmlString("#579C48", out color)) background[i].color = color;
                    break;
                case WeatherState.Rainy:
                    iconWeather[i].sprite = rainyIcon;
                    if (ColorUtility.TryParseHtmlString("#7BB0BC", out color)) background[i].color = color;
                    break;
                case WeatherState.Stormy:
                    iconWeather[i].sprite = stormyIcon;
                    if (ColorUtility.TryParseHtmlString("#909090", out color)) background[i].color = color;
                    break;
                case WeatherState.Snowy:
                    iconWeather[i].sprite = snowyIcon;
                    if (ColorUtility.TryParseHtmlString("#7BB0BC", out color)) background[i].color = color;
                    break;
            }
        }
    }
    #endregion

    #region ----- WEATHER UI -----
    public void UpdateWeather()
    {
        if (Weather.Instance.currentWeather == WeatherState.Clear)
        {
            currentPanelWeather.SetActive(false);
            clear.SetActive(true);
            currentPanelWeather = clear;
        }
        else if (Weather.Instance.currentWeather == WeatherState.Rainy)
        {
            currentPanelWeather.SetActive(false);
            rainy.SetActive(true);
            currentPanelWeather = rainy;
        }
        else if (Weather.Instance.currentWeather == WeatherState.Stormy)
        {
            currentPanelWeather.SetActive(false);
            stormy.SetActive(true);
            currentPanelWeather = stormy;
        }
        else if (Weather.Instance.currentWeather == WeatherState.Snowy)
        {
            currentPanelWeather.SetActive(false);
            snowy.SetActive(true);
            currentPanelWeather = snowy;
        }
    }

    public async void UpdateWeatherTimeline()
    {
        WeatherSchedule weatherScheduleOfDay = Weather.Instance.listWeatherOfMonth[GameTime.Instance.day - 1];

        if (weatherScheduleOfDay.weather == WeatherState.Clear)
        {
            textTimeline1.text = "6h - 24h";
            string none = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UI_MainScreen", "TEXT_None").Task;
            textTimeline2.text = none;
        }
        else if (weatherScheduleOfDay.weather == WeatherState.Rainy)
        {
            int start = weatherScheduleOfDay.randomWeatherStartTime;
            int end = weatherScheduleOfDay.randomWeatherEndTime;
            iconWeatherTimeline.sprite = rainyIcon;
            SetTextTimeline(start, end);
        }
        else if (weatherScheduleOfDay.weather == WeatherState.Stormy)
        {
            int start = weatherScheduleOfDay.randomWeatherStartTime;
            int end = weatherScheduleOfDay.randomWeatherEndTime;
            iconWeatherTimeline.sprite = stormyIcon;
            SetTextTimeline(start, end);
        }
        else if (weatherScheduleOfDay.weather == WeatherState.Snowy)
        {
            int start = weatherScheduleOfDay.randomWeatherStartTime;
            int end = weatherScheduleOfDay.randomWeatherEndTime;
            iconWeatherTimeline.sprite = snowyIcon;
            SetTextTimeline(start,end);
        }
    }
    public void SetTextTimeline(int start, int end)
    {
        if (start != 6 && end != 24)
        {
            textTimeline1.text = $"6h - {start}h\n{end}h - 24h";
            textTimeline2.text = $"{start}h - {end}h";
        }
        else if (start == 6)
        {
            textTimeline1.text = $"{end}h - 24h";
            textTimeline2.text = $"{start}h - {end}h";
        }
        else if (end == 24)
        {
            textTimeline1.text = $"6h - {start}h";
            textTimeline2.text = $"{start}h - {end}h";
        }
    }
    #endregion

    #region ----- STAMINA UI -----
    void UpdateStamina()
    {
        if (staminaBar.value > 0 && CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.runState)
        {
            staminaBar.value -= 0.04f * Time.deltaTime;
        }
        else
        {
            staminaBar.value += 0.25f * Time.deltaTime;
        }
    }
    #endregion

    #region ----- ACTION UI -----
    public void ActionTime(float timeAnimation)
    {
        if (CharacterStateMachine.Instance.currentCharacter == ECharacter.Rin)
        {
            actionBar.value += Time.deltaTime * 1f / (timeAnimation * 0.8f);
        }
        else
        {
            actionBar.value += Time.deltaTime * 1f / timeAnimation;
        }
    }
    public void ResetBar()
    {
        actionBar.value = 0;       
    }
    #endregion

    #region ----- AVATAR CHARACTER UI -----
    public void SetAvatarCharacter()
    {
        switch (CharacterStateMachine.Instance.currentCharacter)
        {
            case ECharacter.Rin:
                avatarUI.sprite = avatarCharacter[0];
                break;
            case ECharacter.May:
                avatarUI.sprite = avatarCharacter[1];
                break;
            case ECharacter.Kai:
                avatarUI.sprite = avatarCharacter[2];
                break;
            case ECharacter.Max:
                avatarUI.sprite = avatarCharacter[3];
                break;
            case ECharacter.Hana:
                avatarUI.sprite = avatarCharacter[4];
                break;
            case ECharacter.Leon:
                avatarUI.sprite = avatarCharacter[5];
                break;
        }
    }
    #endregion
}

