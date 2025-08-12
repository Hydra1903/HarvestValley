using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class MainUIScreen : MonoBehaviour
{
    [Header ("--- Level UI ---")]
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
    void Start()
    {
        UpdateXpUI();
        UpdateMpUI();
        UpdateGold();
    }
    void Update()
    {
        UpdateTime();
    }

    #region ----- LEVEL XP UI -----
    public void UpdateXpUI()
    {
        if (levelManager.currentLevel < levelManager.levelMax)
        {
            xpBar.value = (float)Xp.Instance.xp / levelManager.xpThresholds[levelManager.currentLevel - 1];
            textCurrentXp.text = Xp.Instance.xp.ToString() + "/" + levelManager.xpThresholds[levelManager.currentLevel - 1] + " XP";
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
}
