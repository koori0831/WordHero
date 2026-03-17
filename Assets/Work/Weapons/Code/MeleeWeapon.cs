using UnityEngine;
using Work.Combat.Code;

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
            DamageResolver.TryApplyDamage(context);
        }
    }
}
