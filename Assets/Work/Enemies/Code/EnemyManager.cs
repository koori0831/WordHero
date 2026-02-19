using Assets.Work.Maps.Code;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public List<Enemy> enemies;

        public bool IsCanMoveRoom => enemies.Count <= 0;

        public void Awake()
        {
            foreach (var enemy in enemies)
            {
                enemy.Init(this);
            }
        }
    }
}
