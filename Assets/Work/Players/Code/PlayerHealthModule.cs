using System;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Players.Code
{
    public class PlayerHealthModule : AgentHealthModule
    {
        private AgentStatusModule _statusModule;

        public event Action Damaged;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _statusModule = _owner.GetModule<AgentStatusModule>(true);
            OnDeath.AddListener(_owner.Die);
        }

        public override void TakeDamage(int damageAmount)
        {
            if (_statusModule != null)
            {
                if (_statusModule.HasStatusEffect(StatusType.HitImmunity)) return;
                if (_statusModule.HasStatusEffect(StatusType.Invincible)) return;
            }

            int previousHealth = CurrentHealth;
            base.TakeDamage(damageAmount);

            if (CurrentHealth < previousHealth && CurrentHealth > 0)
            {
                Damaged?.Invoke();
            }
        }
    }
}
