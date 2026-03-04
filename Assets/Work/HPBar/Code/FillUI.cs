using LitMotion;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Work.HPBar.Code
{
    public class FillUI : MonoBehaviour, IUIElement<float>
    {
        [SerializeField] private Image fiilImage;

        private MotionHandle _currentMotion;

        private const float ANIMATION_DURATION = 0.25f;

        public void Disable()
        {
            fiilImage.fillAmount = 0;
            _currentMotion.TryCancel();
            fiilImage.gameObject.SetActive(false);
        }

        public void EnableFor(float fillAmount)
        {
            fiilImage.gameObject.SetActive(true);
            fiilImage.fillAmount = fillAmount;
        }

        public void SetFill(float amount, Action callback = null)
        {
            float currentFill = fiilImage.fillAmount;

            _currentMotion.TryCancel();

            _currentMotion = LMotion.Create(currentFill, amount, ANIMATION_DURATION)
                .WithEase(Ease.OutQuart)
                .WithOnComplete(() => callback?.Invoke())
                .Bind(x => fiilImage.fillAmount = x);
        }
    }
}