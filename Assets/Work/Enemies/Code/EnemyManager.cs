using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.Enemies.Code
{

    public record struct OnEnemySpawnedEvent(List<Enemy> enemyList) : IEvent;

    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemyWaveDataSO waveData;
        [SerializeField] private List<BoxCollider> spawnAreas = new List<BoxCollider>();
        [SerializeField] private float minEnemyDistance = 2f;
        [SerializeField] private float entranceAvoidRadius = 5f;
        [SerializeField] private float navMeshSampleRadius = 3f;
        [SerializeField, Min(1)] private int spawnTryCount = 30;

        private readonly List<Enemy> currentEnemies = new List<Enemy>();
        private readonly Dictionary<Enemy, Action> deathHandlers = new Dictionary<Enemy, Action>();
        private readonly List<Vector3> waveSpawnPositions = new List<Vector3>();

        private BattleStage currentStage;
        private Coroutine waveCoroutine;
        private int currentWaveIndex;
        private bool isInitialized;
        private bool allWavesSpawned;
        private bool isWaitingNextWave;
        private bool isStageCleared;

        public bool IsCanMoveRoom => currentEnemies.Count <= 0;

        public void Init(BattleStage stage)
        {
            if (isInitialized)
                return;

            isInitialized = true;
            currentStage = stage;
            currentWaveIndex = 0;
            allWavesSpawned = false;
            isWaitingNextWave = false;
            isStageCleared = false;

            if (waveData == null || waveData.Waves.Count <= 0)
            {
                Debug.LogWarning($"[EnemyManager] Wave data is empty on '{name}'. Stage will be cleared immediately.", this);
                allWavesSpawned = true;
                TryClearStage();
                return;
            }

            if (HasValidSpawnArea() == false)
            {
                Debug.LogError($"[EnemyManager] Spawn areas are missing on '{name}'.", this);
                return;
            }

            SpawnNextWave();
        }

        private void OnDestroy()
        {
            if (waveCoroutine != null)
            {
                StopCoroutine(waveCoroutine);
                waveCoroutine = null;
            }

            foreach (KeyValuePair<Enemy, Action> pair in deathHandlers)
            {
                if (pair.Key != null && pair.Key.EnemyInfoData != null)
                {
                    pair.Key.EnemyInfoData.HpValue.OnDead -= pair.Value;
                }
            }

            deathHandlers.Clear();
        }

        private void SpawnNextWave()
        {
            isWaitingNextWave = false;

            if (waveCoroutine != null)
            {
                StopCoroutine(waveCoroutine);
                waveCoroutine = null;
            }

            if (currentWaveIndex >= waveData.Waves.Count)
            {
                allWavesSpawned = true;
                TryClearStage();
                return;
            }

            EnemyWave wave = waveData.Waves[currentWaveIndex];
            currentWaveIndex++;
            allWavesSpawned = currentWaveIndex >= waveData.Waves.Count;

            SpawnWaveEnemies(wave);
            RaiseEnemySpawnedEvent();

            if (currentEnemies.Count <= 0)
            {
                if (allWavesSpawned)
                {
                    TryClearStage();
                }
                else
                {
                    waveCoroutine = StartCoroutine(SpawnNextWaveAfter(wave.NextWaveDelay));
                }

                return;
            }

            if (wave.ForceNextWaveTime > 0f && allWavesSpawned == false)
            {
                waveCoroutine = StartCoroutine(ForceNextWaveAfter(wave.ForceNextWaveTime));
            }
        }

        private void SpawnWaveEnemies(EnemyWave wave)
        {
            waveSpawnPositions.Clear();

            for (int i = 0; i < wave.Enemies.Count; i++)
            {
                EnemySpawnEntry entry = wave.Enemies[i];
                if (entry.Prefab == null)
                {
                    Debug.LogWarning($"[EnemyManager] Enemy prefab is missing in wave {currentWaveIndex} on '{name}'.", this);
                    continue;
                }

                for (int j = 0; j < entry.Count; j++)
                {
                    if (TryGetSpawnPosition(out Vector3 spawnPosition) == false)
                    {
                        Debug.LogWarning($"[EnemyManager] Failed to find spawn position for '{entry.Prefab.name}' on '{name}'.", this);
                        continue;
                    }

                    Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                    Enemy enemy = Instantiate(entry.Prefab, spawnPosition, rotation, transform);
                    enemy.Init();
                    RegisterEnemy(enemy);
                    waveSpawnPositions.Add(spawnPosition);
                }
            }
        }

        private bool TryGetSpawnPosition(out Vector3 spawnPosition)
        {
            for (int i = 0; i < spawnTryCount; i++)
            {
                BoxCollider spawnArea = GetRandomSpawnArea();
                if (spawnArea == null)
                    break;

                Bounds bounds = spawnArea.bounds;
                Vector3 randomPoint = new Vector3(
                    UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                    UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                    UnityEngine.Random.Range(bounds.min.z, bounds.max.z));

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas) == false)
                    continue;

                Vector3 candidate = hit.position;

                if (Vector3.Distance(candidate, currentStage.SpawnPoint) < entranceAvoidRadius)
                    continue;

                if (IsTooCloseToOtherEnemy(candidate))
                    continue;

                spawnPosition = candidate;
                return true;
            }

            spawnPosition = default;
            return false;
        }

        private bool HasValidSpawnArea()
        {
            for (int i = 0; i < spawnAreas.Count; i++)
            {
                if (spawnAreas[i] != null)
                    return true;
            }

            return false;
        }

        private BoxCollider GetRandomSpawnArea()
        {
            int validAreaCount = 0;
            for (int i = 0; i < spawnAreas.Count; i++)
            {
                if (spawnAreas[i] != null)
                    validAreaCount++;
            }

            if (validAreaCount <= 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, validAreaCount);
            for (int i = 0; i < spawnAreas.Count; i++)
            {
                if (spawnAreas[i] == null)
                    continue;

                if (randomIndex == 0)
                    return spawnAreas[i];

                randomIndex--;
            }

            return null;
        }

        private bool IsTooCloseToOtherEnemy(Vector3 position)
        {
            float minDistanceSqr = minEnemyDistance * minEnemyDistance;

            for (int i = 0; i < currentEnemies.Count; i++)
            {
                Enemy enemy = currentEnemies[i];
                if (enemy == null || enemy.IsDead)
                    continue;

                if ((enemy.transform.position - position).sqrMagnitude < minDistanceSqr)
                    return true;
            }

            for (int i = 0; i < waveSpawnPositions.Count; i++)
            {
                if ((waveSpawnPositions[i] - position).sqrMagnitude < minDistanceSqr)
                    return true;
            }

            return false;
        }

        private void RegisterEnemy(Enemy enemy)
        {
            currentEnemies.Add(enemy);

            Action deathHandler = () => HandleDeadEvent(enemy);
            deathHandlers.Add(enemy, deathHandler);
            enemy.EnemyInfoData.HpValue.OnDead += deathHandler;
        }

        private IEnumerator ForceNextWaveAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            waveCoroutine = null;

            if (allWavesSpawned == false)
            {
                SpawnNextWave();
            }
        }

        private IEnumerator SpawnNextWaveAfter(float seconds)
        {
            isWaitingNextWave = true;
            yield return new WaitForSeconds(seconds);
            waveCoroutine = null;

            if (currentEnemies.Count <= 0)
            {
                SpawnNextWave();
            }
        }

        private void HandleDeadEvent(Enemy deadEnemy)
        {
            if (deadEnemy != null && deathHandlers.TryGetValue(deadEnemy, out Action deathHandler))
            {
                deadEnemy.EnemyInfoData.HpValue.OnDead -= deathHandler;
                deathHandlers.Remove(deadEnemy);
            }

            currentEnemies.Remove(deadEnemy);
            RaiseEnemySpawnedEvent();

            if (allWavesSpawned)
            {
                TryClearStage();
                return;
            }

            if (currentEnemies.Count <= 0 && isWaitingNextWave == false)
            {
                if (waveCoroutine != null)
                {
                    StopCoroutine(waveCoroutine);
                    waveCoroutine = null;
                }

                float delay = 0f;
                int previousWaveIndex = currentWaveIndex - 1;
                if (previousWaveIndex >= 0 && previousWaveIndex < waveData.Waves.Count)
                {
                    delay = waveData.Waves[previousWaveIndex].NextWaveDelay;
                }

                waveCoroutine = StartCoroutine(SpawnNextWaveAfter(delay));
            }
        }

        private void TryClearStage()
        {
            if (allWavesSpawned && currentEnemies.Count <= 0)
            {
                if (isStageCleared)
                    return;

                isStageCleared = true;
                Bus<OnChestCreatEvent>.Raise(new OnChestCreatEvent());
            }
        }

        private void RaiseEnemySpawnedEvent()
        {
            Bus<OnEnemySpawnedEvent>.Raise(new OnEnemySpawnedEvent(currentEnemies));
        }

    }
}
