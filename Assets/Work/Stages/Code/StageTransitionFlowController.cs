using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.ETC.LocationUI.Code;
using Work.Fade;
using Work.ProgressRate.Code;

namespace Work.Stages.Code
{
    /// <summary>
    /// 스테이지 전환 흐름 제어자
    /// </summary>
    public class StageTransitionFlowController
    {
        private readonly Action<DoorType, GameObject> _generateStage;
        private readonly float _progressMapFallbackDelay;

        private DoorType _pendingDoorType;
        private GameObject _pendingInteractor;
        private CancellationTokenSource _progressMapFallbackCts;

        private bool _isTransitioning;
        private bool _isInitialFlow;
        private bool _isWaitingFadeIn;
        private bool _isWaitingProgressMap;
        private bool _isWaitingFadeOut;
        private bool _isDisposed;

        /// <summary>
        /// 스테이지 전환 흐름 제어자 생성
        /// </summary>
        public StageTransitionFlowController(Action<DoorType, GameObject> generateStage, float progressMapFallbackDelay)
        {
            _generateStage = generateStage;
            _progressMapFallbackDelay = progressMapFallbackDelay;
        }

        /// <summary>
        /// 이벤트 구독 초기화
        /// </summary>
        public void Initialize()
        {
            Bus<OnFadeCompletedEvent>.Events += HandleFadeCompletedEvent;
            Bus<StageProgressMapClosedEvent>.Events += HandleStageProgressMapClosedEvent;
        }

        /// <summary>
        /// 이벤트 구독 해제
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            CancelProgressMapFallback();
            Bus<OnFadeCompletedEvent>.Events -= HandleFadeCompletedEvent;
            Bus<StageProgressMapClosedEvent>.Events -= HandleStageProgressMapClosedEvent;
        }

        /// <summary>
        /// 최초 스테이지 전환 시작
        /// </summary>
        public void StartInitialFlow(DoorType initialDoorType)
        {
            if (_isTransitioning)
            {
                return;
            }

            _isTransitioning = true;
            _isInitialFlow = true;
            _pendingDoorType = initialDoorType;
            _pendingInteractor = null;

            _generateStage.Invoke(_pendingDoorType, _pendingInteractor);
            BeginProgressMapWait();
            Bus<PlayInitialProgressMapEvent>.Raise(new PlayInitialProgressMapEvent());
        }

        /// <summary>
        /// 다음 스테이지 전환 시작
        /// </summary>
        public void RequestTransition(GameObject interactor, DoorType doorType)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("[StageTransitionFlowController] 스테이지 전환이 이미 진행 중입니다.");
                return;
            }

            _isTransitioning = true;
            _isInitialFlow = false;
            _isWaitingFadeIn = true;
            _pendingInteractor = interactor;
            _pendingDoorType = doorType;

            Bus<OnFadeEvent>.Raise(new OnFadeEvent(true));
        }

        /// <summary>
        /// 페이드 완료 이벤트 처리
        /// </summary>
        private void HandleFadeCompletedEvent(OnFadeCompletedEvent evt)
        {
            if (_isDisposed)
            {
                return;
            }

            if (evt.isFadeIn && _isWaitingFadeIn)
            {
                _isWaitingFadeIn = false;
                _generateStage.Invoke(_pendingDoorType, _pendingInteractor);
                BeginProgressMapWait();
                Bus<OnNextRoomEvent>.Raise(new OnNextRoomEvent(_pendingDoorType));
                return;
            }

            if (evt.isFadeIn == false && _isWaitingFadeOut)
            {
                _isWaitingFadeOut = false;
                CompleteTransition();
            }
        }

        /// <summary>
        /// 진행도 맵 닫힘 이벤트 처리
        /// </summary>
        private void HandleStageProgressMapClosedEvent(StageProgressMapClosedEvent evt)
        {
            CompleteProgressMapWait();
        }

        /// <summary>
        /// 진행도 맵 대기 시작
        /// </summary>
        private void BeginProgressMapWait()
        {
            _isWaitingProgressMap = true;
            CancelProgressMapFallback();

            _progressMapFallbackCts = new CancellationTokenSource();
            WaitProgressMapFallbackAsync(_progressMapFallbackCts.Token).Forget();
        }

        /// <summary>
        /// 진행도 맵 대기 완료
        /// </summary>
        private void CompleteProgressMapWait()
        {
            if (_isDisposed || _isWaitingProgressMap == false)
            {
                return;
            }

            _isWaitingProgressMap = false;
            CancelProgressMapFallback();

            if (_isInitialFlow)
            {
                CompleteTransition();
                return;
            }

            _isWaitingFadeOut = true;
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(false));
        }

        /// <summary>
        /// 진행도 맵 대기 안전장치
        /// </summary>
        private async UniTaskVoid WaitProgressMapFallbackAsync(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_progressMapFallbackDelay), cancellationToken: cancellationToken);
                CompleteProgressMapWait();
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 진행도 맵 대기 안전장치 취소
        /// </summary>
        private void CancelProgressMapFallback()
        {
            if (_progressMapFallbackCts == null)
            {
                return;
            }

            _progressMapFallbackCts.Cancel();
            _progressMapFallbackCts.Dispose();
            _progressMapFallbackCts = null;
        }

        /// <summary>
        /// 스테이지 전환 완료 처리
        /// </summary>
        private void CompleteTransition()
        {
            _isTransitioning = false;
            _isInitialFlow = false;
            _isWaitingFadeIn = false;
            _isWaitingProgressMap = false;
            _isWaitingFadeOut = false;
            _pendingInteractor = null;

            Bus<PlayLocationUIEvent>.Raise(new PlayLocationUIEvent());
        }
    }
}
