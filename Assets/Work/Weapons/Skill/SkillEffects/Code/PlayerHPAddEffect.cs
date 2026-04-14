using System;
using Work.Weapons.Code;
using Work.Players.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class PlayerHPAddEffect : ISkillEffect
    {
        public int HPAddAmount;

        public void ExecuteEffect(SkillContext context)
        {
            PlayerHealthModule healthModule = context.Caster.GetModule<PlayerHealthModule>();
            healthModule.Heal(HPAddAmount);
        }
    }
}