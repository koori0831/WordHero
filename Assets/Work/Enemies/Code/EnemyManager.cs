using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.Enemies.Code
{

    public record struct OnEnemySpawnedEvent(List<Enemy> enemyList) : IEvent;

    public class EnemyManager : MonoBehaviour
    {
        private List<Enemy> currentEnemies = new List<Enemy>();

        public bool IsCanMoveRoom => currentEnemies.Count <= 0;

        // 적 위치 잡는거
        // 적 스폰하는거
        // 처음에는 방에 어느정도 문 앞쪽에만 존재하고
        // 걔네들 죽이다보면 뒤에 

        public void Start()
        {
            currentEnemies = GetComponentsInChildren<Enemy>().ToList();

            int enemyCount = currentEnemies.Count;

            for (int i = 0; i < enemyCount; i++)
            {
                Enemy enemy = currentEnemies[i];
                enemy.Init();
                enemy.EnemyInfoData.HpValue.OnDead += HandleDeadEvent;
            }
        }

        private void HandleDeadEvent()
        {
            Debug.Log("Enemy Dead");
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
            Debug.Log("Current Enemies Count: " + currentEnemies.Count);

            if (IsCanMoveRoom)
            {
                Bus<OnChestCreatEvent>.Raise(new OnChestCreatEvent());
            }
        }

    }
}
