using UnityEngine;

public class MerchantRandom : MonoBehaviour
{
    public static MerchantRandom Instance;
    public bool isMerchantSpawned;
    public GameObject merchant;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        LoadMerchant();
    }

    public void LoadMerchant()
    {
        if (isMerchantSpawned)
        {
            merchant.SetActive(true);
        }
        else
        {
            merchant.SetActive(false);
        }      
    }
    public void RandomSpawnInDay()
    {
        if (Season.Instance.currentSeason != SeasonState.Winter)
        {
            isMerchantSpawned = false;
            float random = Random.value;
            if (random <= 0.7f)
            {
                isMerchantSpawned = true;
            }
        }
        SaveManager.SaveIsMerchantSpawned("slot1");
        LoadMerchant();
    }

}
