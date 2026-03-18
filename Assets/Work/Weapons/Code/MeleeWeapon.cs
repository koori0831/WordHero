using UnityEngine;
using Work.Combat.Code;
using static UnityEngine.UI.GridLayoutGroup;

namespace Work.Weapons.Code
{
    public class MeleeWeapon : BaseWeapon
    {
        private Collider _hitBox;

        private void Awake()
        {
            _hitBox = GetComponent<Collider>();
            _hitBox.enabled = false;
        }

        public void StartAttack()
        {
            _hitBox.enabled = true;
        }

        public void EndAttack()
        {
            _hitBox.enabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Owner != null && collision.gameObject == Owner.gameObject) return;

            DamageContext context = new DamageContext(Owner != null ? Owner.gameObject : gameObject, collision.gameObject, Data.BaseDamage);
            KnockbackData knockbackData = new KnockbackData(5f, 0.5f, (collision.transform.position - Owner.Transform.position).normalized, AnimationCurve.EaseInOut(0, 1, 1, 0));
            DamageResolver.TryApplyDamage(context, true , knockbackData);
        }
    }
}
