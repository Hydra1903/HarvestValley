using UnityEngine;
public class LevelManager : MonoBehaviour
{
    public int currentLevel = 1;
    public int levelMax = 30;
    public int[] xpThresholds;
    public LevelUI levelUI;
    public void CheckLevelUp()
    {
        if (Xp.Instance.xp >= xpThresholds[currentLevel - 1])
        {
            Xp.Instance.xp -= xpThresholds[currentLevel - 1];
            currentLevel++;
            levelUI.ShowPanelLevelUp();
        }
    }   
}
