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

    [Serializable]
    public class WaveSpawnAreaGroup
    {
        [field: SerializeField] public List<BoxCollider> SpawnAreas { get; private set; } = new List<BoxCollider>();
    }

    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemyWaveDataSO waveData;
        [SerializeField] private List<WaveSpawnAreaGroup> waveSpawnAreaGroups = new List<WaveSpawnAreaGroup>();
        [SerializeField] private float minEnemyDistance = 2f;
        [SerializeField] private float entranceAvoidRadius = 5f;
        [SerializeField] private float navMeshSampleRadius = 3f;
        [SerializeField, Min(1)] private int spawnTryCount = 30;
        [SerializeField] private GameObject spawnEffectPrefab;
        [SerializeField, Min(0f)] private float spawnEffectDuration = 1.5f;
        [SerializeField, Min(0f)] private float enemySpawnDelay = 0.5f;

        private readonly List<Enemy> currentEnemies = new List<Enemy>();
        private readonly Dictionary<Enemy, Action> deathHandlers = new Dictionary<Enemy, Action>();
        private readonly List<Vector3> waveSpawnPositions = new List<Vector3>();

        private BattleStage currentStage;
        private Coroutine waveCoroutine;
        private int currentWaveIndex;
        private int pendingSpawnCount;
        private bool isInitialized;
        private bool allWavesSpawned;
        private bool isWaitingNextWave;
        private bool isStageCleared;

        public bool IsCanMoveRoom => currentEnemies.Count <= 0;

        public void Init(BattleStage stage)
        {
            if (isInitialized)
                return;

            ResetState(stage);

            if (ValidateSettings() == false)
                return;

            SpawnNextWave();
        }

        private void OnDestroy()
        {
            StopWaveCoroutine();
            UnregisterAllEnemies();
        }

        private void ResetState(BattleStage stage)
        {
            isInitialized = true;
            currentStage = stage;
            currentWaveIndex = 0;
            pendingSpawnCount = 0;
            allWavesSpawned = false;
            isWaitingNextWave = false;
            isStageCleared = false;
        }

        private bool ValidateSettings()
        {
            if (waveData == null || waveData.Waves.Count <= 0)
            {
                Debug.LogWarning($"[EnemyManager] Wave data is empty on '{name}'. Stage will be cleared immediately.", this);
                allWavesSpawned = true;
                TryClearStage();
                return false;
            }

            if (HasValidSpawnAreaGroups() == false)
            {
                return false;
            }

            return true;
        }

        private void StopWaveCoroutine()
        {
            if (waveCoroutine == null)
                return;

            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }

        private void UnregisterAllEnemies()
        {
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

            StopWaveCoroutine();

            if (currentWaveIndex >= waveData.Waves.Count)
            {
                allWavesSpawned = true;
                TryClearStage();
                return;
            }

            EnemyWave wave = waveData.Waves[currentWaveIndex];
            WaveSpawnAreaGroup spawnAreaGroup = waveSpawnAreaGroups[currentWaveIndex];
            currentWaveIndex++;
            allWavesSpawned = currentWaveIndex >= waveData.Waves.Count;

            int immediateSpawnCount = SpawnWaveEnemies(wave, spawnAreaGroup);

            if (immediateSpawnCount > 0)
            {
                RaiseEnemySpawnedEvent();
            }

            if (HasActiveEnemiesOrPendingSpawns() == false)
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

        }

        private int SpawnWaveEnemies(EnemyWave wave, WaveSpawnAreaGroup spawnAreaGroup)
        {
            waveSpawnPositions.Clear();
            int immediateSpawnCount = 0;

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
                    if (TryGetSpawnPosition(spawnAreaGroup, out Vector3 spawnPosition) == false)
                    {
                        Debug.LogWarning($"[EnemyManager] Failed to find spawn position for '{entry.Prefab.name}' on '{name}'.", this);
                        continue;
                    }

                    Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                    if (SpawnEnemyOrSchedule(entry.Prefab, spawnPosition, rotation))
                    {
                        immediateSpawnCount++;
                    }

                    waveSpawnPositions.Add(spawnPosition);
                }
            }

            return immediateSpawnCount;
        }

        private bool SpawnEnemyOrSchedule(Enemy prefab, Vector3 spawnPosition, Quaternion rotation)
        {
            if (spawnEffectPrefab == null && enemySpawnDelay <= 0f)
            {
                CreateEnemyInstance(prefab, spawnPosition, rotation);
                return true;
            }

            pendingSpawnCount++;
            StartCoroutine(SpawnEnemyWithEffect(prefab, spawnPosition, rotation));
            return false;
        }

        private IEnumerator SpawnEnemyWithEffect(Enemy prefab, Vector3 spawnPosition, Quaternion rotation)
        {
            if (spawnEffectPrefab != null)
            {
                GameObject effect = Instantiate(spawnEffectPrefab, spawnPosition, rotation);
                if (spawnEffectDuration > 0f)
                {
                    Destroy(effect, spawnEffectDuration);
                }
            }

            if (enemySpawnDelay > 0f)
            {
                yield return new WaitForSeconds(enemySpawnDelay);
            }

            pendingSpawnCount--;
            CreateEnemyInstance(prefab, spawnPosition, rotation);
            RaiseEnemySpawnedEvent();
            EvaluateWaveProgress();
        }

        private void CreateEnemyInstance(Enemy prefab, Vector3 spawnPosition, Quaternion rotation)
        {
            Enemy enemy = Instantiate(prefab, spawnPosition, rotation, transform);
            enemy.Init();
            RegisterEnemy(enemy);
        }

        private bool TryGetSpawnPosition(WaveSpawnAreaGroup spawnAreaGroup, out Vector3 spawnPosition)
        {
            for (int i = 0; i < spawnTryCount; i++)
            {
                BoxCollider spawnArea = GetRandomSpawnArea(spawnAreaGroup);
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

        private bool HasValidSpawnAreaGroups()
        {
            if (waveSpawnAreaGroups.Count < waveData.Waves.Count)
            {
                Debug.LogError($"[EnemyManager] Wave spawn area group count is less than wave count on '{name}'.", this);
                return false;
            }

            for (int i = 0; i < waveData.Waves.Count; i++)
            {
                if (HasValidSpawnArea(waveSpawnAreaGroups[i]) == false)
                {
                    Debug.LogError($"[EnemyManager] Wave {i + 1} spawn areas are missing on '{name}'.", this);
                    return false;
                }
            }

            return true;
        }

        private bool HasValidSpawnArea(WaveSpawnAreaGroup spawnAreaGroup)
        {
            if (spawnAreaGroup == null)
                return false;

            for (int i = 0; i < spawnAreaGroup.SpawnAreas.Count; i++)
            {
                if (spawnAreaGroup.SpawnAreas[i] != null && GetSpawnAreaWeight(spawnAreaGroup.SpawnAreas[i]) > 0f)
                    return true;
            }

            return false;
        }

        private BoxCollider GetRandomSpawnArea(WaveSpawnAreaGroup spawnAreaGroup)
        {
            float totalWeight = 0f;
            for (int i = 0; i < spawnAreaGroup.SpawnAreas.Count; i++)
            {
                totalWeight += GetSpawnAreaWeight(spawnAreaGroup.SpawnAreas[i]);
            }

            if (totalWeight <= 0f)
                return null;

            float randomWeight = UnityEngine.Random.Range(0f, totalWeight);
            for (int i = 0; i < spawnAreaGroup.SpawnAreas.Count; i++)
            {
                BoxCollider spawnArea = spawnAreaGroup.SpawnAreas[i];
                float weight = GetSpawnAreaWeight(spawnArea);
                if (weight <= 0f)
                    continue;

                if (randomWeight <= weight)
                    return spawnArea;

                randomWeight -= weight;
            }

            return null;
        }

        private float GetSpawnAreaWeight(BoxCollider spawnArea)
        {
            if (spawnArea == null)
                return 0f;

            Bounds bounds = spawnArea.bounds;
            return Mathf.Max(0f, bounds.size.x * bounds.size.z);
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

        private IEnumerator SpawnNextWaveAfter(float seconds)
        {
            isWaitingNextWave = true;
            yield return new WaitForSeconds(seconds);
            waveCoroutine = null;

            if (HasActiveEnemiesOrPendingSpawns() == false)
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

            EvaluateWaveProgress();
        }

        private void EvaluateWaveProgress()
        {
            if (HasActiveEnemiesOrPendingSpawns())
                return;

            if (allWavesSpawned)
            {
                TryClearStage();
                return;
            }

            if (isWaitingNextWave)
                return;

            StopWaveCoroutine();

            float delay = 0f;
            int previousWaveIndex = currentWaveIndex - 1;
            if (previousWaveIndex >= 0 && previousWaveIndex < waveData.Waves.Count)
            {
                delay = waveData.Waves[previousWaveIndex].NextWaveDelay;
            }

            waveCoroutine = StartCoroutine(SpawnNextWaveAfter(delay));
        }

        private void TryClearStage()
        {
            if (allWavesSpawned && HasActiveEnemiesOrPendingSpawns() == false)
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

        private bool HasActiveEnemiesOrPendingSpawns()
        {
            return currentEnemies.Count > 0 || pendingSpawnCount > 0;
        }

    }
}
