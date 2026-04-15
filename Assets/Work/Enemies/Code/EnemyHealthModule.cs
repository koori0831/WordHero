using System;
using UnityEngine;
using UnityEngine.Events;
using Work.Agents.Code;
using Work.Combat.Code;
using Work.Core.Utils.EventBus;

namespace Work.Enemies.Code
{

    public class EnemyHealthModule : AgentHealthModule
    {
        private Enemy Enemy => _owner as Enemy;

        public override void AfterInitialize()
        {
            Enemy.EnemyInfoData.StatusValue.OnStatusTickEvent += HandleStatusTickEvent;
        }

        private void HandleStatusTickEvent(StatusType statusType, float value)
        {
            switch (statusType)
            {
                case StatusType.Freeze:
                    {
                        Bus<DamageTextEvent>.Raise(new DamageTextEvent((int)value, Enemy.gameObject, false, DamageTextType.Freeze));
                        TakeDamage((int)value);
                    }
                    break;
            }
        }

        public override void TakeDamage(int damageAmount)
        {
            Enemy.StateChangeChannel.SendEventMessage(EnemyState.Hit);
            Bus<EnemyHitEvent>.Raise(new EnemyHitEvent(_owner.gameObject, _owner.InfoData));
            Bus<GetSkillEnergyEvent>.Raise(new GetSkillEnergyEvent(0.02f));
            base.TakeDamage(damageAmount);
        }

        
    }
}