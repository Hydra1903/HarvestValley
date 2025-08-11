using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelUI : MonoBehaviour
{
    public GameObject[] panelLevelUp;
    public GameObject backgroundLevelUp;
    public TextMeshProUGUI textNumberLevelUp;
    public Slider xpBar;
    public TextMeshProUGUI textCurrentXp;
    public TextMeshProUGUI textCurrentLevel;

    public LevelManager levelManager;
    public int xpPlus;
    void Start()
    {
        UpdateXpUI();
    }
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
    public void PlusXp()
    {
        Xp.Instance.AddXp(xpPlus);
        UpdateXpUI();
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
}
