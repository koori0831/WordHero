using System.Collections.Generic;
using UnityEngine;

namespace Work.Weapon.Code
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "SO/SkillData")]
    public class SkillDataSO : ScriptableObject
    {
        public string SkillName;
        public int Cost;
        public string AnimParam;

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
