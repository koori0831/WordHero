using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
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

        private CancellationTokenSource _fadeCts;

        /// <summary>
        /// 초기 구독 및 시작 페이드 처리
        /// </summary>
        private void Awake()
        {
            ResolveView();
            BusObservable.On<OnFadeEvent>()
                .Subscribe(HandleFadeEvent)
                .AddTo(this);
            fadeView.PlayFade(0f, fadeDuration, null);
        }

        /// <summary>
        /// 페이드 요청 처리
        /// </summary>
        private void HandleFadeEvent(OnFadeEvent evt)
        {
            PlayFadeEventAsync(evt.isFadeIn).Forget();
        }

        /// <summary>
        /// 페이드 연출 대기 처리
        /// </summary>
        public async UniTask FadeAsync(bool isFadeIn, CancellationToken cancellationToken)
        {
            ResolveView();
            CancelFadeRequest();

            CancellationTokenSource localCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _fadeCts = localCts;

            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));
            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            float targetAlpha = isFadeIn ? 1f : 0f;

            fadeView.PlayFade(targetAlpha, fadeDuration, () => completionSource.TrySetResult());

            try
            {
                await completionSource.Task.AttachExternalCancellation(localCts.Token);
                if (isFadeIn == false)
                {
                    Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
                }
            }
            catch (OperationCanceledException)
            {
                if (ReferenceEquals(_fadeCts, localCts))
                {
                    fadeView.CancelFade();
                }

                throw;
            }
            finally
            {
                if (ReferenceEquals(_fadeCts, localCts))
                {
                    _fadeCts = null;
                    localCts.Dispose();
                }
            }
        }

        /// <summary>
        /// 이벤트 기반 페이드 완료 발행
        /// </summary>
        private async UniTaskVoid PlayFadeEventAsync(bool isFadeIn)
        {
            try
            {
                await FadeAsync(isFadeIn, CancellationToken.None);
                Bus<OnFadeCompletedEvent>.Raise(new OnFadeCompletedEvent(isFadeIn));
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 페이드 연출 정리
        /// </summary>
        private void OnDestroy()
        {
            CancelFadeRequest();
        }

        /// <summary>
        /// 페이드 요청 취소
        /// </summary>
        private void CancelFadeRequest()
        {
            if (_fadeCts != null)
            {
                _fadeCts.Cancel();
                _fadeCts.Dispose();
                _fadeCts = null;
            }

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
