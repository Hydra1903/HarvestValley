using UnityEngine;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int currentLevel = 1;
    public int levelMax = 30;
    public int[] xpThresholds;
    public MainUIScreen mainUIScreen;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void CheckLevelUp()
    {
        if (Xp.Instance.xp >= xpThresholds[currentLevel - 1])
        {
            Xp.Instance.xp -= xpThresholds[currentLevel - 1];
            currentLevel++;
            mainUIScreen.ShowPanelLevelUp();
        }
    }   
}
