using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Imprint.Code;
using Work.Weapons.Skill.Code;

namespace Work.Weapons.Code
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public Player Owner { get; set; }

        [field: SerializeField] public WeaponDataSO Data { get; private set; }

        public WeaponImprintSlots Imprints = new();

        public bool IsSkillUsing { get; set; }

        public virtual void Attack(int comboCount = 0) 
        {
            
        }

        public void UsePrimary(Transform target, Vector3 direction) => ExecuteSkill(Data?.PrimarySkill, target, direction);
        public void UseSecondary(Transform target, Vector3 direction) => ExecuteSkill(Data?.SecondarySkill, target, direction);

        private void ExecuteSkill(SkillDataSO skill, Transform target, Vector3 direction)
        {
            if (Owner == null || skill == null || IsSkillUsing == true) return;

            Vector3 targetPosition = target != null ? target.position : Owner.transform.position;
            Vector3 castDirection = direction.sqrMagnitude > 0f ? direction : Owner.transform.forward;

            skill.Cast(new (Owner, targetPosition, castDirection));
            if (skill.AnimParam) Owner.ChangeState(skill.AnimParam.stateName);

            IsSkillUsing = true;
        }
    }
}
