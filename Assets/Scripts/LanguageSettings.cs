using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageSettings : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private bool active = false;

    private void Start()
    {
        // 0 - english, 1 - russian
        if (PlayerPrefs.HasKey("language"))
        {
            int val = PlayerPrefs.GetInt("language");
            SetLanguage(val);
            return;
        }
        Debug.Log(LocalizationSettings.SelectedLocale);
        bool isLocaleRus = LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1];
        int language = (isLocaleRus ? 1 : 0);
        SetLanguage(language);
    }

    /// <summary>
    /// Changes game language, doesn't change dropdown
    /// </summary>
    /// <param name="locale_id">id of language</param>
    public void SetLanguage(int locale_id) 
    {
        if (active) return;

        if(locale_id < 0 || locale_id > 1)
        {
            Debug.LogWarning("incorrect language id");
            locale_id = 0;
        }
        dropdown.value = locale_id;
        StartCoroutine(SetLocale(locale_id));
    }
    private IEnumerator SetLocale(int locale_id)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[locale_id];
        PlayerPrefs.SetInt("language", locale_id);
        active = false;
    }
    //public void OnLanguageChanged(int value)
    //{
    //    SetLanguage(value);
    //}

    public static int GetLanguage()
    {
        return (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1]) ? 1 : 0;
    }
}
