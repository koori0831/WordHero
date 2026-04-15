using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Weapons.Imprint.Code;

public class WordItem : MonoBehaviour
{
    [SerializeField] private TMP_Text wordName;
    [SerializeField] private TMP_Text wordDescription;
    public Button button;

    public ImprintWordSO Word { get; private set; }

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void SetWord(string name, string description)
    {
        wordName.text = name;
        wordDescription.text = description;
    }

    public void SetWord(ImprintWordSO word, int amount)
    {
        Word = word;
        SetWord(word.Name, $"{word.Description}\n보유: x{amount}");
    }
}
