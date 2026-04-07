using GondrLib.Dependencies;
using UnityEngine;
using Work.Combat.Code;
using Work.Players.Code;

namespace Work.SkillEnergyUI.Code
{
    public class SkillEnergyPanel : MonoBehaviour
    {
        [Inject] private Player _player;
        private SkillEnergyValue EnergyContainer;

        [SerializeField] private EnergyFillUI energyFillUI;

        private void Awake()
        {
            
        }

        public void Start()
        {
            SkillEnergyModule energyModule = _player.GetModule<SkillEnergyModule>(true);
            EnergyContainer = energyModule.EnergyContainer;
            EnergyContainer.OnChangedEvent += HandleEnergyChangeEvent;
            EnergyContainer.OnInsufficientCostEvent += HandleInsufficientCostEvent;
            EnableAllUI();
        }

        private void OnDestroy()
        {
            if (EnergyContainer == null)
            {
                return;
            }

            EnergyContainer.OnChangedEvent -= HandleEnergyChangeEvent;
            EnergyContainer.OnInsufficientCostEvent -= HandleInsufficientCostEvent;
        }

        private void HandleEnergyChangeEvent()
        {
            energyFillUI.RefreshUI(EnergyContainer);
        }

        private void HandleInsufficientCostEvent(int requiredCost, float currentEnergy)
        {
            energyFillUI.PlayInsufficientShake(requiredCost, currentEnergy);
        }

        private void EnableAllUI()
        {
            energyFillUI.EnableFor(EnergyContainer, EnergyContainer.MaxValue);
        }
    }
}
