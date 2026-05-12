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
        [SerializeField] private float _rotationSpeed = 15f; 
        
        public PoolItemSO PoolItem => _poolItem;
        public GameObject GameObject => gameObject;

        private Enemy _targetEnemy;
        public Enemy TargetEnemy => _targetEnemy;

        private RectTransform _rectTransform;
        private float _margin = 50f;
        private Vector2 _currentDir; // 계산된 화면상 방향 벡터 저장
        private bool _isFirstFrame = true;

        /// <summary>
        /// 인디케이터의 위치를 외부(Manager)에서 제어하기 위한 프로퍼티
        /// </summary>
        public Vector2 AnchoredPosition
        {
            get => _rectTransform.anchoredPosition;
            set => _rectTransform.anchoredPosition = value;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetTarget(Enemy enemy, float margin)
        {
            _targetEnemy = enemy;
            _margin = margin;
            _isFirstFrame = true;
        }

        /// <summary>
        /// 적의 위치에 따라 인디케이터의 위치와 회전값을 업데이트.
        /// </summary>
        public void UpdateIndicator(Camera mainCamera, RectTransform container)
        {
            if (_targetEnemy == null || _targetEnemy.IsDead)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
                return;
            }

            // 1. 화면 내 존재 여부 판정
            Vector3 screenPos = mainCamera.WorldToScreenPoint(_targetEnemy.transform.position);
            
            // 화면 안에 있고 카메라 앞쪽(Z > 0)에 있는 경우에만 '화면 안'으로 판정
            bool isInside = screenPos.z > 0 && 
                            screenPos.x > _margin && screenPos.x < Screen.width - _margin && 
                            screenPos.y > _margin && screenPos.y < Screen.height - _margin;

            if (!isInside)
            {
                if (gameObject.activeSelf == false)
                {
                    gameObject.SetActive(true);
                    _isFirstFrame = true;
                }
                // 2. 나침반 방식의 위치 계산 - 점프 및 다이나믹 현상 해결
                UpdatePositionByCompass(mainCamera, container.rect);
            }
            else
            {
                if (gameObject.activeSelf == true) gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 지면을 기준으로 한 360도 방위각을 계산하여 화면 가장자리에 투영함
        /// </summary>
        private void UpdatePositionByCompass(Camera mainCamera, Rect containerRect)
        {
            //카메라의 시야 방향을 지면에 평행하게 투영 (기준 축 수립)
            Vector3 camForward = mainCamera.transform.forward;
            camForward.y = 0;
            camForward.Normalize();
            
            Vector3 camRight = mainCamera.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            //화면 중앙이 가리키는 지면 지점(중심점) 획득
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Plane groundPlane = new Plane(Vector3.up, _targetEnemy.transform.position);
            Vector3 centerPos;
            if (groundPlane.Raycast(ray, out float enter))
            {
                centerPos = ray.GetPoint(enter);
            }
            else
            {
                centerPos = mainCamera.transform.position + camForward * 10f;
            }

            //중심점에서 적을 향하는 평면 방향 벡터
            Vector3 diff = _targetEnemy.transform.position - centerPos;
            
            //평면 벡터를 카메라 축(화면의 상하좌우)으로 변환
            // xDir: 화면의 좌우, yDir: 화면의 상하
            float xDir = Vector3.Dot(diff.normalized, camRight);
            float yDir = Vector3.Dot(diff.normalized, camForward);

            _currentDir = new Vector2(xDir, yDir);
            if (_currentDir.sqrMagnitude < 0.001f)
            {
                _currentDir = Vector2.up;
            }
            else
            {
                _currentDir.Normalize();
            }

            //UI 좌표계에 맞춰 가장자리 교점 계산
            float halfW = containerRect.width * 0.5f - _margin;
            float halfH = containerRect.height * 0.5f - _margin;

            float m = Mathf.Approximately(_currentDir.x, 0) ? _currentDir.y * 1000000f : _currentDir.y / _currentDir.x;
            float screenRatio = halfH / halfW;

            Vector2 offset = Vector2.zero;
            if (Mathf.Abs(m) <= screenRatio)
            {
                float x = _currentDir.x > 0 ? halfW : -halfW;
                offset = new Vector2(x, m * x);
            }
            else
            {
                float y = _currentDir.y > 0 ? halfH : -halfH;
                offset = new Vector2(y / m, y);
            }

            if (!float.IsNaN(offset.x) && !float.IsNaN(offset.y))
            {
                _rectTransform.anchoredPosition = offset;
            }
            
            if (_isFirstFrame)
            {
                FixRotation(true);
                _isFirstFrame = false;
            }
        }

        public void FixRotation(bool immediate = false)
        {
            if (_targetEnemy == null) return;

            // 계산된 평위각 방향을 그대로 활용하여 화살표 각도 설정
            float angle = Mathf.Atan2(_currentDir.y, _currentDir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            if (immediate)
            {
                _rectTransform.localRotation = targetRotation;
            }
            else
            {
                _rectTransform.localRotation = Quaternion.Slerp(
                    _rectTransform.localRotation, 
                    targetRotation, 
                    Time.deltaTime * _rotationSpeed
                );
            }
        }

        public void ResetItem()
        {
            _targetEnemy = null;
            gameObject.SetActive(false);
            _isFirstFrame = true;
        }

        public void SetUpPool(Pool pool) { }
    }
}
