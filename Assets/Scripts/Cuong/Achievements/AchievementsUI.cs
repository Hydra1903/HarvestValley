using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.FilePathAttribute;

public class AchivementsUI : MonoBehaviour
{
    public static AchivementsUI Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public Button[] buttonReward;
    public GameObject[] prevent;
    public bool[] isReward;

    public TextMeshProUGUI harvestedCropsCountText;
    public TextMeshProUGUI typesOfCropsPlantedCountText;
    public TextMeshProUGUI timesWateredCountText;
    public TextMeshProUGUI greenhouseCropsHarvestedCountText;
    public TextMeshProUGUI animalProductsCollectedCountText;
    public TextMeshProUGUI farmProductsSoldCountText;
    public TextMeshProUGUI perennialHarvestsCountText;
    public TextMeshProUGUI buildingsUpgradedOrUnlockedCountText;
    public TextMeshProUGUI staminaUsedCountText;
    public TextMeshProUGUI totalMoneyEarnedCountText;
    void Start()
    {
        SetListener();
        LoadAchievementsUI();
        LoadStatistics();
    }
    public void SetListener()
    {
        for (int i = 0; i < buttonReward.Length; i++)
        {
            int index = i; 
            buttonReward[index].onClick.AddListener(() =>
            {
                Gold.Instance.AddGold(Achivements.Instance.Gold[index]);
                Xp.Instance.AddXp(Achivements.Instance.Xp[index]);
                isReward[index] = true;
                prevent[index].SetActive(true);
            });
        }
    }
    public void LoadAchievementsUI()
    {
        for (int i = 0; i < buttonReward.Length; i++)
        {
            if (isReward[i])
            {
                prevent[i].SetActive(true);
            }
            else
            {
                prevent[i].SetActive(false);
            }
        }
        for (int i = 0; i < buttonReward.Length; i++)
        {
            if (Achivements.Instance.isAchivementComplete[i])
            {
                buttonReward[i].interactable = true;
            }
            else
            {
                buttonReward[i].interactable = false;
            }
        }
    }
    public void LoadStatistics()
    {
        harvestedCropsCountText.text = Achivements.Instance.harvestedCropsCount.ToString();
        typesOfCropsPlantedCountText.text = Achivements.Instance.typesOfCropsPlantedCount.ToString();
        timesWateredCountText.text = Achivements.Instance.timesWateredCount.ToString();
        greenhouseCropsHarvestedCountText.text = Achivements.Instance.greenhouseCropsHarvestedCount.ToString();
        animalProductsCollectedCountText.text = Achivements.Instance.animalProductsCollectedCount.ToString();
        farmProductsSoldCountText.text = Achivements.Instance.farmProductsSoldCount.ToString();
        perennialHarvestsCountText.text = Achivements.Instance.perennialHarvestsCount.ToString();
        buildingsUpgradedOrUnlockedCountText.text = Achivements.Instance.buildingsUpgradedOrUnlockedCount.ToString();
        staminaUsedCountText.text = Achivements.Instance.staminaUsedCount.ToString();
        totalMoneyEarnedCountText.text = Achivements.Instance.totalMoneyEarnedCount.ToString();
    }
}
