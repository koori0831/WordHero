using UnityEngine;
using GondrLib.ObjectPool.RunTime;
using UnityEngine.Serialization;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 적 방향 인디케이터 뷰
    /// </summary>
    public class EnemyIndicatorView : MonoBehaviour, IPoolable
    {
        [FormerlySerializedAs("_poolItem")]
        [SerializeField] private PoolItemSO poolItem;
        [FormerlySerializedAs("_rotationSpeed")]
        [SerializeField] private float rotationSpeed = 15f;
        
        /// <summary>
        /// 풀 아이템 설정
        /// </summary>
        public PoolItemSO PoolItem => poolItem;

        /// <summary>
        /// 풀 대상 게임 오브젝트
        /// </summary>
        public GameObject GameObject => gameObject;

        private RectTransform _rectTransform;
        private Quaternion _targetRotation;
        private bool _isFirstFrame = true;

        /// <summary>
        /// 인디케이터 표시 여부
        /// </summary>
        public bool IsVisible => gameObject.activeSelf;

        /// <summary>
        /// 인디케이터 UI 위치
        /// </summary>
        public Vector2 AnchoredPosition
        {
            get => _rectTransform.anchoredPosition;
            set => _rectTransform.anchoredPosition = value;
        }

        /// <summary>
        /// 컴포넌트 참조 초기화
        /// </summary>
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 표시 상태 반영
        /// </summary>
        public void ApplyState(IndicatorViewState viewState)
        {
            if (viewState.IsVisible == false)
            {
                gameObject.SetActive(false);
                _isFirstFrame = true;
                return;
            }

            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
                _isFirstFrame = true;
            }

            _rectTransform.anchoredPosition = viewState.AnchoredPosition;
            _targetRotation = viewState.Rotation;
        }

        /// <summary>
        /// 회전값 반영
        /// </summary>
        public void ApplyRotation()
        {
            if (_isFirstFrame)
            {
                _rectTransform.localRotation = _targetRotation;
                _isFirstFrame = false;
            }
            else
            {
                _rectTransform.localRotation = Quaternion.Slerp(
                    _rectTransform.localRotation, 
                    _targetRotation, 
                    Time.deltaTime * rotationSpeed
                );
            }
        }

        /// <summary>
        /// 풀 반환 시 상태 초기화
        /// </summary>
        public void ResetItem()
        {
            gameObject.SetActive(false);
            _isFirstFrame = true;
            _targetRotation = Quaternion.identity;
        }

        /// <summary>
        /// 풀 초기 설정
        /// </summary>
        public void SetUpPool(Pool pool) { }
    }
}
