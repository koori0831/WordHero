using UnityEngine;
using Work.Weapons.Skill.Code;

namespace Work.Weapons.Code
{
    [CreateAssetMenu(fileName = "WeaponDataSO", menuName = "SO/WeaponDataSO", order = 1)]
    public class WeaponDataSO : ScriptableObject
    {
        public string WeaponName;
        public string WeaponDescription;
        public WeaponType Type;
        public Vector3 HandlerPosition;
        public Vector3 HandlerRotation;
        public Sprite WeaponIcon;
        public int BaseDamage;
        public float AttackSpeed;
        public SkillDataSO PrimarySkill;
        public SkillDataSO SecondarySkill;
        public SkillDataSO TriggerSkill;
    }
}
