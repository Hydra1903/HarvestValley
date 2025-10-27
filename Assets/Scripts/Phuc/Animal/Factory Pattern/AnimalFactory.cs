using UnityEngine;

public enum AnimalType
{
    None,
    WhiteSheep,
    CreamSheep,
    BlackSheep,
    WhiteGoat,
    BlackGoat
}

public static class AnimalFactory
{
    public static GameObject GetPrefab(AnimalType type)
    {
        switch (type)
        {
            case AnimalType.BlackGoat:
                return Resources.Load<GameObject>("Prefabs/BlackGoat");
            case AnimalType.WhiteGoat:
                return Resources.Load<GameObject>("Prefabs/WhiteGoat");
            case AnimalType.WhiteSheep:
                return Resources.Load<GameObject>("Prefabs/WhiteSheep");
            case AnimalType.BlackSheep:
                return Resources.Load<GameObject>("Prefabs/BlackSheep");
            case AnimalType.CreamSheep:
                return Resources.Load<GameObject>("Prefabs/CreamSheep");
            default:
                Debug.LogWarning("? Invalid or missing animal type prefab.");
                return null;
        }
    }
}
