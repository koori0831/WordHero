using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;

namespace Work.ETC.LocationUI.Code
{
    public record struct OnShowLocationNameEvent(string LocationName) : IEvent;

    /// <summary>
    /// 발행됐을 때 로케이션 UI를 띄우는 이벤트
    /// </summary>
    public readonly record struct PlayLocationUIEvent : IEvent;

    public class LocationNameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI locationNameText;
        [SerializeField] private Image locationNameLine;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float displayDuration = 2.5f;

        private MotionHandle _fadeInHandle;
        private MotionHandle _fadeOutHandle;

        public void Awake()
        {
            Bus<OnShowLocationNameEvent>.Events += HandleShowLocationNameEvent;
            locationNameText.gameObject.SetActive(false);
            locationNameLine.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<OnShowLocationNameEvent>.Events -= HandleShowLocationNameEvent;
            CancelMotions();
        }

        private void HandleShowLocationNameEvent(OnShowLocationNameEvent evt)
        {
            ShowLocationName(evt.LocationName);
        }

        public void ShowLocationName(string locationName)
        {
            CancelMotions();
            locationNameText.text = locationName;
            locationNameText.gameObject.SetActive(true);
            locationNameLine.gameObject.SetActive(true);
            SetAlpha(0f);

            _fadeInHandle = LMotion.Create(0f, 1f, fadeDuration)
                .WithEase(Ease.InCubic)
                .WithOnComplete(() =>
                {
                    _fadeOutHandle = LMotion.Create(1f, 0f, fadeDuration)
                        .WithEase(Ease.OutCubic)
                        .WithDelay(displayDuration)
                        .WithOnComplete(() =>
                        {
                            locationNameText.gameObject.SetActive(false);
                            locationNameLine.gameObject.SetActive(false);
                        })
                        .Bind(SetAlpha)
                        .AddTo(gameObject);
                }
                )
                .Bind(SetAlpha)
                .AddTo(gameObject);
        }

        private void CancelMotions()
        {
            _fadeInHandle.TryCancel();
            _fadeOutHandle.TryCancel();
        }

        private void SetAlpha(float alpha)
        {
            Color textColor = locationNameText.color;
            textColor.a = alpha;
            locationNameText.color = textColor;

            Color lineColor = locationNameLine.color;
            lineColor.a = alpha;
            locationNameLine.color = lineColor;
        }
    }
}
