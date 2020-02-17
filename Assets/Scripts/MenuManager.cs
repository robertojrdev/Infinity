using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button langButtonPT;
    public Button langButtonEN;
    public Button langButtonES;

    private void Start()
    {
        langButtonPT.onClick.AddListener(() => LanguageManager.SetLanguage("pt"));
        langButtonEN.onClick.AddListener(() => LanguageManager.SetLanguage("en"));
        langButtonES.onClick.AddListener(() => LanguageManager.SetLanguage("es"));
    }
}