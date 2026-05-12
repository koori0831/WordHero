using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.ETC.LocationUI.Code
{
    /// <summary>
    /// 로케이션 이름 표시 제어자
    /// </summary>
    public class LocationNamePresenter : MonoBehaviour
    {
        [SerializeField] private LocationNameView locationNameView;

        private CancellationTokenSource _cts;

        /// <summary>
        /// 표시 이벤트 구독 처리
        /// </summary>
        private void Awake()
        {
            ResolveView();
            BusObservable.On<OnShowLocationNameEvent>()
                .Subscribe(HandleShowLocationNameEvent)
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
        /// 로케이션 이름 표시 이벤트 처리
        /// </summary>
        private void HandleShowLocationNameEvent(OnShowLocationNameEvent evt)
        {
            PlayLocationNameAsync(evt.LocationName).Forget();
        }

        /// <summary>
        /// 로케이션 이름 표시 대기 처리
        /// </summary>
        private async UniTaskVoid PlayLocationNameAsync(string locationName)
        {
            ResolveView();
            CancelProcess();

            if (locationNameView == null)
            {
                return;
            }

            CancellationTokenSource localCts = new CancellationTokenSource();
            _cts = localCts;

            try
            {
                await locationNameView.PlayAsync(locationName, localCts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                DisposeIfCurrent(localCts);
            }
        }

        /// <summary>
        /// 뷰 참조 확인
        /// </summary>
        private void ResolveView()
        {
            if (locationNameView != null)
            {
                return;
            }

            locationNameView = FindFirstObjectByType<LocationNameView>();
        }

        /// <summary>
        /// 표시 연출 취소
        /// </summary>
        private void CancelProcess()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            if (locationNameView != null)
            {
                locationNameView.CancelMotions();
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
