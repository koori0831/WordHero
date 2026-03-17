using Code.FSM;
using System.Collections.Generic;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.Weapons.Skill.Code
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "SO/SkillData")]
    public class SkillDataSO : ScriptableObject
    {
        public string SkillName;
        public string SkillDescription;
        public Sprite SkillIcon;
        public int Cost;
        public StateSO AnimParam;

        [SerializeReference]
        public List<ISkillEffect> Effects = new List<ISkillEffect>();

        public void Cast(Transform caster, Vector3 target, Vector3 direction)
        {
            foreach (var effect in Effects)
            {
                effect.ExecuteEffect(caster, target, direction);
            }
        }
    }
}
