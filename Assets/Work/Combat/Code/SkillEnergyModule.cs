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
        public float Energy
        {
            get => (int)_energy;
            private set => _energy = Mathf.Clamp(value, 0, _maxEnergy);
        }

        public void Initialize(Agent agent)
        {
            _agent = agent;
            Bus<GetSkillEnergyEvent>.Events += OnGetEnergyEvent;
        }

        private void OnGetEnergyEvent(GetSkillEnergyEvent evt)
        {
            float amount = evt.amount;
            Energy += amount;
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