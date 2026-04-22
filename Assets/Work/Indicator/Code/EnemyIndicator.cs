using UnityEngine;
using Work.Enemies.Code;
using GondrLib.ObjectPool.RunTime;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 화면 밖의 적을 추적하여 방향을 표시하는 인디케이터 UI 컴포넌트
    /// </summary>
    public class EnemyIndicator : MonoBehaviour, IPoolable
    {
        [SerializeField] private PoolItemSO _poolItem;
        public PoolItemSO PoolItem => _poolItem;
        public GameObject GameObject => gameObject;

        private Enemy _targetEnemy;
        public Enemy TargetEnemy => _targetEnemy;

        private RectTransform _rectTransform;
        private Camera _mainCamera;
        private float _margin = 50f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _mainCamera = Camera.main;
        }

        public void SetTarget(Enemy enemy, float margin)
        {
            _targetEnemy = enemy;
            _margin = margin;
        }

        /// <summary>
        /// 적의 위치에 따라 인디케이터의 위치와 회전값을 업데이트.
        /// </summary>
        public void UpdateIndicator()
        {
            if (_targetEnemy == null || _targetEnemy.IsDead)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
                return;
            }

            Vector3 screenPos = _mainCamera.WorldToScreenPoint(_targetEnemy.transform.position);

            // 화면 내에 있는지 확인 (Z가 음수면 카메라 뒤쪽)
            bool isOffScreen = screenPos.z < 0 || 
                               screenPos.x <= _margin || screenPos.x >= Screen.width - _margin || 
                               screenPos.y <= _margin || screenPos.y >= Screen.height - _margin;

            if (isOffScreen)
            {
                if (gameObject.activeSelf == false) gameObject.SetActive(true);
                UpdatePosition(screenPos);
            }
            else
            {
                if (gameObject.activeSelf == true) gameObject.SetActive(false);
            }
        }

        private void UpdatePosition(Vector3 screenPos)
        {
            // 카메라 뒤쪽인 경우 좌표 반전
            if (screenPos.z < 0)
            {
                screenPos *= -1f;
            }

            Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 dir = (Vector2)screenPos - screenCenter;

            float cos = dir.x / dir.magnitude;
            float sin = dir.y / dir.magnitude;
            
            float m = Mathf.Approximately(cos, 0) ? sin * 1000000f : sin / cos;
            float screenRatio = screenCenter.y / screenCenter.x;

            Vector2 offset = Vector2.zero;

            if (Mathf.Abs(m) <= screenRatio)
            {
                float x = cos > 0 ? screenCenter.x - _margin : -screenCenter.x + _margin;
                offset = new Vector2(x, m * x);
            }
            else
            {
                float y = sin > 0 ? screenCenter.y - _margin : -screenCenter.y + _margin;
                offset = new Vector2(y / m, y);
            }

            _rectTransform.anchoredPosition = offset;

            // 방향에 맞게 회전
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void ResetItem()
        {
            _targetEnemy = null;
            gameObject.SetActive(false);
        }

        public void SetUpPool(Pool pool) { }
    }
}
