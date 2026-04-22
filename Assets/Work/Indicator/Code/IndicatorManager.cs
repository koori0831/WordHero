using System.Collections.Generic;
using UnityEngine;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// EnemyManager의 리스트를 참조하여 모든 적 인디케이터를 동기화 관리하는 시스템
    /// </summary>
    public class IndicatorManager : MonoBehaviour
    {
        [SerializeField] private PoolItemSO _indicatorPoolItem;
        [SerializeField] private float _margin = 50f;
        [SerializeField] private RectTransform _indicatorContainer;

        [Inject] private PoolManagerSO _poolManager;

        private List<Enemy> _monitoredEnemyList;
        private readonly Dictionary<Enemy, EnemyIndicator> _indicatorMap = new Dictionary<Enemy, EnemyIndicator>();
        private readonly List<EnemyIndicator> _activeIndicators = new List<EnemyIndicator>();

        private void Start()
        {
            // EnemyManager에서 리스트를 전달해주는 이벤트를 구독
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

        private void Update()
        {
            if (_monitoredEnemyList == null) return;

            // 1. 리스트 기반으로 신규 적 인디케이터 추가
            int enemyCount = _monitoredEnemyList.Count;
            for (int i = 0; i < enemyCount; i++)
            {
                Enemy enemy = _monitoredEnemyList[i];
                if (enemy != null && !_indicatorMap.ContainsKey(enemy))
                {
                    AddIndicator(enemy);
                }
            }

            // 2. 동기화 및 업데이트 (리스트에 없는 적의 인디케이터 제거)
            for (int i = _activeIndicators.Count - 1; i >= 0; i--)
            {
                EnemyIndicator indicator = _activeIndicators[i];
                Enemy target = indicator.TargetEnemy;

                // 적이 죽었거나 리스트에서 사라졌다면 제거
                if (target == null || target.IsDead || !_monitoredEnemyList.Contains(target))
                {
                    RemoveIndicatorAtIndex(i);
                }
                else
                {
                    indicator.UpdateIndicator();
                }
            }
        }

        private void AddIndicator(Enemy enemy)
        {
            IPoolable poolable = _poolManager.Pop(_indicatorPoolItem);
            EnemyIndicator indicator = poolable as EnemyIndicator;

            if (indicator != null)
            {
                indicator.transform.SetParent(_indicatorContainer, false);
                indicator.SetTarget(enemy, _margin);
                
                _indicatorMap.Add(enemy, indicator);
                _activeIndicators.Add(indicator);
            }
        }

        private void RemoveIndicatorAtIndex(int index)
        {
            EnemyIndicator indicator = _activeIndicators[index];
            if (indicator.TargetEnemy != null)
            {
                _indicatorMap.Remove(indicator.TargetEnemy);
            }
            _activeIndicators.RemoveAt(index);
            _poolManager.Push(indicator);
        }
    }
}
