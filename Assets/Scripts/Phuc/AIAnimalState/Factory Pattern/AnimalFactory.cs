using System.Linq;
using UnityEngine;

public enum AnimalType { None, WhiteSheep, CreamSheep, BlackSheep, BlackGoat, WhiteGoat }

public static class AnimalFactory
{
    public static GameObject GetPrefab(AnimalType type)
    {
       
        switch (type)
        {
            case AnimalType.BlackGoat:
                return  Resources.Load<GameObject>("Prefabs/BlackGoat");
                break;
            case AnimalType.WhiteGoat:
                return Resources.Load<GameObject>("Prefabs/WhiteGoat");
                break;
            case AnimalType.WhiteSheep:
                return Resources.Load<GameObject>("Prefabs/WhiteSheep");
                break;
            case AnimalType.BlackSheep:
                return Resources.Load<GameObject>("Prefabs/BlackSheep");
                break;
            case AnimalType.CreamSheep:
                return Resources.Load<GameObject>("Prefabs/CreamSheep");
                break;
            default:
                Debug.LogWarning("Invalid or unselected animal type.");
                return null;
        }
      
    }
}
