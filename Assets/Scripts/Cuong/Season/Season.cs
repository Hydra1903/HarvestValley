using UnityEngine;
public enum SeasonState 
{
    Spring,
    Summer,
    Fall,
    Winter
}
public class Season : MonoBehaviour
{
    public static Season Instance;
    public SeasonState currentSeason = SeasonState.Spring;
    public MainUIScreen mainUIScreen;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void ChangeOfSeasons()
    {
        switch (currentSeason)
        {
            case SeasonState.Spring:
                currentSeason = SeasonState.Summer;
                break;
            case SeasonState.Summer:
                currentSeason = SeasonState.Fall;
                break;
            case SeasonState.Fall:
                currentSeason = SeasonState.Winter;
                break;
            case SeasonState.Winter:
                currentSeason = SeasonState.Spring;
                break;
        }
        mainUIScreen.UpdateSeason();
    }
}
