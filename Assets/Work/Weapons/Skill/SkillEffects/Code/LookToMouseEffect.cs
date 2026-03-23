using UnityEngine;
using Work.Weapons.Code;
using Work.Players.Code;
using System;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class LookToMouseEffect : ISkillEffect
    {
        public void ExecuteEffect(Player caster, Vector3 target, Vector3 direction)
        {
            caster.LockOn();
        }
    }
}