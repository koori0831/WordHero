using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;
using Work.Weapons.HitBox.Code;

namespace Work.Weapons.HitBox.Effects.Code
{
    [Serializable]
    public class StatusHitEffect : IHitEffect
    {
        public StatusEffect StatusEffect;

        public void ExecuteHit(GameObject caster, GameObject target, Vector3 hitPoint)
        {
            if (target.TryGetComponent(out AgentStatusModule statusModule))
            {
                if (StatusEffect != null)
                {
                    statusModule.AddStatus(StatusEffect);
                }
            }
        }
    }
}
