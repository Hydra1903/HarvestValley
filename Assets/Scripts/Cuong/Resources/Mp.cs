using UnityEngine;

public class Mp : MonoBehaviour
{
    public static Mp Instance;
    public int mp = 0;
    public int maxMana;
    public MainUIScreen mainUIScreen;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        SetMaxMpCharacter();
    }
    public void SetMaxMpCharacter()
    {
        switch (CharacterStateMachine.Instance.currentCharacter)
        {
            case ECharacter.Rin:
                maxMana = 100;
                break;
            case ECharacter.May:
                maxMana = 85;
                break;
            case ECharacter.Kai:
                maxMana = 110;
                break;
            case ECharacter.Max:
                maxMana = 100;
                break;
            case ECharacter.Hana:
                maxMana = 90;
                break;
            case ECharacter.Leon:
                maxMana = 130;
                break;
        }
        mainUIScreen.UpdateMpUI();
    }
    public void UseMp(int amount)
    {
        if (mp >= amount)
        {
            mp -= amount;
            mainUIScreen.UpdateMpUI();
        }
        else
        {
            Notification.Instance.ShowNotification("Hết năng lượng!");
        }
    }
    public void PlusMp(int amount)
    {
        if (mp < maxMana)
        {
            mp = Mathf.Min(mp + amount, maxMana);
            mainUIScreen.UpdateMpUI();
        }
    }
    public void ResetMp(bool passOut)
    {
        if (!passOut)
        {
            mp = maxMana;
        }
        else
        {
            mp = maxMana/2;
        }
        mainUIScreen.UpdateMpUI();
    }
}
