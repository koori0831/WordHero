using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.Enemies.Code
{
    public class EnemyManager : MonoBehaviour
    {
        private List<Enemy> currentEnemies = new List<Enemy>();

        public bool IsCanMoveRoom => currentEnemies.Count <= 0;

        public void Start()
        {
            currentEnemies = GetComponentsInChildren<Enemy>().ToList();

            int enemyCount = currentEnemies.Count;

            for (int i = 0; i < enemyCount; i++)
            {
                Enemy enemy = currentEnemies[i];
                currentEnemies.Add(enemy);
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
