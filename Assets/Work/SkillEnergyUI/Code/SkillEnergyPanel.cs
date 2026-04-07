using GondrLib.Dependencies;
using System;
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
            EnableAllUI();
        }

        private void OnDestroy()
        {
            EnergyContainer.OnChangedEvent -= HandleEnergyChangeEvent;
        }

        private void HandleEnergyChangeEvent()
        {
            energyFillUI.RefreshUI(EnergyContainer);
        }

        private void EnableAllUI()
        {
            energyFillUI.EnableFor(EnergyContainer, EnergyContainer.MaxValue);
        }
    }
}