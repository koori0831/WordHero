using TMPro;
using UnityEngine;

namespace Work.HPBar.Code
{
    public class HPField : MonoBehaviour, IUIElement<float, float>
    {
        [SerializeField] private FillUI hpLine, backgroundLine;
        [SerializeField] private TextMeshProUGUI currentText, maxText;

        public void Disable()
        {
            hpLine.Disable();
            backgroundLine.Disable();

        }

        public void EnableFor(float currentHealth, float maxHealth)
        {
            hpLine.EnableFor(currentHealth / maxHealth);
            backgroundLine.EnableFor(currentHealth / maxHealth);
            SetHpText(currentHealth, maxHealth);
        }

        private void SetHpText(float currentHealth, float maxHealth)
        {
            string cur = Mathf.Clamp(currentHealth, 0, maxHealth).ToString("N0");
            string max = "/ " + maxHealth.ToString("N0");
            currentText.text = cur;
            maxText.text = max;
        }

        public void HpChange(float currentHealth, float maxHealth)
        {
            hpLine.SetFill((currentHealth / maxHealth), () => backgroundLine.SetFill((currentHealth / maxHealth)));

            SetHpText(currentHealth, maxHealth);


        }
    }
}