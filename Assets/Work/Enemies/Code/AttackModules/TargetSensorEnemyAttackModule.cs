using System.Collections.Generic;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Enemies.Code.AttackModules
{
    public class TargetSensorEnemyAttackModule : EnemyAttackModule
    {
        [SerializeField] protected TargetSensor targetSensor;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            targetSensor?.Init(agent);
        }

        public override void Attack()
        {
            if (targetSensor == null)
                return;

            List<IDamageable> damageables = targetSensor.Cast<IDamageable>();

            damageables.ForEach(d =>
            {
                Component target = d as Component;
                if (target == null)
                    return;

                KnockbackData knockbackData = new KnockbackData(
                    5f,
                    0.5f,
                    (target.transform.position - _owner.Transform.position).normalized,
                    AnimationCurve.EaseInOut(0, 1, 1, 0));

                DamageContext context = new DamageContext(_owner.gameObject, target.gameObject, damage);
                DamageResolver.TryApplyDamage(context, true, knockbackData);
            });
        }
    }
}
