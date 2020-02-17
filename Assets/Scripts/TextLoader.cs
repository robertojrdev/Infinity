using UnityEngine;
using UnityEngine.UI;

public class TextLoader : MonoBehaviour
{
    [SerializeField] public Text text;
    public string id;

    private void Start()
    {
        LoadText();
        LanguageManager.onUpdate += LoadText;
    }

    private void OnDestroy()
    {
        LanguageManager.onUpdate -= LoadText;
    }

    private void LoadText()
    {
        if(!text)
            return;
        var value = LanguageManager.GetString(id);
        text.text = value;
    }

    private void Reset()
    {
        text = GetComponent<Text>();
    }
}