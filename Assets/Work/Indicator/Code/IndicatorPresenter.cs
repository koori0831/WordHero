using System.Collections.Generic;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Serialization;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 적 방향 인디케이터 표시 제어자
    /// </summary>
    public class IndicatorPresenter : MonoBehaviour
    {
        [FormerlySerializedAs("_indicatorPoolItem")]
        [SerializeField] private PoolItemSO indicatorPoolItem;
        [FormerlySerializedAs("_margin")]
        [SerializeField] private float margin = 50f;
        [FormerlySerializedAs("_minSpacing")]
        [SerializeField] private float minSpacing = 40f;
        [SerializeField] private IndicatorContainerView indicatorContainerView;

        [Inject] private PoolManagerMono _poolManager;

        private List<Enemy> _monitoredEnemyList;
        private HashSet<Enemy> _enemyLookupSet = new HashSet<Enemy>();
        private Dictionary<Enemy, EnemyIndicatorView> _indicatorMap = new Dictionary<Enemy, EnemyIndicatorView>();
        private Dictionary<EnemyIndicatorView, Enemy> _indicatorTargetMap = new Dictionary<EnemyIndicatorView, Enemy>();
        private List<EnemyIndicatorView> _activeIndicators = new List<EnemyIndicatorView>();
        private List<EnemyIndicatorView> _topBucket = new List<EnemyIndicatorView>();
        private List<EnemyIndicatorView> _bottomBucket = new List<EnemyIndicatorView>();
        private List<EnemyIndicatorView> _leftBucket = new List<EnemyIndicatorView>();
        private List<EnemyIndicatorView> _rightBucket = new List<EnemyIndicatorView>();
        private IndicatorViewStateCalculator _viewStateCalculator = new IndicatorViewStateCalculator();

        private Camera _cachedMainCamera;
        private bool _isContainerWarningLogged;

        /// <summary>
        /// 초기 구독 처리
        /// </summary>
        private void Start()
        {
            _cachedMainCamera = Camera.main;
            ResolveContainerView();
            Bus<OnEnemySpawnedEvent>.Events += HandleEnemyListReceived;
        }

        /// <summary>
        /// 이벤트 구독 해제 처리
        /// </summary>
        private void OnDestroy()
        {
            Bus<OnEnemySpawnedEvent>.Events -= HandleEnemyListReceived;
        }

        /// <summary>
        /// 적 목록 수신 처리
        /// </summary>
        private void HandleEnemyListReceived(OnEnemySpawnedEvent evt)
        {
            _monitoredEnemyList = evt.enemyList;
        }

        /// <summary>
        /// 카메라 캐시 확인
        /// </summary>
        private void CheckCamera()
        {
            if (_cachedMainCamera == null)
            {
                _cachedMainCamera = Camera.main;
            }
        }

        /// <summary>
        /// 인디케이터 표시 갱신 처리
        /// </summary>
        private void Update()
        {
            CheckCamera();
            ResolveContainerView();

            if (_cachedMainCamera == null || indicatorContainerView == null || _monitoredEnemyList == null)
            {
                return;
            }

            SynchronizeEnemyLookupSet();
            SynchronizeIndicators();
            ResolveOverlapsFast();
            ApplyResolvedRotations();
        }

        /// <summary>
        /// 컨테이너 뷰 확인
        /// </summary>
        private void ResolveContainerView()
        {
            if (indicatorContainerView != null)
            {
                return;
            }

            indicatorContainerView = FindFirstObjectByType<IndicatorContainerView>();
            if (indicatorContainerView == null && _isContainerWarningLogged == false)
            {
                _isContainerWarningLogged = true;
                Debug.LogWarning("[IndicatorPresenter] IndicatorContainerView를 찾을 수 없습니다.", this);
            }
        }

        /// <summary>
        /// 적 조회 목록 동기화
        /// </summary>
        private void SynchronizeEnemyLookupSet()
        {
            _enemyLookupSet.Clear();

            try
            {
                int enemyCount = _monitoredEnemyList.Count;
                for (int i = 0; i < enemyCount; i++)
                {
                    Enemy enemy = _monitoredEnemyList[i];
                    if (enemy != null)
                    {
                        _enemyLookupSet.Add(enemy);
                    }
                }
            }
            catch (System.Exception)
            {
                _enemyLookupSet.Clear();
            }
        }

        /// <summary>
        /// 인디케이터 생성 및 제거 동기화
        /// </summary>
        private void SynchronizeIndicators()
        {
            foreach (Enemy enemy in _enemyLookupSet)
            {
                if (_indicatorMap.ContainsKey(enemy) == false)
                {
                    AddIndicator(enemy);
                }
            }

            for (int i = _activeIndicators.Count - 1; i >= 0; i--)
            {
                EnemyIndicatorView indicatorView = _activeIndicators[i];
                if (indicatorView == null)
                {
                    _activeIndicators.RemoveAt(i);
                    continue;
                }

                Enemy target = _indicatorTargetMap.TryGetValue(indicatorView, out Enemy targetEnemy) ? targetEnemy : null;
                IndicatorTargetModel targetModel = new IndicatorTargetModel(target);

                if (targetModel.IsValid == false || _enemyLookupSet.Contains(target) == false)
                {
                    RemoveIndicatorAtIndex(i, target);
                    continue;
                }

                IndicatorViewState viewState = _viewStateCalculator.Calculate(targetModel, _cachedMainCamera, indicatorContainerView.ContainerRect, margin);
                indicatorView.ApplyState(viewState);
            }
        }

        /// <summary>
        /// 인디케이터 중첩 보정
        /// </summary>
        private void ResolveOverlapsFast()
        {
            _topBucket.Clear();
            _bottomBucket.Clear();
            _leftBucket.Clear();
            _rightBucket.Clear();

            Rect rect = indicatorContainerView.ContainerRect;
            float halfWidth = rect.width * 0.5f;
            float halfHeight = rect.height * 0.5f;
            float limitX = Mathf.Max(0f, halfWidth - margin);
            float limitY = Mathf.Max(0f, halfHeight - margin);

            int activeCount = _activeIndicators.Count;
            for (int i = 0; i < activeCount; i++)
            {
                EnemyIndicatorView indicatorView = _activeIndicators[i];
                if (indicatorView == null || indicatorView.IsVisible == false)
                {
                    continue;
                }

                Vector2 position = indicatorView.AnchoredPosition;
                if (position.y >= limitY - 2f)
                {
                    _topBucket.Add(indicatorView);
                }
                else if (position.y <= -limitY + 2f)
                {
                    _bottomBucket.Add(indicatorView);
                }
                else if (position.x <= -limitX + 2f)
                {
                    _leftBucket.Add(indicatorView);
                }
                else if (position.x >= limitX - 2f)
                {
                    _rightBucket.Add(indicatorView);
                }
            }

            ResolveSingleBucket(_topBucket, true, limitX, limitY);
            ResolveSingleBucket(_bottomBucket, true, limitX, limitY);
            ResolveSingleBucket(_leftBucket, false, limitX, limitY);
            ResolveSingleBucket(_rightBucket, false, limitX, limitY);
        }

        /// <summary>
        /// 단일 가장자리 중첩 보정
        /// </summary>
        private void ResolveSingleBucket(List<EnemyIndicatorView> bucket, bool isHorizontal, float limitX, float limitY)
        {
            if (bucket.Count < 2)
            {
                return;
            }

            if (isHorizontal)
            {
                bucket.Sort((EnemyIndicatorView a, EnemyIndicatorView b) => a.AnchoredPosition.x.CompareTo(b.AnchoredPosition.x));
            }
            else
            {
                bucket.Sort((EnemyIndicatorView a, EnemyIndicatorView b) => a.AnchoredPosition.y.CompareTo(b.AnchoredPosition.y));
            }

            for (int iteration = 0; iteration < 2; iteration++)
            {
                for (int i = 0; i < bucket.Count - 1; i++)
                {
                    EnemyIndicatorView previous = bucket[i];
                    EnemyIndicatorView next = bucket[i + 1];
                    Vector2 previousPosition = previous.AnchoredPosition;
                    Vector2 nextPosition = next.AnchoredPosition;
                    float diff = isHorizontal ? nextPosition.x - previousPosition.x : nextPosition.y - previousPosition.y;

                    if (diff >= minSpacing)
                    {
                        continue;
                    }

                    float push = (minSpacing - diff) * 0.5f;
                    if (isHorizontal)
                    {
                        previous.AnchoredPosition -= new Vector2(push, 0f);
                        next.AnchoredPosition += new Vector2(push, 0f);
                        previous.AnchoredPosition = new Vector2(Mathf.Clamp(previous.AnchoredPosition.x, -limitX, limitX), previous.AnchoredPosition.y);
                        next.AnchoredPosition = new Vector2(Mathf.Clamp(next.AnchoredPosition.x, -limitX, limitX), next.AnchoredPosition.y);
                    }
                    else
                    {
                        previous.AnchoredPosition -= new Vector2(0f, push);
                        next.AnchoredPosition += new Vector2(0f, push);
                        previous.AnchoredPosition = new Vector2(previous.AnchoredPosition.x, Mathf.Clamp(previous.AnchoredPosition.y, -limitY, limitY));
                        next.AnchoredPosition = new Vector2(next.AnchoredPosition.x, Mathf.Clamp(next.AnchoredPosition.y, -limitY, limitY));
                    }
                }
            }
        }

        /// <summary>
        /// 중첩 보정 후 회전 반영
        /// </summary>
        private void ApplyResolvedRotations()
        {
            for (int i = 0; i < _activeIndicators.Count; i++)
            {
                EnemyIndicatorView indicatorView = _activeIndicators[i];
                if (indicatorView != null && indicatorView.IsVisible)
                {
                    indicatorView.ApplyRotation();
                }
            }
        }

        /// <summary>
        /// 인디케이터 추가 처리
        /// </summary>
        private void AddIndicator(Enemy enemy)
        {
            if (_poolManager == null || indicatorPoolItem == null || indicatorContainerView == null)
            {
                return;
            }

            EnemyIndicatorView indicatorView = _poolManager.Pop<EnemyIndicatorView>(indicatorPoolItem);
            if (indicatorView == null)
            {
                return;
            }

            indicatorContainerView.Attach(indicatorView.transform);
            _indicatorMap[enemy] = indicatorView;
            _indicatorTargetMap[indicatorView] = enemy;
            _activeIndicators.Add(indicatorView);
        }

        /// <summary>
        /// 인디케이터 제거 처리
        /// </summary>
        private void RemoveIndicatorAtIndex(int index, Enemy target)
        {
            EnemyIndicatorView indicatorView = _activeIndicators[index];
            if (target != null)
            {
                _indicatorMap.Remove(target);
            }

            if (indicatorView != null)
            {
                if (_indicatorTargetMap.TryGetValue(indicatorView, out Enemy mappedTarget))
                {
                    _indicatorMap.Remove(mappedTarget);
                }

                _indicatorTargetMap.Remove(indicatorView);
                _poolManager.Push(indicatorView);
            }

            _activeIndicators.RemoveAt(index);
        }
    }
}
