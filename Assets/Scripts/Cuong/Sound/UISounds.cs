using UnityEngine;

public class UISounds : MonoBehaviour
{
    public static UISounds Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public AudioSource audioSourceUI;
    public AudioClip openUI;
    public AudioClip closeUI;
    public AudioClip clickButton;
    public AudioClip collectItem;
    public AudioClip levelUp;
    public AudioClip completeAchievement;
    public AudioClip upgradeBuilding;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void PlaySound_OpenUI()
    {
        audioSourceUI.clip = openUI;
        audioSourceUI.Play();
    }
    public void PlaySound_CloseUI()
    {
        audioSourceUI.clip = closeUI;
        audioSourceUI.Play();
    }
    public void PlaySound_ClickButton()
    {
        audioSourceUI.clip = clickButton;
        audioSourceUI.Play();
    }
    public void PlaySound_CollectItem()
    {
        audioSourceUI.clip = collectItem;
        audioSourceUI.Play();
    }
    public void PlaySound_LevelUp()
    {
        audioSourceUI.clip = levelUp;
        audioSourceUI.Play();
    }
    public void PlaySound_CompleteAchievement()
    {
        audioSourceUI.clip = completeAchievement;
        audioSourceUI.Play();
    }
    public void PlaySound_UpgradeBuilding()
    {
        audioSourceUI.clip = upgradeBuilding;
        audioSourceUI.Play();
    }

}
