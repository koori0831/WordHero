using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text cost;

    public void SetIcon(Sprite sprite, int skillCost, Color color)
    {
        icon.sprite = sprite;
        cost.text = skillCost.ToString();
        cost.color = color;
    }
}
