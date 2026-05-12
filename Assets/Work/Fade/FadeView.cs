using System;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace Work.Fade
{
    /// <summary>
    /// 화면 페이드 뷰
    /// </summary>
    public class FadeView : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;

        private MotionHandle _fadeHandle;

        /// <summary>
        /// 현재 알파값
        /// </summary>
        public float Alpha => fadeImage.color.a;

        /// <summary>
        /// 컴포넌트 참조 초기화
        /// </summary>
        private void Awake()
        {
            ResolveImage();
        }

        /// <summary>
        /// 에디터 참조 자동 설정
        /// </summary>
        private void Reset()
        {
            ResolveImage();
        }

        /// <summary>
        /// 페이드 알파 연출
        /// </summary>
        public void PlayFade(float targetAlpha, float duration, Action onComplete)
        {
            ResolveImage();
            CancelFade();

            _fadeHandle = LMotion.Create(Alpha, targetAlpha, duration)
                .WithOnComplete(() => onComplete?.Invoke())
                .Bind(SetAlpha)
                .AddTo(gameObject);
        }

        /// <summary>
        /// 페이드 연출 취소
        /// </summary>
        public void CancelFade()
        {
            _fadeHandle.TryCancel();
        }

        /// <summary>
        /// 알파값 직접 반영
        /// </summary>
        private void SetAlpha(float alpha)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }

        /// <summary>
        /// 이미지 참조 확인
        /// </summary>
        private void ResolveImage()
        {
            if (fadeImage == null)
            {
                fadeImage = GetComponent<Image>();
            }
        }
    }
}
