using UnityEngine;

[System.Serializable]
public class GameState
{
    private const string PREFS_SAVE_GAME = "save-game";
 
    public Vector2Int currentLevel;
    public string selectedLanguage = "en";

    public void SaveGame()
    {
        var json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(PREFS_SAVE_GAME, json);
    }

    public static GameState LoadOrDefault()
    {
        if (PlayerPrefs.HasKey(PREFS_SAVE_GAME))
        {
            return JsonUtility.FromJson<GameState>(
                PlayerPrefs.GetString(PREFS_SAVE_GAME));
        }
        else
        {
            return new GameState();
        }
    }
}