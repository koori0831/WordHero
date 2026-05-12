using R3;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.AcquireItem.Code
{
    /// <summary>
    /// 획득 아이템 UI 표시 제어자
    /// </summary>
    public class AcquireItemPresenter : MonoBehaviour
    {
        [SerializeField] private AcquireItemFieldView fieldView;

        /// <summary>
        /// 획득 이벤트 구독 처리
        /// </summary>
        private void Awake()
        {
            ResolveView();
            BusObservable.On<OnGetItemEvent>()
                .Subscribe(HandleGetItemEvent)
                .AddTo(this);
        }

        /// <summary>
        /// 획득 이벤트 처리
        /// </summary>
        private void HandleGetItemEvent(OnGetItemEvent evt)
        {
            ResolveView();
            if (fieldView == null)
            {
                return;
            }

            fieldView.AddSlot(evt.Name, evt.Type, evt.Color);
        }

        /// <summary>
        /// 뷰 참조 확인
        /// </summary>
        private void ResolveView()
        {
            if (fieldView != null)
            {
                return;
            }

            fieldView = GetComponent<AcquireItemFieldView>();
            if (fieldView == null)
            {
                fieldView = FindFirstObjectByType<AcquireItemFieldView>();
            }
        }
    }
}
