using R3;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.ETC.LocationUI.Code
{
    /// <summary>
    /// 로케이션 이름 표시 요청 트리거
    /// </summary>
    public class LocationUITrigger : MonoBehaviour
    {
        [SerializeField] private string locationName;

        private bool _isPlayed;

        /// <summary>
        /// 표시 요청 구독 처리
        /// </summary>
        private void Awake()
        {
            BusObservable.On<PlayLocationUIEvent>()
                .Subscribe(HandlePlayLocationUIEvent)
                .AddTo(this);
        }

        /// <summary>
        /// 로케이션 표시 요청 처리
        /// </summary>
        private void HandlePlayLocationUIEvent(PlayLocationUIEvent evt)
        {
            if (_isPlayed)
            {
                return;
            }

            _isPlayed = true;
            Bus<OnShowLocationNameEvent>.Raise(new OnShowLocationNameEvent(locationName));
            Destroy(gameObject);
        }
    }
}
