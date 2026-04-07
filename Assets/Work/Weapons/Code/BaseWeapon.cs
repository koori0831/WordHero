using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Skill.Code;

namespace Work.Weapons.Code
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public Player Owner { get; set; }

        [field: SerializeField] public WeaponDataSO Data { get; private set; }

        public bool IsSkillUsing { get; set; }

        public virtual void Attack(int comboCount = 0) 
        {
            
        }

        //다른곳에서 코스트 확인 함수를 만들고 해당 함수를 통해서 스킬발동이 가능한지 확인
        //Player Cost Module 만들고 해당 클래스에서 이벤트를 통해서 코스트 변결ㅇd
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
