using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Fade
{
    /// <summary>
    /// 화면 페이드 표시 제어자
    /// </summary>
    public class FadePresenter : MonoBehaviour
    {
        [SerializeField] private FadeView fadeView;
        [SerializeField] private float fadeDuration = 1.0f;

        /// <summary>
        /// 초기 구독 및 시작 페이드 처리
        /// </summary>
        private void Awake()
        {
            ResolveView();
            Bus<OnFadeEvent>.Events += HandleFadeEvent;
            fadeView.PlayFade(0f, fadeDuration, null);
        }

        /// <summary>
        /// 페이드 요청 처리
        /// </summary>
        private void HandleFadeEvent(OnFadeEvent evt)
        {
            ResolveView();
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

            float targetAlpha = evt.isFadeIn ? 1f : 0f;
            fadeView.PlayFade(targetAlpha, fadeDuration, () =>
            {
                Bus<OnFadeCompletedEvent>.Raise(new OnFadeCompletedEvent(evt.isFadeIn));
                if (evt.isFadeIn == false)
                {
                    Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
                }
            });
        }

        /// <summary>
        /// 이벤트 구독 해제 처리
        /// </summary>
        private void OnDestroy()
        {
            Bus<OnFadeEvent>.Events -= HandleFadeEvent;
            if (fadeView != null)
            {
                fadeView.CancelFade();
            }
        }

        /// <summary>
        /// 뷰 참조 확인
        /// </summary>
        private void ResolveView()
        {
            if (fadeView == null)
            {
                fadeView = GetComponent<FadeView>();
            }
        }
    }
}
