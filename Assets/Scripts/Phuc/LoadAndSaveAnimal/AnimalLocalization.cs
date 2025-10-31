using UnityEngine;
using UnityEngine.Localization.Settings;

public static class AnimalLocalization
{
    public static string GetLocalizedName(AnimalType type)
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "vi";

        bool isEnglish = code.StartsWith("en");

        string text;

        switch (type)
        {
            case AnimalType.WhiteSheep: text = isEnglish ? "White Sheep" : "Cừu trắng"; break;
            case AnimalType.BlackSheep: text = isEnglish ? "Black Sheep" : "Cừu đen"; break;
            case AnimalType.CreamSheep: text = isEnglish ? "Cream Sheep" : "Cừu kem"; break;
            case AnimalType.WhiteGoat: text = isEnglish ? "White Goat" : "Dê trắng"; break;
            case AnimalType.BlackGoat: text = isEnglish ? "Black Goat" : "Dê đen"; break;
            default: text = isEnglish ? "Unknown" : "Không xác định"; break;
        }
        return FixEncoding(text);
    }

    private static string FixEncoding(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        byte[] bytes = System.Text.Encoding.Default.GetBytes(text);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}


public static class AnimalHelpers
{
    public static AnimalType MapToAnimalType(AnimalTypeed typeEd, string variant)
    {
        if (typeEd == AnimalTypeed.None)
            return AnimalType.None;

        string v = (variant ?? "").ToLowerInvariant().Trim();

        bool isWhite = v.Contains("white") || v.Contains("trắng") || v.Contains("white sheep") || v.Contains("white goat");
        bool isBlack = v.Contains("black") || v.Contains("đen") || v.Contains("black sheep") || v.Contains("black goat");
        bool isCream = v.Contains("cream") || v.Contains("kem") || v.Contains("cream sheep");

        switch (typeEd)
        {
            case AnimalTypeed.Goat:
                if (isWhite) return AnimalType.WhiteGoat;
                if (isBlack) return AnimalType.BlackGoat;
                return AnimalType.WhiteGoat; 

            case AnimalTypeed.Sheep:
                if (isWhite) return AnimalType.WhiteSheep;
                if (isBlack) return AnimalType.BlackSheep;
                if (isCream) return AnimalType.CreamSheep;
                return AnimalType.WhiteSheep; 

            default:
                return AnimalType.None;
        }
    }

    public static string GetDisplayNameFromData(AnimalTypeed typeEd, string variant)
    {
        var mapped = MapToAnimalType(typeEd, variant);
        return AnimalLocalization.GetLocalizedName(mapped);
    }
}
