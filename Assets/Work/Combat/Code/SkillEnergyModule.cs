using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;

namespace Work.Combat.Code
{
    public class SkillEnergyModule : MonoBehaviour, IAgentModule
    {
        private Agent _agent;
        [SerializeField] private int _maxEnergy;

        private float _energy;
        public int Energy
        {
            get => (int)_energy;
        }

        public void Initialize(Agent agent)
        {
            _agent = agent;
            Bus<GetSkillEnergyEvent>.Events += OnGetEnergyEvent;
        }

        private void OnGetEnergyEvent(GetSkillEnergyEvent evt)
        {
            float amount = evt.amount;
            _energy += Mathf.Clamp(amount, 0, _maxEnergy);
        }

        public bool TryUseCost(int requiredCost)
        {
            if (Energy >= requiredCost)
            {
                _energy -= requiredCost;
                return true;
            }
            return false;
        }
    }
}