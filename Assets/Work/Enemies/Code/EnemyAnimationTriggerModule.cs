using System;
using System.Collections;
using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyAnimationTriggerModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _owner;

        public Action OnAnimationEnd;

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
        }

        public void AnimationEndTrigger()
        {
            OnAnimationEnd?.Invoke();
        }
    }
}