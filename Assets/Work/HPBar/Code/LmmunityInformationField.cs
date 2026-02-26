using TMPro;
using UnityEngine;

namespace Work.HPBar.Code
{
    public class LmmunityInformationField : MonoBehaviour, IUIElement<string>
    {
        [SerializeField] private TextMeshProUGUI statusTextField;

        public void Disable()
        {
            gameObject.SetActive(false);
            // 면역 정보 숨김 로직 추가
        }

        public void EnableFor(string item)
        {
            gameObject.SetActive(true);
            statusTextField.SetText(item);
        }

        internal void SetStatusText(string statusText)
        {
            statusTextField.SetText(statusText);
        }
    }
}