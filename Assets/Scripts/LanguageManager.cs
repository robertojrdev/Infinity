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
    public static Action onUpdate;

    [SerializeField] private TextAsset languageJSON;

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

        LoadLanguages();
    }

    private void Start()
    {
        SetLanguage(GameManager.GameState.selectedLanguage);
    }

    private void LoadLanguages()
    {
        values = JsonConvert
            .DeserializeObject<Dictionary<string, Dictionary<string, string>>>(
                languageJSON.ToString());

        if (onUpdate != null)
            onUpdate.Invoke();
    }

    public static void SetLanguage(string lang)
    {
        if (values == null)
        {
            Debug.LogError("Languages not loaded");
            return;
        }

        if (!values.ContainsKey(lang))
        {
            Debug.LogError("Language '" + lang + "' not found");
            return;
        }

        GameManager.GameState.selectedLanguage = lang;
        GameManager.GameState.SaveGame();

        print("Language changed");

        if (onUpdate != null)
            onUpdate.Invoke();
    }

    public static string GetString(string id)
    {
        if (values == null)
        {
            Debug.LogError("Languages not loaded");
            return "<>";
        }

        if (!values[GameManager.GameState.selectedLanguage].ContainsKey(id))
        {
            Debug.LogWarning("string '" + id + "' not found in current language (" + GameManager.GameState.selectedLanguage + ")");
            return "<>";
        }

        return values[GameManager.GameState.selectedLanguage][id];
    }
}