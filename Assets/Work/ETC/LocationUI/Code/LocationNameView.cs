using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Work.ETC.LocationUI.Code
{
    /// <summary>
    /// 로케이션 이름 표시 뷰
    /// </summary>
    public class LocationNameView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI locationNameText;
        [SerializeField] private Image locationNameLine;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float displayDuration = 2.5f;

        private MotionHandle _fadeInHandle;
        private MotionHandle _fadeOutHandle;

        /// <summary>
        /// 초기 표시 상태 설정
        /// </summary>
        private void Awake()
        {
            Hide();
        }

        /// <summary>
        /// 연출 정리 처리
        /// </summary>
        private void OnDestroy()
        {
            CancelMotions();
        }

        /// <summary>
        /// 로케이션 이름 표시 연출
        /// </summary>
        public async UniTask PlayAsync(string locationName, CancellationToken cancellationToken)
        {
            ShowLocationName(locationName);
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration + displayDuration + fadeDuration), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 로케이션 이름 표시 시작
        /// </summary>
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

        /// <summary>
        /// 로케이션 이름 숨김 처리
        /// </summary>
        public void Hide()
        {
            CancelMotions();
            locationNameText.gameObject.SetActive(false);
            locationNameLine.gameObject.SetActive(false);
            SetAlpha(0f);
        }

        /// <summary>
        /// 로케이션 이름 연출 정리
        /// </summary>
        public void CancelMotions()
        {
            _fadeInHandle.TryCancel();
            _fadeOutHandle.TryCancel();
        }

        /// <summary>
        /// 알파값 반영
        /// </summary>
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
