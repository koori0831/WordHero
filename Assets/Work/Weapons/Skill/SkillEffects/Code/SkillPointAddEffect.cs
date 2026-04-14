using System;
using Work.Combat.Code;
using Work.Weapons.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class SkillPointAddEffect : ISkillEffect
    {
        public float EnergyAddAmount;

        public void ExecuteEffect(SkillContext context)
        {
            SkillEnergyModule skillPointModule = context.Caster.GetModule<SkillEnergyModule>();
            skillPointModule.AddSkillPoint(EnergyAddAmount);
        }
    }
}