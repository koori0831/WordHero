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

        public void ExecuteEffect(SkillContext context)
        {
            Collider[] hitColliders = Physics.OverlapSphere(context.Caster.transform.position, Radius, TargetLayer);

            foreach (var hitCollider in hitColliders)
            {
                Agent agent = hitCollider.GetComponent<Agent>();
                AgentStatusModule statusModule = agent.GetModule<AgentStatusModule>();
                if (statusModule != null)
                {
                    statusModule.AddStatus(StatusEffect);
                }
            }
        }
    }
}
