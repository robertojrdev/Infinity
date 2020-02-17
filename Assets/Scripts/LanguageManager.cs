using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance { get; private set; }


    public static Dictionary<string, Dictionary<string, string>> values =
        new Dictionary<string, Dictionary<string, string>>();
    public static string currentLanguage {get; private set;} = "en";
    public static Action onUpdate;

    [SerializeField] private TextAsset languageJSON;
    [SerializeField] private string defaltLanguage = "en";

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Destroy(this);
            Debug.LogWarning("Multiple language managers not allowed");
            return;
        }

        currentLanguage = defaltLanguage;
        LoadLanguages();
    }

    private void LoadLanguages()
    {
        values = JsonConvert
            .DeserializeObject<Dictionary<string, Dictionary<string, string>>>(
                languageJSON.ToString());

        print("Loaded languages");

        if(onUpdate != null)
            onUpdate.Invoke();
    }

    public static void SetLanguage(string lang)
    {
        if(values == null)
        {
            Debug.LogError("Languages not loaded");
            return;
        }

        if(!values.ContainsKey(lang))
        {
            Debug.LogError("Language '" + lang + "' not found");
            return;
        }

        currentLanguage = lang;
        
        print("Language changed");
        
        if(onUpdate != null)
            onUpdate.Invoke();
    }

    public static string GetString(string id)
    {
        if(values == null)
        {
            Debug.LogError("Languages not loaded");
            return "<>";
        }

        if(!values[currentLanguage].ContainsKey(id))
        {
            Debug.LogWarning("string '" + id + "' not found in current language (" + currentLanguage +")");
            return "<>";
        }

        return values[currentLanguage][id];
    }
}