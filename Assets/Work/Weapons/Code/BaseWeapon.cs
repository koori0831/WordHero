using UnityEngine;
using Work.Agents.Code;
using Work.Interaction.Code;

namespace Work.Weapons.Code
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public Agent Owner { get; set; }

        [field: SerializeField] public WeaponDataSO Data { get; private set; }
        public void UsePrimary(Transform target, Vector3 direction)
        {
            if (Owner == null || Data == null) return;

            if (Data.PrimarySkill != null)
            {
                Vector3 targetPosition = target != null ? target.position : Owner.transform.position;
                Vector3 castDirection = direction.sqrMagnitude > 0f ? direction : Owner.transform.forward;
                Data.PrimarySkill.Cast(Owner.transform, targetPosition, castDirection);
            }
        }
        public void UseSecondary(Transform target, Vector3 direction)
        {
            if (Owner == null || Data == null) return;

            if (Data.SecondarySkill != null)
            {
                Vector3 targetPosition = target != null ? target.position : Owner.transform.position;
                Vector3 castDirection = direction.sqrMagnitude > 0f ? direction : Owner.transform.forward;
                Data.SecondarySkill.Cast(Owner.transform, targetPosition, castDirection);
            }
        }
        public void UseTrigger(Transform target, Vector3 direction)
        {
            if (Owner == null || Data == null) return;

            if (Data.TriggerSkill != null)
            {
                Vector3 targetPosition = target != null ? target.position : Owner.transform.position;
                Vector3 castDirection = direction.sqrMagnitude > 0f ? direction : Owner.transform.forward;
                Data.TriggerSkill.Cast(Owner.transform, targetPosition, castDirection);
            }
        }

        public virtual void Attack(Transform target, Vector3 direction)
        {
        }


        public void Drop(Vector3 dropPosition)
        {
            DropService.DropWeapon(this, dropPosition);
        }
    }
}
