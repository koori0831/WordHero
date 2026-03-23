using UnityEngine;
using Work.Interaction.Code;
using Work.Players.Code;
using Work.Weapons.Skill.Code;

namespace Work.Weapons.Code
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public Player Owner { get; set; }

        [field: SerializeField] public WeaponDataSO Data { get; private set; }

        public bool IsSkillUsing { get; set; }

        public void UsePrimary(Transform target, Vector3 direction) => ExecuteSkill(Data?.PrimarySkill, target, direction);
        public void UseSecondary(Transform target, Vector3 direction) => ExecuteSkill(Data?.SecondarySkill, target, direction);
        public void UseTrigger(Transform target, Vector3 direction) => ExecuteSkill(Data?.TriggerSkill, target, direction);

        private void ExecuteSkill(SkillDataSO skill, Transform target, Vector3 direction)
        {
            if (Owner == null || skill == null || IsSkillUsing == true) return;

            Vector3 targetPosition = target != null ? target.position : Owner.transform.position;
            Vector3 castDirection = direction.sqrMagnitude > 0f ? direction : Owner.transform.forward;

            skill.Cast(Owner, targetPosition, castDirection);
            if (skill.AnimParam) Owner.ChangeState(skill.AnimParam.stateName);

            IsSkillUsing = true;
        }
    }
}
