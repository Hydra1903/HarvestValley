using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum ECharacter
{
    Rin,
    May,
    Kai,
    Max,
    Hana,
    Leon
}
public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public GameObject[] InforCharacter;
    public GameObject[] Character;
    public int currentLocation = 0;
    public TextMeshProUGUI pages;
    public static ECharacter currentCharacter = ECharacter.Rin;
    public void ChangeCharacter(int index)
    {
        currentLocation += index;
        currentLocation = Mathf.Clamp(currentLocation, 0, InforCharacter.Length - 1);
        for (int i = 0; i < InforCharacter.Length; i++)
        {
            if (currentLocation == i)
            {
                InforCharacter[i].SetActive(true);
                Character[i].SetActive(true);
            }
            else
            {
                InforCharacter[i].SetActive(false);
                Character[i].SetActive(false);
            }
        }
        pages.text = (currentLocation + 1) + "/" + InforCharacter.Length;
    }
    public void SelectCharacter()
    {
        switch (currentLocation)
        {
            case 0:
                currentCharacter = ECharacter.Rin;
                break;
            case 1:
                currentCharacter = ECharacter.May;
                break;
            case 2:
                currentCharacter = ECharacter.Kai;
                break;
            case 3:
                currentCharacter = ECharacter.Max;
                break;
            case 4:
                currentCharacter = ECharacter.Hana;
                break;
            case 5:
                currentCharacter = ECharacter.Leon;
                break;
        }
    }
    private void Update()
    {
        Debug.Log(currentCharacter);
    }
}
