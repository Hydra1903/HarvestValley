using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
    public static LanguageSwitcher Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void SetLanguage(string code)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == code)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }
    }
}

