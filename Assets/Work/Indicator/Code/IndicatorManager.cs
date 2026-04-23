using System.Collections.Generic;
using UnityEngine;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// EnemyManager의 리스트를 참조하여 모든 적 인디케이터를 동기화 관리하는 시스템 (최고 성능 최적화 버전)
    /// </summary>
    public class IndicatorManager : MonoBehaviour
    {
        [SerializeField] private PoolItemSO _indicatorPoolItem;
        [SerializeField] private float _margin = 50f;
        [SerializeField] private float _minSpacing = 40f; 
        [SerializeField] private RectTransform _indicatorContainer;

        [Inject] private PoolManagerMono _poolManager;

        private List<Enemy> _monitoredEnemyList;
        private readonly HashSet<Enemy> _enemyLookupSet = new HashSet<Enemy>(); 
        private readonly Dictionary<Enemy, EnemyIndicator> _indicatorMap = new Dictionary<Enemy, EnemyIndicator>();
        private readonly List<EnemyIndicator> _activeIndicators = new List<EnemyIndicator>();
        
        // 최적화: 버킷 리스트 재사용 (GC 할당 최소화)
        private readonly List<EnemyIndicator> _topBucket = new List<EnemyIndicator>();
        private readonly List<EnemyIndicator> _bottomBucket = new List<EnemyIndicator>();
        private readonly List<EnemyIndicator> _leftBucket = new List<EnemyIndicator>();
        private readonly List<EnemyIndicator> _rightBucket = new List<EnemyIndicator>();

        private Camera _cachedMainCamera;

        private void Start()
        {
            _cachedMainCamera = Camera.main;
            Bus<OnEnemySpawnedEvent>.Events += HandleEnemyListReceived;
        }

        private void OnDestroy()
        {
            Bus<OnEnemySpawnedEvent>.Events -= HandleEnemyListReceived;
        }

        private void HandleEnemyListReceived(OnEnemySpawnedEvent evt)
        {
            _monitoredEnemyList = evt.enemyList;
        }

        private void CheckCamera()
        {
            if (_cachedMainCamera == null)
            {
                _cachedMainCamera = Camera.main;
            }
        }

        private void Update()
        {
            CheckCamera();
            if (_cachedMainCamera == null || _monitoredEnemyList == null) return;

            // 1. HashSet 동기화 (O(N))
            _enemyLookupSet.Clear();
            try 
            {
                int enemyCount = _monitoredEnemyList.Count;
                for (int i = 0; i < enemyCount; i++)
                {
                    Enemy enemy = _monitoredEnemyList[i];
                    if (enemy != null) _enemyLookupSet.Add(enemy);
                }
            }
            catch (System.Exception)
            {
                return;
            }

            // 2. 신규 적 체크 (HashSet 기반)
            foreach (Enemy enemy in _enemyLookupSet)
            {
                if (!_indicatorMap.ContainsKey(enemy))
                {
                    AddIndicator(enemy);
                }
            }

            // 3. 기존 인디케이터 업데이트 및 제거
            for (int i = _activeIndicators.Count - 1; i >= 0; i--)
            {
                EnemyIndicator indicator = _activeIndicators[i];
                if (indicator == null) { _activeIndicators.RemoveAt(i); continue; }

                Enemy target = indicator.TargetEnemy;

                if (target == null || target.IsDead || !_enemyLookupSet.Contains(target))
                {
                    RemoveIndicatorAtIndex(i, target);
                }
                else
                {
                    indicator.UpdateIndicator(_cachedMainCamera, _indicatorContainer);
                }
            }

            // 4. [고성능 최적화] 가장자리 라인별 버킷 정렬 방식의 중첩 방지 (O(N log N))
            ResolveOverlapsFast();
            
            // 5. 최종 위치를 기준으로 회전값 보정
            for (int i = 0; i < _activeIndicators.Count; i++)
            {
                if (_activeIndicators[i] != null && _activeIndicators[i].gameObject.activeSelf)
                {
                    _activeIndicators[i].FixRotation();
                }
            }
        }

        /// <summary>
        /// 인디케이터들을 4개 벽면으로 분류하고 각 면에서 정렬하여 인접한 것들끼리만 밀어냄
        /// </summary>
        private void ResolveOverlapsFast()
        {
            _topBucket.Clear(); _bottomBucket.Clear(); _leftBucket.Clear(); _rightBucket.Clear();
            
            Rect rect = _indicatorContainer.rect;
            float halfW = rect.width * 0.5f;
            float halfH = rect.height * 0.5f;
            float limitX = Mathf.Max(0, halfW - _margin);
            float limitY = Mathf.Max(0, halfH - _margin);

            // [Step 1] 버킷 분류 (O(N))
            int activeCount = _activeIndicators.Count;
            for (int i = 0; i < activeCount; i++)
            {
                EnemyIndicator ind = _activeIndicators[i];
                if (ind == null || !ind.gameObject.activeSelf) continue;

                Vector2 pos = ind.AnchoredPosition;
                // 가장 가까운 가장자리 벽면으로 분류
                if (pos.y >= limitY - 2f) _topBucket.Add(ind);
                else if (pos.y <= -limitY + 2f) _bottomBucket.Add(ind);
                else if (pos.x <= -limitX + 2f) _leftBucket.Add(ind);
                else if (pos.x >= limitX - 2f) _rightBucket.Add(ind);
            }

            // [Step 2] 각 버킷 내 정렬 및 인접 밀어내기 (O(N log N))
            ResolveSingleBucket(_topBucket, true, limitX, limitY);
            ResolveSingleBucket(_bottomBucket, true, limitX, limitY);
            ResolveSingleBucket(_leftBucket, false, limitX, limitY);
            ResolveSingleBucket(_rightBucket, false, limitX, limitY);
        }

        private void ResolveSingleBucket(List<EnemyIndicator> bucket, bool isHorizontal, float limitX, float limitY)
        {
            if (bucket.Count < 2) return;

            // 좌표 기반 정렬 (Horizontal은 X축, Vertical은 Y축)
            if (isHorizontal)
                bucket.Sort((a, b) => a.AnchoredPosition.x.CompareTo(b.AnchoredPosition.x));
            else
                bucket.Sort((a, b) => a.AnchoredPosition.y.CompareTo(b.AnchoredPosition.y));

            // 인접한 항목끼리만 밀어냄 (2회 반복으로 연쇄 밀림 처리)
            for (int iter = 0; iter < 2; iter++)
            {
                for (int i = 0; i < bucket.Count - 1; i++)
                {
                    EnemyIndicator a = bucket[i];
                    EnemyIndicator b = bucket[i + 1];
                    Vector2 posA = a.AnchoredPosition;
                    Vector2 posB = b.AnchoredPosition;

                    float diff = isHorizontal ? (posB.x - posA.x) : (posB.y - posA.y);

                    if (diff < _minSpacing)
                    {
                        float push = (_minSpacing - diff) * 0.5f;
                        if (isHorizontal)
                        {
                            a.AnchoredPosition -= new Vector2(push, 0);
                            b.AnchoredPosition += new Vector2(push, 0);
                            a.AnchoredPosition = new Vector2(Mathf.Clamp(a.AnchoredPosition.x, -limitX, limitX), a.AnchoredPosition.y);
                            b.AnchoredPosition = new Vector2(Mathf.Clamp(b.AnchoredPosition.x, -limitX, limitX), b.AnchoredPosition.y);
                        }
                        else
                        {
                            a.AnchoredPosition -= new Vector2(0, push);
                            b.AnchoredPosition += new Vector2(0, push);
                            a.AnchoredPosition = new Vector2(a.AnchoredPosition.x, Mathf.Clamp(a.AnchoredPosition.y, -limitY, limitY));
                            b.AnchoredPosition = new Vector2(b.AnchoredPosition.x, Mathf.Clamp(b.AnchoredPosition.y, -limitY, limitY));
                        }
                    }
                }
            }
        }

        private void AddIndicator(Enemy enemy)
        {
            EnemyIndicator indicator = _poolManager.Pop<EnemyIndicator>(_indicatorPoolItem);
            if (indicator != null)
            {
                indicator.transform.SetParent(_indicatorContainer, false);
                indicator.SetTarget(enemy, _margin);
                _indicatorMap[enemy] = indicator;
                _activeIndicators.Add(indicator);
            }
        }

        private void RemoveIndicatorAtIndex(int index, Enemy target)
        {
            EnemyIndicator indicator = _activeIndicators[index];
            if (target != null) _indicatorMap.Remove(target);
            
            Enemy keyToRemove = null;
            foreach (var kvp in _indicatorMap)
            {
                if (kvp.Value == indicator) { keyToRemove = kvp.Key; break; }
            }
            if (keyToRemove != null) _indicatorMap.Remove(keyToRemove);

            _activeIndicators.RemoveAt(index);
            _poolManager.Push(indicator);
        }
    }
}
