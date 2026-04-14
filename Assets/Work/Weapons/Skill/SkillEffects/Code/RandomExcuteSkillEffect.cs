using UnityEngine;
using Work.Weapons.Code;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class RandomExcuteSkillEffect : ISkillEffect
    {
        [SerializeReference]
        public List<ISkillEffect> PossibleEffects;

        public void ExecuteEffect(SkillContext context)
        {
            if (PossibleEffects.Count == 0)
                return;
            int index = Random.Range(0, PossibleEffects.Count);
            PossibleEffects[index].ExecuteEffect(context);
        }
    }
}