using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// UITestScene 등에서 인디케이터 시스템을 테스트하기 위한 클래스
    /// </summary>
    public class IndicatorTestManager : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private int _spawnCount = 15;
        [SerializeField] private float _spawnRange = 40f;
        [SerializeField] private float _moveSpeed = 10f;

        private List<Enemy> _testEnemies = new List<Enemy>();
        private bool _isMoving = true;

        private void Update()
        {
            #if UNITY_EDITOR
            // 1번 키: 적 스폰 및 리스트 전송
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpawnAndSendList();
            }

            // 2번 키: 랜덤하게 적 하나 제거
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                RemoveRandomEnemy();
            }

            // 3번 키: 적들의 이동 토글
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                _isMoving = !_isMoving;
            }

            // 4번 키: 적 전체 제거
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
            {
                ClearEnemies();
            }

            // 테스트 적들을 원형으로 돌려서 인디케이터 움직임 확인
            if (_isMoving)
            {
                float time = Time.time * _moveSpeed;
                for (int i = 0; i < _testEnemies.Count; i++)
                {
                    if (_testEnemies[i] == null) continue;
                    
                    float angle = (i * Mathf.PI * 2 / _testEnemies.Count) + (time * 0.1f);
                    Vector3 targetPos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * _spawnRange;
                    _testEnemies[i].transform.position = targetPos;
                }
            }
            #endif
        }

        [ContextMenu("Spawn and Send List")]
        public void SpawnAndSendList()
        {
            ClearEnemies();

            for (int i = 0; i < _spawnCount; i++)
            {
                GameObject obj = new GameObject($"TestEnemy_{i}");
                TestEnemy enemy = obj.AddComponent<TestEnemy>();
                enemy.Init();
                
                _testEnemies.Add(enemy);
            }

            // 담당자가 구현할 "리스트 쏴주기" 호출
            Bus<OnEnemySpawnedEvent>.Raise(new OnEnemySpawnedEvent(_testEnemies));
            Debug.Log($"[Test] {_spawnCount} enemies spawned and list sent!");
        }

        private void RemoveRandomEnemy()
        {
            if (_testEnemies.Count == 0) return;

            int randomIndex = Random.Range(0, _testEnemies.Count);
            Enemy target = _testEnemies[randomIndex];

            if (target is TestEnemy testEnemy)
            {
                testEnemy.Kill();
                _testEnemies.RemoveAt(randomIndex);
                Destroy(target.gameObject);
                Debug.Log($"[Test] Random enemy removed. Remaining: {_testEnemies.Count}");
            }
        }

        private void ClearEnemies()
        {
            foreach (var enemy in _testEnemies)
            {
                if (enemy != null) Destroy(enemy.gameObject);
            }
            _testEnemies.Clear();
            Debug.Log("[Test] All test enemies cleared.");
        }
    }
}
