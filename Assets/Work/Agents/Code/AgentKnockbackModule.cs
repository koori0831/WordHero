using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Agents.Code
{
    public class AgentKnockbackModule : MonoBehaviour, IAgentModule
    {
        private Agent _owner;
        private AgentMovementModule _mover;
        private AgentStatusModule _statusModule;

        public KnockbackData LastKnockbackData { get; private set; }


        public void Initialize(Agent agent)
        {
            _owner = agent;
            _owner.OnKnockbackEvent.AddListener(ApplyKnockback);
            _statusModule = _owner.GetModule<AgentStatusModule>();
            _mover = _owner.GetModule<AgentMovementModule>(true);
        }

        public void ApplyKnockback(KnockbackData knockbackData)
        {
            if (_statusModule.HasStatusEffect(StatusType.HitImmunity))
                return;
            if (_statusModule.HasStatusEffect(StatusType.SuperArmor))
                return;
            if (_statusModule.HasStatusEffect(StatusType.Invincible))
                return;
            if (_statusModule.HasStatusEffect(StatusType.KnockbackImmune))
                return;

            LastKnockbackData = knockbackData;
            _mover.KnockBack(knockbackData);
        }
    }
}