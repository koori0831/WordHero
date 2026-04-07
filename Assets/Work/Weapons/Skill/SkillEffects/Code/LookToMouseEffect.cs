using System;
using Work.Weapons.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class LookToMouseEffect : ISkillEffect
    {
        public void ExecuteEffect(SkillContext context)
        {
            context.Caster.LockOn();
        }
    }
}