using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    public Button menuButton;
    public GameObject levelMenu;
    public GameObject languageMenu;

    [Space(10), Header("Language Menu")]
    public Button langButtonPT;
    public Button langButtonEN;
    public Button langButtonES;

    [Space(10), Header("Levels Menu")]
    public Transform levelButtonsHolder;
    public Button levelButtonPrefab;
    public Button minusLevelButton;
    public Button plusLevelButton;
    public Button resumeButton;
    public Text levelPackText;


    private int currentLevelPack = 0;

    private void Start()
    {
        menuButton.onClick.AddListener(GameManager.instance.PauseGame);
        resumeButton.onClick.AddListener(GameManager.instance.ResumeGame);

        langButtonPT.onClick.AddListener(() => LanguageManager.SetLanguage("pt"));
        langButtonEN.onClick.AddListener(() => LanguageManager.SetLanguage("en"));
        langButtonES.onClick.AddListener(() => LanguageManager.SetLanguage("es"));

        minusLevelButton.onClick.AddListener(() => ChangeLevelPack(false));
        plusLevelButton.onClick.AddListener(() => ChangeLevelPack(true));

        currentLevelPack = GameManager.CurrentLevel.x;
        LoadLevelsButtons(currentLevelPack);
    }

    private void ChangeLevelPack(bool increase)
    {
        var previous = currentLevelPack;
        currentLevelPack += increase ? 1 : -1;
        currentLevelPack = Mathf.Clamp(currentLevelPack, 0, GameManager.LevelsFile.levels.Count);
        print(increase + " - " + previous + "/" + currentLevelPack);
        if (currentLevelPack != previous)
            LoadLevelsButtons(currentLevelPack);
    }

    private void UpdateLevelButtons()
    {
        LoadLevelsButtons(currentLevelPack);
    }

    private void LoadLevelsButtons(int levelPackage)
    {
        //clear previous buttons
        foreach (Transform child in levelButtonsHolder)
        {
            Destroy(child.gameObject);
        }

        //instantiate new ones
        var currentLevel = GameManager.CurrentLevel;
        var file = GameManager.LevelsFile;
        var levels = file.levels[currentLevelPack].Levels;

        for (int i = 0; i < levels.Count; i++)
        {
            var button = Instantiate(levelButtonPrefab, levelButtonsHolder);
            button.GetComponentInChildren<Text>().text = i.ToString();
            int lIndex = i;
            button.onClick.AddListener(() => LoadLevelFromMenu(new Vector2Int(levelPackage, lIndex)));

            if (currentLevel.x < levelPackage || currentLevel.y < i)
                button.interactable = false;
        }
    }

    private void LoadLevelFromMenu(Vector2Int level)
    {
        CloseMenus();
        GameManager.LoadNewGame(level);
    }

    public void OpenMenu()
    {
        menuButton.gameObject.SetActive(false);
        levelMenu.SetActive(true);
    }

    public void CloseMenus()
    {
        menuButton.gameObject.SetActive(true);
        levelMenu.SetActive(false);
        languageMenu.SetActive(false);
    }
}