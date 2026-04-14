using LitMotion;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Weapons.Imprint.Code;

namespace Work.Weapons.Imprint.Code.TestUI
{
    public class TriggerUITest : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform progressBar;

        private MotionHandle _progressHandle;
        private float _progressBaseWidth;
        private float _progressBaseHeight;

        private void Awake()
        {
            if (progressBar != null)
            {
                _progressBaseWidth = progressBar.sizeDelta.x;
                _progressBaseHeight = progressBar.sizeDelta.y;
            }
            HideUI();
        }

        private void OnEnable()
        {
            Bus<WeaponTriggerOpenedEvent>.Events += OnTriggerOpened;
            Bus<WeaponTriggerActivatedEvent>.Events += OnTriggerActivated;
        }

        private void OnDisable()
        {
            Bus<WeaponTriggerOpenedEvent>.Events -= OnTriggerOpened;
            Bus<WeaponTriggerActivatedEvent>.Events -= OnTriggerActivated;
            _progressHandle.TryCancel();
        }

        private void OnTriggerOpened(WeaponTriggerOpenedEvent evt)
        {
            _progressHandle.TryCancel();
            SetCanvasVisible(true);
            SetProgress(1f);

            _progressHandle = LMotion.Create(1f, 0f, evt.Duration)
                .WithEase(Ease.Linear)
                .WithOnComplete(HideUI)
                .Bind(SetProgress)
                .AddTo(gameObject);
        }

        private void OnTriggerActivated(WeaponTriggerActivatedEvent evt)
        {
            HideUI();
        }

        private void SetProgress(float value)
        {
            if (progressBar == null)
                return;

            progressBar.sizeDelta = new Vector2(_progressBaseWidth * value, _progressBaseHeight);
        }

        private void HideUI()
        {
            _progressHandle.TryCancel();
            SetCanvasVisible(false);
            SetProgress(0f);
        }

        private void SetCanvasVisible(bool isVisible)
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}
