using System;
using UnityEngine;
using Work.Combat.Code;

namespace Work.Enemies.Code
{
    public class EnemyKnockbackModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _owner;
        private EnemyStatusModule _statusModule;

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            _owner.OnKnockbackEvent.AddListener(ApplyKnockback);

            _statusModule = _owner.GetModule<EnemyStatusModule>();
        }

        private void ApplyKnockback(KnockbackData knockbackData)
        {
            if (_statusModule.HasStatusEffect(StatusType.HitImmunity))
                return;
            if (_statusModule.HasStatusEffect(StatusType.SuperArmor))
                return;
            if (_statusModule.HasStatusEffect(StatusType.Invincible))
                return;
            if (_statusModule.HasStatusEffect(StatusType.KnockbackImmune))
                return;
        }
    }
}