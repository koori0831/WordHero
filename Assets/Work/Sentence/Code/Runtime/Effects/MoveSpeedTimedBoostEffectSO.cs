using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Sentence.Code.Runtime.Effects
{
    [CreateAssetMenu(fileName = "MoveSpeedTimedBoostEffect", menuName = "SO/Sentence/Effect/MoveSpeedTimedBoost", order = 23)]
    public class MoveSpeedTimedBoostEffectSO : SentenceEffectSO
    {
        [SerializeField] private StatusEffect speedUpTemplate;

        public override void Fire(in SentenceEffectFireContext context)
        {
            if (context.Owner == null || speedUpTemplate == null) return;

            Agent owner = context.Owner.GetComponent<Agent>();
            if (owner == null) return;

            AgentStatusModule statusModule = owner.GetModule<AgentStatusModule>(true);
            if (statusModule == null) return;

            statusModule.AddStatus(speedUpTemplate);
        }
    }
}
