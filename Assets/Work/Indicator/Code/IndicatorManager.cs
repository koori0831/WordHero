using System.Collections.Generic;
using UnityEngine;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 모든 적 인디케이터를 중앙에서 생성, 제거 및 업데이트 관리하는 시스템
    /// </summary>
    public class IndicatorManager : MonoBehaviour
    {
        [SerializeField] private PoolItemSO _indicatorPoolItem;
        [SerializeField] private float _margin = 50f;
        [SerializeField] private RectTransform _indicatorContainer;

        [Inject] private PoolManagerSO _poolManager;

        private readonly Dictionary<Enemy, EnemyIndicator> _indicatorMap = new Dictionary<Enemy, EnemyIndicator>();
        private readonly List<EnemyIndicator> _activeIndicators = new List<EnemyIndicator>();

        private void Start()
        {
            Bus<OnEnemySpawnEvent>.Events += HandleEnemySpawn;
            Bus<OnEnemyDestroyEvent>.Events += HandleEnemyDestroy;

            // 이미 씬에 존재하는 적들이 있을 경우를 대비한 초기화
            Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (Enemy enemy in existingEnemies)
            {
                AddIndicator(enemy);
            }
        }

        private void OnDestroy()
        {
            Bus<OnEnemySpawnEvent>.Events -= HandleEnemySpawn;
            Bus<OnEnemyDestroyEvent>.Events -= HandleEnemyDestroy;
        }

        private void Update()
        {
            // 모든 활성 인디케이터를 중앙에서 일괄 업데이트 (성능 최적화)
            int count = _activeIndicators.Count;
            for (int i = 0; i < count; i++)
            {
                _activeIndicators[i].UpdateIndicator();
            }
        }

        private void HandleEnemySpawn(OnEnemySpawnEvent evt)
        {
            AddIndicator(evt.Enemy);
        }

        private void HandleEnemyDestroy(OnEnemyDestroyEvent evt)
        {
            RemoveIndicator(evt.Enemy);
        }

        private void AddIndicator(Enemy enemy)
        {
            if (enemy == null || _indicatorMap.ContainsKey(enemy))
            {
                return;
            }

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

        private void RemoveIndicator(Enemy enemy)
        {
            if (_indicatorMap.TryGetValue(enemy, out EnemyIndicator indicator))
            {
                _activeIndicators.Remove(indicator);
                _indicatorMap.Remove(enemy);
                _poolManager.Push(indicator);
            }
        }
    }
}
