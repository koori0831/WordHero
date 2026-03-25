using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;
using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class AoEStatusEffect : ISkillEffect
    {
        public float Radius;
        public StatusEffect StatusEffect;
        public LayerMask TargetLayer;

        public void ExecuteEffect(Player caster, Vector3 target, Vector3 direction)
        {
            Collider[] hitColliders = Physics.OverlapSphere(caster.transform.position, Radius, TargetLayer);

            foreach (var hitCollider in hitColliders)
            {
                Agent agent = hitCollider.GetComponent<Agent>();
                AgentStatusModule statusModule = agent.GetModule<AgentStatusModule>();
                if (statusModule != null)
                {
                    Debug.Log("아니 되자나요");
                    statusModule.AddStatus(StatusEffect);
                }
            }
        }
    }
}
