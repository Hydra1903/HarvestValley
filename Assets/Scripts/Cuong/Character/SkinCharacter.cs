using UnityEngine;

public class SkinCharacter : MonoBehaviour
{
    public GameObject[] skinCharacter;
    void Start()
    {
        switch (CharacterStateMachine.Instance.currentCharacter)
        {
            case ECharacter.Rin:
                skinCharacter[0].SetActive(true);
                break;
            case ECharacter.May:
                skinCharacter[1].SetActive(true);
                break;
            case ECharacter.Kai:
                skinCharacter[2].SetActive(true);
                break;
            case ECharacter.Max:
                skinCharacter[3].SetActive(true);
                break;
            case ECharacter.Hana:
                skinCharacter[4].SetActive(true);
                break;
            case ECharacter.Leon:
                skinCharacter[5].SetActive(true);
                break;
        }
    }
}
