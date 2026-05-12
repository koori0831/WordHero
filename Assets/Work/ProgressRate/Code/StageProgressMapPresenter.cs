using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.ProgressRate.Code
{
    /// <summary>
    /// 스테이지 진행도 맵 표시 제어자
    /// </summary>
    public class StageProgressMapPresenter : MonoBehaviour
    {
        [SerializeField] private StageProgressMapView progressMapView;
        [SerializeField] private StageFlowConfigSO stageFlowConfig;
        [SerializeField] private DoorType initialRoomType = DoorType.Wood;

        private readonly List<DoorType> _roomHistory = new List<DoorType>();
        private int _currentStageIndex = -1;
        private CancellationTokenSource _cts;

        /// <summary>
        /// 초기 구독 처리
        /// </summary>
        private void Awake()
        {
            ResolveView();
            ResetState(GetInitialRoomType());
            BusObservable.On<ResetStageProgressEvent>()
                .Subscribe(HandleResetEvent)
                .AddTo(this);
            BusObservable.On<SetInitialStageProgressEvent>()
                .Subscribe(HandleSetInitialEvent)
                .AddTo(this);
        }

        /// <summary>
        /// 연출 정리 처리
        /// </summary>
        private void OnDestroy()
        {
            CancelProcess();
        }

        /// <summary>
        /// 최초 진행도 맵 연출 대기 처리
        /// </summary>
        public async UniTask PlayInitialAsync(DoorType roomType, CancellationToken cancellationToken)
        {
            ResolveView();
            ResetState(roomType);
            _currentStageIndex = 0;
            CancelProcess();

            if (progressMapView == null)
            {
                return;
            }

            CancellationTokenSource localCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _cts = localCts;
            try
            {
                await progressMapView.PlayInitialAsync(_roomHistory, GetTotalStageCount(), localCts.Token);
            }
            finally
            {
                DisposeIfCurrent(localCts);
            }
        }

        /// <summary>
        /// 다음 진행도 맵 연출 대기 처리
        /// </summary>
        public async UniTask PlayNextAsync(DoorType nextRoomType, CancellationToken cancellationToken)
        {
            ResolveView();
            if (_currentStageIndex < 0)
            {
                _currentStageIndex = 0;
            }

            _roomHistory.Add(nextRoomType);
            _currentStageIndex++;
            CancelProcess();
            int totalStageCount = GetTotalStageCount();

            if (progressMapView == null || _currentStageIndex >= totalStageCount)
            {
                return;
            }

            CancellationTokenSource localCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _cts = localCts;
            try
            {
                await progressMapView.PlayNextAsync(_roomHistory, _currentStageIndex, totalStageCount, localCts.Token);
            }
            finally
            {
                DisposeIfCurrent(localCts);
            }
        }

        /// <summary>
        /// 초기 방 타입 설정 이벤트 처리
        /// </summary>
        private void HandleSetInitialEvent(SetInitialStageProgressEvent evt)
        {
            initialRoomType = evt.InitialRoomType;
            if (_currentStageIndex == -1)
            {
                ResetState(GetInitialRoomType());
            }
        }

        /// <summary>
        /// 진행도 맵 리셋 이벤트 처리
        /// </summary>
        private void HandleResetEvent(ResetStageProgressEvent evt)
        {
            initialRoomType = evt.InitialRoomType;
            ResetState(initialRoomType);
            CancelProcess();
            if (progressMapView != null)
            {
                progressMapView.Hide();
            }
        }

        /// <summary>
        /// 진행도 상태 초기화
        /// </summary>
        private void ResetState(DoorType roomType)
        {
            _currentStageIndex = -1;
            _roomHistory.Clear();
            _roomHistory.Add(roomType);
        }

        /// <summary>
        /// 최초 방 타입
        /// </summary>
        private DoorType GetInitialRoomType()
        {
            return stageFlowConfig != null ? stageFlowConfig.InitialDoorType : initialRoomType;
        }

        /// <summary>
        /// 전체 스테이지 수
        /// </summary>
        private int GetTotalStageCount()
        {
            return stageFlowConfig != null ? stageFlowConfig.TotalStageCount : 1;
        }

        /// <summary>
        /// 뷰 참조 확인
        /// </summary>
        private void ResolveView()
        {
            if (progressMapView != null)
            {
                return;
            }

            progressMapView = GetComponent<StageProgressMapView>();
            if (progressMapView == null)
            {
                progressMapView = FindFirstObjectByType<StageProgressMapView>();
            }
        }

        /// <summary>
        /// 진행도 맵 연출 취소
        /// </summary>
        private void CancelProcess()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            if (progressMapView != null)
            {
                progressMapView.CancelProcess();
            }
        }

        /// <summary>
        /// 현재 취소 토큰 정리
        /// </summary>
        private void DisposeIfCurrent(CancellationTokenSource cancellationTokenSource)
        {
            if (ReferenceEquals(_cts, cancellationTokenSource) == false)
            {
                return;
            }

            _cts = null;
            cancellationTokenSource.Dispose();
        }
    }
}
