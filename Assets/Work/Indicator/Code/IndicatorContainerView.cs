using UnityEngine;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 인디케이터 캔버스 컨테이너 뷰
    /// </summary>
    public class IndicatorContainerView : MonoBehaviour
    {
        [SerializeField] private RectTransform indicatorContainer;

        private RectTransform _cachedRectTransform;

        /// <summary>
        /// 인디케이터 부모 트랜스폼
        /// </summary>
        public RectTransform IndicatorContainer => GetIndicatorContainer();

        /// <summary>
        /// 인디케이터 표시 영역
        /// </summary>
        public Rect ContainerRect => GetIndicatorContainer().rect;

        /// <summary>
        /// 컴포넌트 참조 초기화
        /// </summary>
        private void Awake()
        {
            CacheRectTransform();
        }

        /// <summary>
        /// 에디터 참조 자동 설정
        /// </summary>
        private void Reset()
        {
            CacheRectTransform();
            indicatorContainer = _cachedRectTransform;
        }

        /// <summary>
        /// 인디케이터 부모 지정
        /// </summary>
        public void Attach(Transform indicatorTransform)
        {
            indicatorTransform.SetParent(GetIndicatorContainer(), false);
        }

        /// <summary>
        /// 컨테이너 참조 반환
        /// </summary>
        private RectTransform GetIndicatorContainer()
        {
            if (indicatorContainer == null)
            {
                CacheRectTransform();
                indicatorContainer = _cachedRectTransform;
            }

            return indicatorContainer;
        }

        /// <summary>
        /// RectTransform 캐시 처리
        /// </summary>
        private void CacheRectTransform()
        {
            if (_cachedRectTransform == null)
            {
                _cachedRectTransform = GetComponent<RectTransform>();
            }
        }
    }
}
