using System;
using System.Collections.Generic;
using UnityEngine;

namespace Work.Enemies.Code
{
    [CreateAssetMenu(menuName = "SO/Enemy/Wave Data")]
    public class EnemyWaveDataSO : ScriptableObject
    {
        [field: SerializeField] public List<EnemyWave> Waves { get; private set; } = new List<EnemyWave>();
    }

    [Serializable]
    public class EnemyWave
    {
        [field: SerializeField] public List<EnemySpawnEntry> Enemies { get; private set; } = new List<EnemySpawnEntry>();
        [field: SerializeField] public float NextWaveDelay { get; private set; } = 1f;
        [field: SerializeField] public float ForceNextWaveTime { get; private set; } = 20f;
    }

    [Serializable]
    public class EnemySpawnEntry
    {
        [field: SerializeField] public Enemy Prefab { get; private set; }
        [field: SerializeField, Min(1)] public int Count { get; private set; } = 1;
    }
}
