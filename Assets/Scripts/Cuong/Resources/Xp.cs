using UnityEngine;

public class Xp : MonoBehaviour
{
    public static Xp Instance;
    public int xp = 0;

    public LevelManager levelManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void AddXp(int amount)
    {
        if (levelManager.currentLevel < levelManager.levelMax)
        {
            xp += amount;
            levelManager.CheckLevelUp();
        }
    }
}
