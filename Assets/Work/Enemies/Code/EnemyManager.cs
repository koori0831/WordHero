using Assets.Work.Maps.Code;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.Enemies.Code
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public List<Enemy> enemies;
        [field: SerializeField] public List<GameObject> enemySpawnPoints;
        [field: SerializeField] public int onePointInMaxEnemyCount = 20;
        [field: SerializeField] public float spawnRadius = 8f;

        private int _minEnemyCount => onePointInMaxEnemyCount - 5 <= 1 ? 1 : onePointInMaxEnemyCount - 5;
        private List<Enemy> currentEnemies = new List<Enemy>();
        
        public bool IsCanMoveRoom => currentEnemies.Count <= 0;

        public void Start()
        {
            if (enemies.Count <= 0) return;

            foreach(GameObject point in enemySpawnPoints)
            {
                Vector3 spawnPoint = point.transform.position;
                int enemyCount = UnityEngine.Random.Range(_minEnemyCount, onePointInMaxEnemyCount + 1);

                for(int i = 0; i < enemyCount; i++)
                {
                    Vector3 rnad = UnityEngine.Random.onUnitSphere * (spawnRadius);
                    Vector3 newPos = spawnPoint + new Vector3(rnad.x, 0, rnad.z);
                    newPos.y = 0;
                    Enemy enemy = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Count - 1)], newPos, Quaternion.identity);
                    currentEnemies.Add(enemy);
                    enemy.Init();
                    enemy.EnemyInfoData.HpValue.OnDead += HandleDeadEvent;
                    enemy.gameObject.transform.parent = point.transform;
                }
            }
        }

        private void HandleDeadEvent()
        {
            for (int i = currentEnemies.Count - 1; i >= 0; i--)
            {
                if (currentEnemies[i] == null)
                {
                    currentEnemies.RemoveAt(i);
                    break;
                }
                else if (currentEnemies[i].IsDead == true)
                {
                    currentEnemies[i].EnemyInfoData.HpValue.OnDead -= HandleDeadEvent;
                    currentEnemies.RemoveAt(i);
                    break;
                }
            }

            if(IsCanMoveRoom)
            {
                Bus<OnChestCreatEvent>.Raise(new OnChestCreatEvent());
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (enemySpawnPoints.Count <= 0) return;
            foreach (GameObject point in enemySpawnPoints)
            {
                Gizmos.DrawWireSphere(point.transform.position, spawnRadius);
            }
        }
    }
}
