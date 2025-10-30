using UnityEngine;
using UnityEngine.UI;

public enum AnimalTypeed { None, Goat, Sheep }
public enum Static {None, Good, Bad }
[CreateAssetMenu(menuName ="Animal/Sample")]
public class AnimalData : ScriptableObject
{
    public string animalName;
    public ItemData item;
    public Sprite icon;
    public AnimalTypeed animalType;
    [Header("Variant")]
    public string variant;
}