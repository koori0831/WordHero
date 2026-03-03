using UnityEngine;

namespace Work.HPBar.Code
{
    public class HPField : MonoBehaviour, IUIElement<float, float>
    {
        [SerializeField] private FillUI hpLine, backgroundLine;

        public void Disable()
        {
            hpLine.Disable();
            backgroundLine.Disable();
            
        }

        public void EnableFor(float currentHealth, float maxHealth)
        {
            hpLine.EnableFor(currentHealth / maxHealth);
            backgroundLine.EnableFor(currentHealth / maxHealth);
        }

        public void HpChange(float currentHealth, float maxHealth)
        {
            hpLine.SetFill((currentHealth / maxHealth), () => backgroundLine.SetFill((currentHealth / maxHealth)));
        }
    }
}