using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Infomation")]
    public string characterName;
    public Sprite characterIcon;
    public GameObject characterPrefab;

    [Header("Stamina and capacity")]
    public int maxHealth;
    public int maxStamina;
    public float moveSpeed;
    public float jumpForce;
}