using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public void English ()
    {
        LanguageSwitcher.Instance.SetLanguage("en");
        Debug.Log("tieengs anh");
    }
    public void Vietnamese()
    {
        LanguageSwitcher.Instance.SetLanguage("vi-VN");
        Debug.Log("tieengs viet");
    }
}
