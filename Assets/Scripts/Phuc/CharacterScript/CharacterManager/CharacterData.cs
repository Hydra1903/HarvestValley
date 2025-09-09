using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Infomation")]
    public string characterName;
    public Sprite characterIcon;
    public string sex;
    public string age;
    public string habit;
    public string genitive;
    [Header("Stamina and capacity")]
    public int maxStamina;
    public string skill;
    public Mesh SkinnyCharacter;
}