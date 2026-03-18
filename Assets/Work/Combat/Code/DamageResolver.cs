using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Combat.Code
{
    public static class DamageResolver
    {
        public static bool TryApplyDamage(in DamageContext context, bool isKnockbackAttack = false, KnockbackData knockbackData = new KnockbackData())
        {
            if (context.Target == null) return false;
            if (!context.Target.TryGetComponent(out IDamageable damageable)) return false;

            if(isKnockbackAttack && context.Target.TryGetComponent(out IKnockbackable knockbackable))
            {
                knockbackable.TakeKnockback(knockbackData);
            }


            damageable.TakeDamage(context.Damage);

            GameObject source = context.Source != null ? context.Source : context.Target;
            Bus<CombatHitEvent>.Raise(new CombatHitEvent(source, context.Target, context.Damage, context.IsCritical));
            return true;
        }
    }
}
