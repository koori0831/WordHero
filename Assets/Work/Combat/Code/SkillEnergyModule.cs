using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;

namespace Work.Combat.Code
{
    [Serializable]
    public class SkillEnergyValue
    {
        private int _maxEnergy;
        private float _energy = 0;

        public Action OnChangedEvent;
        public Action<int, float> OnInsufficientCostEvent;

        public SkillEnergyValue(int maxEnergy)
        {
            _maxEnergy = maxEnergy;
        }

        public float Energy
        {
            get => _energy;
            private set => _energy = Mathf.Clamp(value, 0, _maxEnergy);
        }

        public int MaxValue => _maxEnergy;

        public static SkillEnergyValue operator +(SkillEnergyValue energyValue, float amount)
        {
            energyValue.Energy += amount;
            return energyValue;
        }

        public static SkillEnergyValue operator -(SkillEnergyValue energyValue, float amount)
        {
            energyValue.Energy -= amount;
            return energyValue;
        }

        public static implicit operator float(SkillEnergyValue energyValue)
        {
            return energyValue.Energy;
        }
    }

    public class SkillEnergyModule : MonoBehaviour, IAgentModule
    {
        private Agent _agent;
        [SerializeField] private int maxEnergy;
        public SkillEnergyValue EnergyContainer { get; private set; }
        public void AddSkillPoint (float amount)
        {
            EnergyContainer += amount;
            EnergyContainer.OnChangedEvent?.Invoke();
        }

        public void Initialize(Agent agent)
        {
            _agent = agent;
            EnergyContainer = new SkillEnergyValue(maxEnergy);
            Bus<GetSkillEnergyEvent>.Events += OnGetEnergyEvent;
        }

        private void OnDestroy()
        {
            Bus<GetSkillEnergyEvent>.Events -= OnGetEnergyEvent;
        }

        private float cost = 0;

        private void OnGetEnergyEvent(GetSkillEnergyEvent evt)
        {
            cost+= evt.amount;
            EnergyContainer += evt.amount;
            EnergyContainer.OnChangedEvent?.Invoke();
        }

        public bool TryUseCost(int requiredCost)
        {
            if (EnergyContainer >= requiredCost)
            {
                EnergyContainer -= (float)requiredCost;
                EnergyContainer.OnChangedEvent?.Invoke();

                return true;
            }

            EnergyContainer.OnInsufficientCostEvent?.Invoke(requiredCost, EnergyContainer.Energy);
            return false;
        }
    }
}
