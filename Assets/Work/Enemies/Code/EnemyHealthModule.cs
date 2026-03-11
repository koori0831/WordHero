using System;
using UnityEngine;
using UnityEngine.Events;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;

namespace Work.Enemies.Code
{

    public class EnemyHealthModule : AgentHealthModule
    {
        private Enemy Enemy => _owner as Enemy;
        public override void TakeDamage(int damageAmount)
        {
            Enemy.StateChangeChannel.SendEventMessage(EnemyState.Hit);
            Bus<EnemyHitEvent>.Raise(new EnemyHitEvent(_owner.gameObject, _owner.InfoData));
            base.TakeDamage(damageAmount);
        }

        
    }
}