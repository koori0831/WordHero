using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Enemies.Code.AttackModules;

namespace Work.Enemies.Code
{
    public class EnemyAnimationTriggerModule : MonoBehaviour, IAgentModule
    {
        private Enemy _owner;

        public Action OnAnimationEnd;
        public Action OnAttackEvent;

        public void Initialize(Agent agent)
        {
            _owner = agent as Enemy;
        }

        public void AnimationEndTrigger()
        {
            OnAnimationEnd?.Invoke();
        }

        public void HandleAttackTrigger()
        {
            OnAttackEvent?.Invoke();
        }

        public void HandleSpearAttackTrigger()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.Attack();
        }

        public void PlaySpearPrepareEffect()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.PlayPrepareEffect();
        }

        public void ShowSpearWarningDecal()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.ShowWarningDecal();
        }

        public void HideSpearWarningDecal()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.HideWarningDecal();
        }

        public void ResetSpearCombo()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.ResetCombo();
        }

        public void ShowChargeWarningDecal()
        {
            _owner.GetModule<ChargeAttackModule>(true)?.ShowChargeWarningDecal();
        }

        public void StartCharge()
        {
            _owner.GetModule<ChargeAttackModule>(true)?.StartCharge();
        }

        public void EndCharge()
        {
            _owner.GetModule<ChargeAttackModule>(true)?.EndCharge();
        }

        public void EndChargeAnimationTrigger()
        {
            ChargeAttackModule chargeAttackModule = _owner.GetModule<ChargeAttackModule>(true);
            if (chargeAttackModule == null)
            {
                AnimationEndTrigger();
                return;
            }

            chargeAttackModule.EndCharge();
            AnimationEndTrigger();
        }

        public void HandleChargeMeleeAttackTrigger()
        {
            _owner.GetModule<ChargeAttackModule>(true)?.MeleeAttack();
        }

        public void ResetCharge()
        {
            _owner.GetModule<ChargeAttackModule>(true)?.ResetCharge();
        }

        public void SwitchToChargeAttackPhase()
        {
        }

        public void SwitchToMeleeAttackPhase()
        {
        }
    }
}
