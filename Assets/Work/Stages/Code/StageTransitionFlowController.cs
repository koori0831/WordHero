using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.ETC.LocationUI.Code;
using Work.Fade;
using Work.Input.Code;
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
        private readonly FadePresenter _fadePresenter;
        private readonly StageProgressMap _stageProgressMap;

        private CancellationTokenSource _transitionCts;
        private bool _isTransitioning;
        private bool _isDisposed;

        /// <summary>
        /// 스테이지 전환 흐름 제어자 생성
        /// </summary>
        public StageTransitionFlowController(Action<DoorType, GameObject> generateStage, float progressMapFallbackDelay, FadePresenter fadePresenter, StageProgressMap stageProgressMap)
        {
            _generateStage = generateStage;
            _progressMapFallbackDelay = progressMapFallbackDelay;
            _fadePresenter = fadePresenter;
            _stageProgressMap = stageProgressMap;
        }

        /// <summary>
        /// 이벤트 구독 초기화
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// 전환 흐름 해제
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            CancelTransition();
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

            _transitionCts = new CancellationTokenSource();
            RunInitialFlowAsync(initialDoorType, _transitionCts.Token).Forget();
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

            _transitionCts = new CancellationTokenSource();
            RunTransitionAsync(interactor, doorType, _transitionCts.Token).Forget();
        }

        /// <summary>
        /// 최초 전환 흐름
        /// </summary>
        private async UniTaskVoid RunInitialFlowAsync(DoorType initialDoorType, CancellationToken cancellationToken)
        {
            _isTransitioning = true;

            try
            {
                _generateStage.Invoke(initialDoorType, null);
                await PlayInitialProgressMapAsync(initialDoorType, cancellationToken);
                CompleteTransition();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                CompleteTransition();
            }
        }

        /// <summary>
        /// 다음 전환 흐름
        /// </summary>
        private async UniTaskVoid RunTransitionAsync(GameObject interactor, DoorType doorType, CancellationToken cancellationToken)
        {
            _isTransitioning = true;
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

            try
            {
                await PlayFadeAsync(true, cancellationToken);
                _generateStage.Invoke(doorType, interactor);
                Bus<OnNextRoomEvent>.Raise(new OnNextRoomEvent(doorType));
                await PlayNextProgressMapAsync(doorType, cancellationToken);
                await PlayFadeAsync(false, cancellationToken);
                CompleteTransition();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                CompleteTransition();
            }
            finally
            {
                if (_isDisposed == false)
                {
                    Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
                }
            }
        }

        /// <summary>
        /// 선택적 페이드 대기
        /// </summary>
        private UniTask PlayFadeAsync(bool isFadeIn, CancellationToken cancellationToken)
        {
            if (_fadePresenter == null)
            {
                return UniTask.CompletedTask;
            }

            return _fadePresenter.FadeAsync(isFadeIn, cancellationToken);
        }

        /// <summary>
        /// 선택적 최초 진행도 맵 대기
        /// </summary>
        private UniTask PlayInitialProgressMapAsync(DoorType initialDoorType, CancellationToken cancellationToken)
        {
            if (_stageProgressMap == null)
            {
                return UniTask.CompletedTask;
            }

            return PlayProgressMapWithFallbackAsync(_stageProgressMap.PlayInitialAsync(initialDoorType, cancellationToken), cancellationToken);
        }

        /// <summary>
        /// 선택적 다음 진행도 맵 대기
        /// </summary>
        private UniTask PlayNextProgressMapAsync(DoorType doorType, CancellationToken cancellationToken)
        {
            if (_stageProgressMap == null)
            {
                return UniTask.CompletedTask;
            }

            return PlayProgressMapWithFallbackAsync(_stageProgressMap.PlayNextAsync(doorType, cancellationToken), cancellationToken);
        }

        /// <summary>
        /// 진행도 맵 안전 대기
        /// </summary>
        private async UniTask PlayProgressMapWithFallbackAsync(UniTask progressMapTask, CancellationToken cancellationToken)
        {
            if (_progressMapFallbackDelay <= 0f)
            {
                await progressMapTask;
                return;
            }

            UniTask fallbackTask = UniTask.Delay(TimeSpan.FromSeconds(_progressMapFallbackDelay), cancellationToken: cancellationToken);
            int completedIndex = await UniTask.WhenAny(progressMapTask, fallbackTask);
            if (completedIndex == 1)
            {
                Debug.LogWarning("[StageTransitionFlowController] 진행도 맵 연출 대기 시간이 초과되어 전환을 계속합니다.");
            }
        }

        /// <summary>
        /// 전환 취소
        /// </summary>
        private void CancelTransition()
        {
            if (_transitionCts == null)
            {
                return;
            }

            _transitionCts.Cancel();
            _transitionCts.Dispose();
            _transitionCts = null;
        }

        /// <summary>
        /// 스테이지 전환 완료 처리
        /// </summary>
        private void CompleteTransition()
        {
            _isTransitioning = false;
            CancelTransition();

            if (_isDisposed)
            {
                return;
            }

            Bus<PlayLocationUIEvent>.Raise(new PlayLocationUIEvent());
        }
    }
}
