using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
}
