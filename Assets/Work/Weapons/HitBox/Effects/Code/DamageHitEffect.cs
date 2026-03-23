using System;
using UnityEngine;
using Work.Combat.Code;
using Alchemy.Inspector;
using Work.Weapons.HitBox.Code;

namespace Work.Weapons.HitBox.Effects.Code
{
    [Serializable]
    public class DamageHitEffect : IHitEffect
    {
        public int Damage;
        public bool UseKnockback = false;
        [ShowIf(nameof(UseKnockback))]
        public float KnockbackForce = 5f;
        [ShowIf(nameof(UseKnockback))]
        public float KnockbackDuration = 0.5f;

        public void ExecuteHit(GameObject caster, GameObject target, Vector3 hitPoint)
        {
            DamageContext context = new DamageContext(caster, target, Damage);

            Vector3 knockbackDirection = (target.transform.position - caster.transform.position).normalized;
            KnockbackData knockbackData = new KnockbackData
            {
                Force = KnockbackForce,
                Duration = KnockbackDuration,
                Direction = knockbackDirection,
                KnockbackCurve = AnimationCurve.EaseInOut(0, 0, 1, 1)
            };

            DamageResolver.TryApplyDamage(context, UseKnockback, knockbackData);
        }
    }
}