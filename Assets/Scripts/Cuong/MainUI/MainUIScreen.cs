using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;


public class MainUIScreen : MonoBehaviour
{
    [Header("--- Level UI ---")]
    public GameObject[] panelLevelUp;
    public GameObject backgroundLevelUp;
    public TextMeshProUGUI textNumberLevelUp;
    public Slider xpBar;
    public TextMeshProUGUI textCurrentXp;
    public TextMeshProUGUI textCurrentLevel;
    public LevelManager levelManager;

    [Header("--- Mana UI ---")]
    public Slider mpBar;
    public TextMeshProUGUI textCurrentMp;

    [Header("--- Gold UI ---")]
    public TextMeshProUGUI textGold;

    [Header("--- Time UI ---")]
    public TextMeshProUGUI textTime;
    public TextMeshProUGUI textDay;

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
    void Start()
    {
        UpdateXpUI();
        UpdateMpUI();
        UpdateGold();
    }
    void Update()
    {

    }

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
    }

    IEnumerator Hide(GameObject currentPanelLevelUp)
    {
        yield return new WaitForSeconds(3f);
        backgroundLevelUp.SetActive(false);
        currentPanelLevelUp.SetActive(false);
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
        textGold.text = Gold.Instance.gold.ToString();
    }
    #endregion

    #region ----- TIME UI -----
    public void UpdateTime()
    {
        textTime.text = $"{GameTime.Instance.hour}:{GameTime.Instance.minute:00}";
        textDay.text = "Ngày " + GameTime.Instance.day.ToString();
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
}

