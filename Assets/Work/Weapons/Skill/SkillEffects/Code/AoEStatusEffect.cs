using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;
using Work.Weapons.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class AoEStatusEffect : ISkillEffect
    {
        public float Radius;
        public StatusEffect StatusEffect;
        public LayerMask TargetLayer;

        public void ExecuteEffect(Transform caster, Vector3 target, Vector3 direction)
        {
            Collider[] hitColliders = Physics.OverlapSphere(caster.transform.position, Radius, TargetLayer);

            foreach (var hitCollider in hitColliders)
            {
                AgentStatusModule statusModule = hitCollider.GetComponent<AgentStatusModule>();
                if (statusModule != null)
                {
                    statusModule.AddStatus(StatusEffect);
                }
            }
        }
    }
}
