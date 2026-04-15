using UnityEngine;
using Work.Weapons.Skill.Code;
using Alchemy.Inspector;
using System.Collections.Generic;
using Work.Information.Code;

namespace Work.Weapons.Code
{
    [CreateAssetMenu(fileName = "WeaponDataSO", menuName = "SO/WeaponDataSO", order = 1)]
    public class WeaponDataSO : InfoDataSO
    {
        public WeaponType Type;
        [SerializeReference]
        public IImprintTriggerEvent ImprintTriggerEvent;
        public string TriggerDescription;
        private bool _isMelee { get { return Type == WeaponType.Melee; } }


        [ShowIf(nameof(_isMelee))]
        public List<ComboHitBox> ComboHitBoxes;
        public Vector3 HandlerPosition;
        public Vector3 HandlerRotation;
        public Sprite WeaponIcon;
        public SkillDataSO PrimarySkill;
        public SkillDataSO SecondarySkill;
        public AnimatorOverrideController AnimSet;
    }
}
