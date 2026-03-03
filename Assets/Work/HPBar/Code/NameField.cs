using TMPro;
using UnityEngine;

namespace Work.HPBar.Code
{
    public class NameField : MonoBehaviour, IUIElement<string>
    {
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private TextMeshProUGUI shadowNameField;

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void EnableFor(string name)
        {
            gameObject.SetActive(true);
            nameField.SetText(name);
            shadowNameField.SetText(name);
        }
    }
}